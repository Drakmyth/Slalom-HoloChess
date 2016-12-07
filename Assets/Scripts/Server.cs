using System;
using System.Linq;
using Assets.Scripts.MessageModels;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts
{
    public class Server : NetworkBehaviour
    {

        public int Port = 1300;
        public bool IsServerStarted;
        private GameState _gameState;
        private bool _isGuestReady = false;
        private bool _isHostReady = false;
//        private GameMode _gameMode;
        private bool _isLocalSinglePlayer = true;

        void Update()
        {
            if (!IsServerStarted)
            {
                return;
            }

            if (!_isGuestReady && _isLocalSinglePlayer)
            {
                _isGuestReady = true;
            }
        }

        public void Init(string ipAddress = "127.0.0.1")
        {
            DontDestroyOnLoad(this);

            try
            {

                NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
                
                NetworkServer.RegisterHandler(CustomMessageTypes.StateAck, OnStateAck);

                NetworkServer.RegisterHandler(CustomMessageTypes.SelectMonsterRequest, OnSelectMonster);

                NetworkServer.RegisterHandler(CustomMessageTypes.SelectActionRequest, OnSelectAction);

                NetworkServer.RegisterHandler(CustomMessageTypes.AttackRequest, OnProcessAttackAction);

                NetworkServer.RegisterHandler(CustomMessageTypes.PushDestinationRequest, OnProcessPushDestination);

                NetworkServer.Listen(ipAddress, Port);

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

        public void SendToClient(string data, Client client, short messageTypeId = 0)
        {
            try
            {
                NetworkServer.SendToClient(client.NetClient.connection.connectionId, messageTypeId, new StringMessage(data));
            }
            catch (NullReferenceException npe)
            {
                Debug.Log("Connection does not exist for " + client.ClientName);
                Debug.Log(npe.Message);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ShutDown()
        {
            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
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
                    //TODO: Net push game state to client
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
                    //TODO: Net push game state to client
                }

            }
        }
    }
}
