using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MessageModels;
using DejarikLibrary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

namespace Assets.Scripts.AI
{
    class SithAI : Client
    {
        private readonly Random _random = new Random();
        private Dictionary<int, int> _friendlyMonsterState = new Dictionary<int, int>();
        private Dictionary<int, int> _enemyMonsterState = new Dictionary<int, int>();
        private List<int> _monstersWithAvailableMoves;

        // Use this for initialization
        void Start()
        {
            //DontDestroyOnLoad(this);

            IsHost = true;
            ClientName = "Sith AI";
           
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(string ipAddress)
        {
            try
            {
                NetClient = new NetworkClient();

                NetClient.RegisterHandler(CustomMessageTypes.GameStart, OnGameStart);

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMonstersResponse, OnAvailableMonsters);

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMovesResponse, OnAvailableMoves);

                NetClient.RegisterHandler(CustomMessageTypes.AttackPushResponse, OnAttackPushResponse);

                NetClient.RegisterHandler(CustomMessageTypes.GameState, OnGameStateResponse);

                NetClient.RegisterHandler(CustomMessageTypes.GameStateSync, OnGameStateSyncResponse);

                NetClient.Connect(ipAddress, 1300);

                Debug.Log("Sith AI");

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }


        private void OnGameStart(NetworkMessage netMsg)
        {
            GameStartMessage gameStartMessage = netMsg.ReadMessage<GameStartMessage>();

            //Convert json strings to objects
            _friendlyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.HostMonsters);
            _enemyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.GuestMonsters);

        }

        private void OnAvailableMonsters(NetworkMessage msg)
        {
            AvailableMonstersResponseMessage message = msg.ReadMessage<AvailableMonstersResponseMessage>();

            //reverse 1 : 1 relationship for easy monster selection
            Dictionary<int, int> nodeMonsters = _friendlyMonsterState.ToDictionary(m => m.Value, m => m.Key);

            if (message.ActionNumber != 1 && message.ActionNumber != 2)
            {
                return;
            }

            // get first monster
            var firstIndex = message.AvailableMonsterNodeIds[0];

            NetClient.Send(CustomMessageTypes.SelectMonsterRequest, new SelectMonsterRequestMessage
            {
                ActionNumber = message.ActionNumber,
                SubActionNumber = message.SubActionNumber,
                SelectedMonsterTypeId = nodeMonsters[firstIndex]
            });
        }

        private void OnAvailableMoves(NetworkMessage msg)
        {
            AvailableMovesResponseMessage message = msg.ReadMessage<AvailableMovesResponseMessage>();

            if (message.ActionNumber != 1 && message.ActionNumber != 2)
            {
                return;
            }

            if (message.AvailableAttackNodeIds.Any())
            {
                // pick first attack result since they are ordered by efficiency
                NetClient.Send(CustomMessageTypes.SelectActionRequest, new SelectActionRequestMessage
                {
                    ActionNumber = message.ActionNumber,
                    SubActionNumber = message.SubActionNumber,
                    SelectedNodeId = message.AvailableAttackNodeIds.First()
                });
            }
            else
            {

                if (message.AvailableMoveNodeIds.Any())
                {
                    int randInt = _random.Next(0, message.AvailableMoveNodeIds.Length - 1);
                    NetClient.Send(CustomMessageTypes.SelectActionRequest, new SelectActionRequestMessage
                    {
                        ActionNumber = message.ActionNumber,
                        SubActionNumber = message.SubActionNumber,
                        SelectedNodeId = message.AvailableMoveNodeIds[randInt]
                    });
                }
                else
                {
                    _monstersWithAvailableMoves.Remove(message.SelectedMonsterTypeId);

                    //TODO: this would perhaps be better as a custom message type that triggers a pass
                    NetClient.Send(CustomMessageTypes.SelectActionRequest, new SelectActionRequestMessage
                    {
                        ActionNumber = message.ActionNumber,
                        SubActionNumber = message.SubActionNumber,
                        SelectedNodeId = -1
                    });
                }
            }
        }

        private void OnAttackPushResponse(NetworkMessage msg)
        {
            AttackPushResponseMessage message = msg.ReadMessage<AttackPushResponseMessage>();

            bool canPush = (message.ActionNumber == 1 || message.ActionNumber == 2) && message.SubActionNumber == 6;
            bool canCounterPush = (message.ActionNumber == 3 || message.ActionNumber == 4) && message.SubActionNumber == 7;

            if (message.AvailablePushDestinationIds.Any() && (canPush || canCounterPush))
            {
                int selectedDestinationNodeId = message.AvailablePushDestinationIds.ElementAt(_random.Next(message.AvailablePushDestinationIds.Length));
                NetClient.Send(CustomMessageTypes.PushDestinationRequest, new PushDestinationRequestMessage
                {
                    ActionNumber = message.ActionNumber,
                    SubActionNumber = message.SubActionNumber,
                    SelectedNodeId = selectedDestinationNodeId
                });
            }
        }

        private void OnGameStateResponse(NetworkMessage msg)
        {
            GameStateMessage message = msg.ReadMessage<GameStateMessage>();

            _friendlyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(message.HostMonsterState);
            _enemyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(message.GuestMonsterState);
            _monstersWithAvailableMoves = _friendlyMonsterState.Keys.ToList();

        }

        private void OnGameStateSyncResponse(NetworkMessage msg)
        {
            GameStateSyncMessage message = msg.ReadMessage<GameStateSyncMessage>();

            _friendlyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(message.HostMonsterState);
            _enemyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(message.GuestMonsterState);
            _monstersWithAvailableMoves = _friendlyMonsterState.Keys.ToList();

        }
    }
}