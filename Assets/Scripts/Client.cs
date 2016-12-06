using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MessageModels;
using DejarikLibrary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Client : NetworkBehaviour
    {

        public bool IsHost = false;
        //TODO: Net make this private and abstract necessary functionality out
        public NetworkClient NetClient;
        public string ClientName = "client";
        public ClientGameState GameState;

        public void Init(string hostAddress, int port)
        {
            DontDestroyOnLoad(this);

            if (!IsHost)
            {
                ClientName = "guest";
            }
                        
            try
            {
                NetClient = new NetworkClient();

                NetClient.RegisterHandler(MsgType.Connect, OnConnected);

                NetClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);

                NetClient.RegisterHandler(MsgType.Error, OnError);

                NetClient.RegisterHandler(CustomMessageTypes.GameStart, OnGameStart);

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMonstersResponse, OnAvailableMonsters);

                NetClient.RegisterHandler(CustomMessageTypes.SelectMonsterResponse, OnSelectMonster);

                NetClient.RegisterHandler(CustomMessageTypes.AvailableMovesResponse, OnAvailableMoves);

                NetClient.RegisterHandler(CustomMessageTypes.SelectMoveActionResponse, OnSelectMoveAction);

                NetClient.RegisterHandler(CustomMessageTypes.SelectAttackActionResponse, OnSelectAttackAction);
                
                NetClient.RegisterHandler(CustomMessageTypes.AttackKillResponse, OnAttackKillResponse);

                NetClient.RegisterHandler(CustomMessageTypes.AttackPushResponse, OnAttackPushResponse);

                NetClient.RegisterHandler(CustomMessageTypes.PushDestinationResponse, OnPushDestinationResponse);

                NetClient.Connect(hostAddress, port);

                Debug.Log("Client");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        public void InitHost()
        {

            IsHost = true;
            ClientName = "host";
            try
            {
                Init("127.0.0.1", 1300);
                Debug.Log("HostClient");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        public bool Send(short messageType, MessageBase message)
        {
            return NetClient.Send(messageType, message);
        }

        private void OnConnected(NetworkMessage netMsg)
        {
            Debug.Log("Connected to server");
        }

        private void OnDisconnected(NetworkMessage msg)
        {
            Debug.Log("Disconnected from server");
        }

        private void OnError(NetworkMessage msg)
        {
            Debug.Log("Error connecting with code " + msg.reader.ReadString());
        }

        private void OnAvailableMonsters(NetworkMessage msg)
        {
            Debug.Log("Available monsters");
            AvailableMonstersResponseMessage message = msg.ReadMessage<AvailableMonstersResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmAvailableMonsters(message.AvailableMonsterNodeIds.ToList(), message.ActionNumber,
                message.SubActionNumber);
        }

        private void OnSelectMonster(NetworkMessage msg)
        {
            Debug.Log("Monster selected");
            SelectMonsterResponseMessage message = msg.ReadMessage<SelectMonsterResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmSubActionTwo(message.SelectedMonsterTypeId, message.ActionNumber, message.SubActionNumber);

        }

        private void OnAvailableMoves(NetworkMessage msg)
        {
            Debug.Log("Available moves calculated");
            AvailableMovesResponseMessage message = msg.ReadMessage<AvailableMovesResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmSubActionThree(message.AvailableMoveNodeIds.ToList(), message.AvailableAttackNodeIds.ToList(), message.ActionNumber, message.SubActionNumber);

        }

        private void OnSelectMoveAction(NetworkMessage msg)
        {
            Debug.Log("Action selected");

            SelectMoveResponseMessage message = msg.ReadMessage<SelectMoveResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmSelectMoveAction(message.MovementPathIds.ToList(), message.DestinationNodeId, message.ActionNumber, message.SubActionNumber);
        }

        private void OnSelectAttackAction(NetworkMessage msg)
        {
            Debug.Log("Action selected");

            SelectAttackResponseMessage message = msg.ReadMessage<SelectAttackResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmSelectAttackAction(message.AttackNodeId, message.ActionNumber, message.SubActionNumber);
        }

        private void OnAttackKillResponse(NetworkMessage msg)
        {
            Debug.Log("Kill Attack Result");

            AttackKillResponseMessage message = msg.ReadMessage<AttackKillResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmAttackResult((AttackResult)message.AttackResultId, message.AttackingMonsterTypeId, message.DefendingMonsterTypeId, message.ActionNumber, message.SubActionNumber);
        }

        private void OnAttackPushResponse(NetworkMessage msg)
        {
            Debug.Log("Push Attack Result");

            AttackPushResponseMessage message = msg.ReadMessage<AttackPushResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmAttackPushResult((AttackResult)message.AttackResultId, message.AvailablePushDestinationIds, message.AttackingMonsterTypeId, message.DefendingMonsterTypeId, message.ActionNumber, message.SubActionNumber);
        }

        private void OnPushDestinationResponse(NetworkMessage msg)
        {
            Debug.Log("Push destination received");

            PushDestinationResponseMessage message = msg.ReadMessage<PushDestinationResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmPushDestination(message.PathToDestinationNodeIds, message.DestinationNodeId,
                message.ActionNumber, message.SubActionNumber);
        }

        public void SendStateAck(int actionNumber, int subActionNumber)
        {
            Send(CustomMessageTypes.StateAck, new StateAckMessage
            {
                ActionNumber = actionNumber,
                SubActionNumber = subActionNumber,
                Message = IsHost? "HostClient up to date" : "GuestClient up to date",
                IsHost = IsHost
            });
        }

        private void OnGameStart(NetworkMessage netMsg)
        {
            GameStartMessage gameStartMessage = netMsg.ReadMessage<GameStartMessage>();

            //Convert json strings to objects
            Dictionary<int, int> friendlyMonsterIds = IsHost? JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.HostMonsters) : JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.GuestMonsters);
            Dictionary<int, int> enemyMonsterIds = IsHost ? JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.GuestMonsters) : JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.HostMonsters);

            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.FriendlyMonsterInitialNodeIds = friendlyMonsterIds;
            gameManager.EnemyMonsterInitialNodeIds = enemyMonsterIds;

            StartCoroutine(LoadGameScene());
        }

        IEnumerator LoadGameScene()
        {
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync("dejarik");

            while (!loadSceneOperation.isDone)
            {
                Debug.Log("Loading the Game Scene");
                yield return null;
            }

        }

        private int AdjustActionNumber(int actionNumber)
        {
            if (!IsHost)
            {
                return (actionNumber + 1) % 4 + 1;
            }

            return actionNumber;
        }


    }
}
