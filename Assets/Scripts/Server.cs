using System;
using System.Linq;
using Assets.Scripts.MessageModels;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

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
        private int _hostConnectionId;
        private int _guestConnectionId;

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


            //TODO: Net this is a work around until we can get IP through code or spin up a matchmaking service
            IpAddress = ipAddress;

            try
            {

                NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);

                NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);

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
            int connectionCount = NetworkServer.connections.Count(c => c != null);
            if (connectionCount == 1)
            {
                _hostConnectionId = netMsg.conn.connectionId;
            }

            if (NetworkServer.connections.Count(c => c != null) == 2)
            {
                _guestConnectionId = netMsg.conn.connectionId;
                _gameState = gameObject.AddComponent<GameState>();
            }

            if (NetworkServer.connections.Count(c => c != null) > 2)
            {
                netMsg.conn.Send(CustomMessageTypes.GameStart, new GameStartMessage
                {
                    HostMonsters = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    GuestMonsters = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    ActionNumber = 1,
                    SubActionNumber = 1
                });

                netMsg.conn.Send(CustomMessageTypes.GameStateSync, new GameStateSyncMessage
                {
                    ActionNumber = _gameState.ActionNumber,
                    SubActionNumber = _gameState.SubActionNumber,
                    Message = "Game State Sync",
                    HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    SelectedMonsterTypeId = _gameState.SelectedMonster != null ? _gameState.SelectedMonster.MonsterTypeId : 0,
                    MovementPathIds = _gameState.SelectedMovementPath != null ? JsonConvert.SerializeObject(_gameState.SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray()) : null,
                    DestinationNodeId = _gameState.SelectedMovementPath != null ? _gameState.SelectedMovementPath.DestinationNode.Id : 0,
                    SelectedAttackNodeId = _gameState.SelectedAttackNode != null ? _gameState.SelectedAttackNode.Id : 0,
                    AvailablePushDestinationIds = JsonConvert.SerializeObject(_gameState.AvailablePushDestinations.Select(n => n.Id))
                });
            }

        }

        private void OnClientDisconnected(NetworkMessage netMsg)
        {
            if (_isLocalSinglePlayer)
            {
                return;
            }

            if (netMsg.conn.connectionId == _hostConnectionId || netMsg.conn.connectionId == _guestConnectionId)
            {
                SendToAll(CustomMessageTypes.GameEnd, new GameEndMessage {IsHostWinner = false, IsGuestWinner = false});
            }
        }

        public void OnSelectMonster(NetworkMessage msg)
        {
            SelectMonsterRequestMessage message = msg.ReadMessage<SelectMonsterRequestMessage>();

            if (_gameState.SubActionNumber != message.SubActionNumber || _gameState.ActionNumber != message.ActionNumber)
            {
                SendToAll(CustomMessageTypes.GameStateSync, new GameStateSyncMessage
                {
                    ActionNumber = _gameState.ActionNumber,
                    SubActionNumber = _gameState.SubActionNumber,
                    Message = "Game State Sync",
                    HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    SelectedMonsterTypeId = _gameState.SelectedMonster != null ? _gameState.SelectedMonster.MonsterTypeId : 0,
                    MovementPathIds = _gameState.SelectedMovementPath != null ? JsonConvert.SerializeObject(_gameState.SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray()) : null,
                    DestinationNodeId = _gameState.SelectedMovementPath != null ? _gameState.SelectedMovementPath.DestinationNode.Id : 0,
                    SelectedAttackNodeId = _gameState.SelectedAttackNode != null ? _gameState.SelectedAttackNode.Id : 0,
                    AvailablePushDestinationIds = JsonConvert.SerializeObject(_gameState.AvailablePushDestinations.Select(n => n.Id))
                });
                return;
            }

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
            SelectActionRequestMessage message = msg.ReadMessage<SelectActionRequestMessage>();

            if (_gameState.SubActionNumber != message.SubActionNumber || _gameState.ActionNumber != message.ActionNumber)
            {
                SendToAll(CustomMessageTypes.GameStateSync, new GameStateSyncMessage
                {
                    ActionNumber = _gameState.ActionNumber,
                    SubActionNumber = _gameState.SubActionNumber,
                    Message = "Game State Sync",
                    HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    SelectedMonsterTypeId = _gameState.SelectedMonster != null ? _gameState.SelectedMonster.MonsterTypeId : 0,
                    MovementPathIds = _gameState.SelectedMovementPath != null ? JsonConvert.SerializeObject(_gameState.SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray()) : null,
                    DestinationNodeId = _gameState.SelectedMovementPath != null ? _gameState.SelectedMovementPath.DestinationNode.Id : 0,
                    SelectedAttackNodeId = _gameState.SelectedAttackNode != null ? _gameState.SelectedAttackNode.Id : 0,
                    AvailablePushDestinationIds = JsonConvert.SerializeObject(_gameState.AvailablePushDestinations.Select(n => n.Id))
                });
                return;
            }


            _isHostReady = false;
            _isGuestReady = false;

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

            if (_gameState.SubActionNumber != message.SubActionNumber || _gameState.ActionNumber != message.ActionNumber)
            {
                SendToAll(CustomMessageTypes.GameStateSync, new GameStateSyncMessage
                {
                    ActionNumber = _gameState.ActionNumber,
                    SubActionNumber = _gameState.SubActionNumber,
                    Message = "Game State Sync",
                    HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    SelectedMonsterTypeId = _gameState.SelectedMonster != null ? _gameState.SelectedMonster.MonsterTypeId : 0,
                    MovementPathIds = _gameState.SelectedMovementPath != null ? JsonConvert.SerializeObject(_gameState.SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray()) : null,
                    DestinationNodeId = _gameState.SelectedMovementPath != null ? _gameState.SelectedMovementPath.DestinationNode.Id : 0,
                    SelectedAttackNodeId = _gameState.SelectedAttackNode != null ? _gameState.SelectedAttackNode.Id : 0,
                    AvailablePushDestinationIds = JsonConvert.SerializeObject(_gameState.AvailablePushDestinations.Select(n => n.Id))
                });
                return;
            }

            _gameState.ProcessAttackAction(message.AttackingMonsterTypeId, message.DefendingMonsterTypeId);
        }

        public void OnProcessPushDestination(NetworkMessage msg)
        {
            PushDestinationRequestMessage message = msg.ReadMessage<PushDestinationRequestMessage>();

            if (_gameState.SubActionNumber != message.SubActionNumber || _gameState.ActionNumber != message.ActionNumber)
            {
                SendToAll(CustomMessageTypes.GameStateSync, new GameStateSyncMessage
                {
                    ActionNumber = _gameState.ActionNumber,
                    SubActionNumber = _gameState.SubActionNumber,
                    Message = "Game State Sync",
                    HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    SelectedMonsterTypeId = _gameState.SelectedMonster != null ? _gameState.SelectedMonster.MonsterTypeId : 0,
                    MovementPathIds = _gameState.SelectedMovementPath != null ? JsonConvert.SerializeObject(_gameState.SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray()) : null,
                    DestinationNodeId = _gameState.SelectedMovementPath != null ? _gameState.SelectedMovementPath.DestinationNode.Id : 0,
                    SelectedAttackNodeId = _gameState.SelectedAttackNode != null ? _gameState.SelectedAttackNode.Id : 0,
                    AvailablePushDestinationIds = JsonConvert.SerializeObject(_gameState.AvailablePushDestinations.Select(n => n.Id))
                });
                return;
            }

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
                    SendToAll(CustomMessageTypes.GameStateSync, new GameStateSyncMessage
                    {
                        ActionNumber = _gameState.ActionNumber,
                        SubActionNumber = _gameState.SubActionNumber,
                        Message = "Game State Sync",
                        HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                        GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                        SelectedMonsterTypeId = _gameState.SelectedMonster != null ? _gameState.SelectedMonster.MonsterTypeId : 0,
                        MovementPathIds = _gameState.SelectedMovementPath != null ? JsonConvert.SerializeObject(_gameState.SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray()) : null,
                        DestinationNodeId = _gameState.SelectedMovementPath != null ? _gameState.SelectedMovementPath.DestinationNode.Id : 0,
                        SelectedAttackNodeId = _gameState.SelectedAttackNode != null ? _gameState.SelectedAttackNode.Id : 0,
                        AvailablePushDestinationIds = JsonConvert.SerializeObject(_gameState.AvailablePushDestinations.Select(n => n.Id))
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
                    SendToAll(CustomMessageTypes.GameStateSync, new GameStateSyncMessage
                    {
                        ActionNumber = _gameState.ActionNumber,
                        SubActionNumber = _gameState.SubActionNumber,
                        Message = "Game State Sync",
                        HostMonsterState = JsonConvert.SerializeObject(_gameState.HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                        GuestMonsterState = JsonConvert.SerializeObject(_gameState.GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                        SelectedMonsterTypeId = _gameState.SelectedMonster != null ? _gameState.SelectedMonster.MonsterTypeId : 0,
                        MovementPathIds = _gameState.SelectedMovementPath != null? JsonConvert.SerializeObject(_gameState.SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray()) : null,
                        DestinationNodeId = _gameState.SelectedMovementPath != null ? _gameState.SelectedMovementPath.DestinationNode.Id : 0,
                        SelectedAttackNodeId = _gameState.SelectedAttackNode != null ? _gameState.SelectedAttackNode.Id : 0,
                        AvailablePushDestinationIds = JsonConvert.SerializeObject(_gameState.AvailablePushDestinations.Select(n => n.Id))
                    });
                }

            }
        }
    }
}
