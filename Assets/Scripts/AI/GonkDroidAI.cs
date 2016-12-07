using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MessageModels;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

namespace Assets.Scripts.AI
{
    class GonkDroidAI : Client
    {
        private readonly Random _random = new Random();
        private Dictionary<int, int> _friendlyMonsterState = new Dictionary<int, int>();
        private Dictionary<int, int> _enemyMonsterState = new Dictionary<int, int>();

        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this);

            IsHost = false;
            ClientName = "Gonk Droid AI";

            try
            {
                NetClient = new NetworkClient();

                NetClient.RegisterHandler(CustomMessageTypes.GameStart, OnGameStart);

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMonstersResponse, OnAvailableMonsters);

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMovesResponse, OnAvailableMoves);

                NetClient.RegisterHandler(CustomMessageTypes.AttackPushResponse, OnAttackPushResponse);

                NetClient.RegisterHandler(CustomMessageTypes.GameState, OnGameStateResponse);

                NetClient.Connect("127.0.0.1", 1300);

                Debug.Log("Gonk Droid AI");

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnGameStart(NetworkMessage netMsg)
        {
            GameStartMessage gameStartMessage = netMsg.ReadMessage<GameStartMessage>();

            //Convert json strings to objects
            _friendlyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.GuestMonsters);
            _enemyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.HostMonsters);

        }

        private void OnAvailableMonsters(NetworkMessage msg)
        {
            AvailableMonstersResponseMessage message = msg.ReadMessage<AvailableMonstersResponseMessage>();

            //reverse 1 : 1 relationship for easy monster selection
            Dictionary<int, int> nodeMonsters = _friendlyMonsterState.ToDictionary(m => m.Value, m => m.Key);

            if (message.ActionNumber != 3 && message.ActionNumber != 4)
            {
                return;
            }

            int randomIndex = _random.Next(0, message.AvailableMonsterNodeIds.Length - 1);

            int randomAvailableMonsterNodeId = message.AvailableMonsterNodeIds[randomIndex];

            NetClient.Send(CustomMessageTypes.SelectMonsterRequest, new SelectMonsterRequestMessage
            {
                ActionNumber = message.ActionNumber,
                SubActionNumber = message.SubActionNumber,
                SelectedMonsterTypeId = nodeMonsters[randomAvailableMonsterNodeId]
            });
        }

        private void OnAvailableMoves(NetworkMessage msg)
        {
            AvailableMovesResponseMessage message = msg.ReadMessage<AvailableMovesResponseMessage>();

            if (message.ActionNumber != 3 && message.ActionNumber != 4)
            {
                return;
            }

            if (message.AvailableAttackNodeIds.Any())
            {
                int randInt = _random.Next(0, message.AvailableAttackNodeIds.Length - 1);
                NetClient.Send(CustomMessageTypes.SelectActionRequest, new SelectActionRequestMessage
                {
                    ActionNumber = message.ActionNumber,
                    SubActionNumber = message.SubActionNumber,
                    SelectedNodeId = message.AvailableAttackNodeIds[randInt]
                });
            }
            else
            {
                int randInt = _random.Next(0, message.AvailableMoveNodeIds.Length - 1);
                NetClient.Send(CustomMessageTypes.SelectActionRequest, new SelectActionRequestMessage
                {
                    ActionNumber = message.ActionNumber,
                    SubActionNumber = message.SubActionNumber,
                    SelectedNodeId = message.AvailableMoveNodeIds[randInt]
                });
            }
        }

        private void OnAttackPushResponse(NetworkMessage msg)
        {
            AttackPushResponseMessage message = msg.ReadMessage<AttackPushResponseMessage>();

            bool canPush = (message.ActionNumber == 3 || message.ActionNumber == 4) && message.SubActionNumber == 6;
            bool canCounterPush = (message.ActionNumber == 1 || message.ActionNumber == 2) && message.SubActionNumber == 7;

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

        public void OnGameStateResponse(NetworkMessage msg)
        {
            GameStateMessage message = msg.ReadMessage<GameStateMessage>();

            _friendlyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(message.GuestMonsterState);
            _enemyMonsterState = JsonConvert.DeserializeObject<Dictionary<int, int>>(message.HostMonsterState);

        }
    }
}