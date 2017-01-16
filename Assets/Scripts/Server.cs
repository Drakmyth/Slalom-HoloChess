using System;
using System.Linq;
using Assets.Scripts.MessageModels;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts
{
    public class Server : NetworkBehaviour
    {

        public int Port = 1300;
        public string IpAddress = "";
        public bool IsServerStarted;
        private GameState _gameState;
        private bool _isGuestReady;
        private bool _isHostReady;
        private bool _isLocalSinglePlayer;

        void Update()
        {
            if (!IsServerStarted)
            {
                return;
            }

            if (!_isGuestReady && _isLocalSinglePlayer)
            {
                _isHostReady = true;
            }
        }

        public void InitSinglePlayer(string ipAddress = "127.0.0.1")
        {
            _isLocalSinglePlayer = true;
            Init(ipAddress);
        }

        public void Init(string ipAddress)
        {           
            ShutDown();

            DontDestroyOnLoad(this);

            IpAddress = ipAddress;

            //TODO: Net this is a work around until we can get IP through code or spin up a matchmaking service

            try
            {

                NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
                
                NetworkServer.RegisterHandler(CustomMessageTypes.StateAck, OnStateAck);

                NetworkServer.RegisterHandler(CustomMessageTypes.SelectMonsterRequest, OnSelectMonster);

                NetworkServer.RegisterHandler(CustomMessageTypes.SelectActionRequest, OnSelectAction);

                NetworkServer.RegisterHandler(CustomMessageTypes.AttackRequest, OnProcessAttackAction);

                NetworkServer.RegisterHandler(CustomMessageTypes.PushDestinationRequest, OnProcessPushDestination);

                NetworkServer.Listen(IpAddress, Port);

                IsServerStarted = true;
            }
            catch (Exception e)
            {
                Debug.Log("Server error: " + e.Message);
            }
        }

        public void SendToAll(short messageTypeId, MessageBase msg)
        {
            _isHostReady = false;
            _isGuestReady = false;
            NetworkServer.SendToAll(messageTypeId, msg);
        }

        public void ShutDown()
        {
            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
            NetworkServer.Reset();
        }

        public bool ClientsAreReady()
        {
            return _isHostReady && _isGuestReady;
        }

        private void OnClientConnected(NetworkMessage netMsg)
        {
            Debug.Log("Client has been connected to host");
            if (NetworkServer.connections.Count(c => c != null) == 2)
            {
                _gameState = gameObject.AddComponent<GameState>();
            }

        }

        public void OnSelectMonster(NetworkMessage msg)
        {
            SelectMonsterRequestMessage message = msg.ReadMessage<SelectMonsterRequestMessage>();

            _gameState.SelectMonster(message.SelectedMonsterTypeId);

            SendToAll(CustomMessageTypes.SelectMonsterResponse, new SelectMonsterResponseMessage
            {
                ActionNumber = _gameState.ActionNumber,
                SubActionNumber = _gameState.SubActionNumber,
                SelectedMonsterTypeId = _gameState.SelectedMonster.MonsterTypeId,
                Message = _gameState.SelectedMonster.Name,
                MessageTypeId = CustomMessageTypes.SelectMonsterResponse
                
            });
        }

        public void OnSelectAction(NetworkMessage msg)
        {
            _isHostReady = false;
            _isGuestReady = false;

            SelectActionRequestMessage message = msg.ReadMessage<SelectActionRequestMessage>();

            if (_gameState.SelectedMonster == null)
            {
                SendToAll(CustomMessageTypes.GameState, new GameStateMessage
                {
                    ActionNumber = _gameState.ActionNumber,
                    SubActionNumber = 2,
                    MessageTypeId = CustomMessageTypes.GameState
                });
                return;
            }

            _gameState.SelectAction(message.SelectedNodeId);
        }

        public void OnProcessAttackAction(NetworkMessage msg)
        {
            AttackRequestMessage message = msg.ReadMessage<AttackRequestMessage>();

            _gameState.ProcessAttackAction(message.AttackingMonsterTypeId, message.DefendingMonsterTypeId);
        }

        public void OnProcessPushDestination(NetworkMessage msg)
        {
            PushDestinationRequestMessage message = msg.ReadMessage<PushDestinationRequestMessage>();

            _gameState.ProcessPushDestination(message.SelectedNodeId);
        }

        public void OnStateAck(NetworkMessage msg)
        {
            StateAckMessage message = msg.ReadMessage<StateAckMessage>();

            if (message.IsHost)
            {
                if (message.ActionNumber == _gameState.ActionNumber && message.SubActionNumber == _gameState.SubActionNumber)
                {
                    _isHostReady = true;
                }
                else
                {
                    _isHostReady = false;
                    SendToAll(CustomMessageTypes.GameState, new GameStateMessage
                    {
                        ActionNumber = _gameState.ActionNumber,
                        SubActionNumber = _gameState.SubActionNumber,
                        Message = "Game State Sync",
                        HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                        GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id))
                    });
                }
            }
            else
            {
                if (message.ActionNumber == _gameState.ActionNumber && message.SubActionNumber == _gameState.SubActionNumber)
                {
                    _isGuestReady = true;
                }
                else
                {
                    _isGuestReady = false;
                    SendToAll(CustomMessageTypes.GameState, new GameStateMessage
                    {
                        ActionNumber = _gameState.ActionNumber,
                        SubActionNumber = _gameState.SubActionNumber,
                        Message = "Game State Sync",
                        HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                        GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id))
                    });
                }

            }
        }
    }
}
