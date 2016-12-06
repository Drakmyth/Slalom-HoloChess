using System;
using System.Linq;
using Assets.Scripts.MessageModels;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

namespace Assets.Scripts.AI
{
    class GonkDroidAI : Client
    {
        private readonly Random _random = new Random();

        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this);

            IsHost = false;
            ClientName = "Gonk Droid AI";

            try
            {
                NetClient = new NetworkClient();

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMonstersResponse, OnAvailableMonsters);

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMovesResponse, OnAvailableMoves);

                NetClient.RegisterHandler(CustomMessageTypes.AttackPushResponse, OnAttackPushResponse);

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

        private void OnAvailableMonsters(NetworkMessage msg)
        {
            AvailableMonstersResponseMessage message = msg.ReadMessage<AvailableMonstersResponseMessage>();

            if (message.ActionNumber != 3 && message.ActionNumber != 4)
            {
                return;
            }

            int randInt = _random.Next(0, message.AvailableMonsterNodeIds.Length - 1);

            NetClient.Send(CustomMessageTypes.SelectMonsterRequest, new SelectMonsterRequestMessage
            {
                ActionNumber = message.ActionNumber,
                SubActionNumber = message.SubActionNumber,
                SelectedMonsterTypeId = message.AvailableMonsterNodeIds[randInt]
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
                NetClient.Send(CustomMessageTypes.SelectMonsterRequest, new SelectMonsterRequestMessage
                {
                    ActionNumber = message.ActionNumber,
                    SubActionNumber = message.SubActionNumber,
                    SelectedMonsterTypeId = message.AvailableAttackNodeIds[randInt]
                });
            }
            else
            {
                int randInt = _random.Next(0, message.AvailableMoveNodeIds.Length - 1);
                NetClient.Send(CustomMessageTypes.SelectMonsterRequest, new SelectMonsterRequestMessage
                {
                    ActionNumber = message.ActionNumber,
                    SubActionNumber = message.SubActionNumber,
                    SelectedMonsterTypeId = message.AvailableMoveNodeIds[randInt]
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
    }
}