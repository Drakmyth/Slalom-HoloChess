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
        public bool IsPlayer = true;
        //TODO: Net make this private and abstract necessary functionality out
        public NetworkClient NetClient;
        public string ClientName = "client";
        public ClientGameState GameState;

        public void Init(string hostAddress, int port)
        {

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

                NetClient.RegisterHandler(CustomMessageTypes.GameStateSync, OnGameStateSyncResponse);

                NetClient.RegisterHandler(CustomMessageTypes.GameState, OnGameStateResponse);

                NetClient.RegisterHandler(CustomMessageTypes.GameEnd, OnGameEnd);

                NetClient.Connect(hostAddress, port);

                Debug.Log("Client");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        public void InitHost(string ipAddress)
        {

            IsHost = true;
            ClientName = "host";
            try
            {
                Init(ipAddress, 1300);
                Debug.Log("HostClient");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        public void InitObserver(string hostAddress, int port)
        {
            IsPlayer = false;
            ClientName = "observer";

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

                NetClient.RegisterHandler(CustomMessageTypes.GameStateSync, OnGameStateSyncResponse);

                NetClient.RegisterHandler(CustomMessageTypes.GameState, OnGameStateResponse);

                NetClient.RegisterHandler(CustomMessageTypes.GameEnd, OnGameEnd);

                NetClient.Connect(hostAddress, port);

                Debug.Log("Observer");
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
            GameManager.Instance.BackButton();
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

            GameState.ConfirmSelectMonster(message.SelectedMonsterTypeId, message.ActionNumber, message.SubActionNumber);

        }

        private void OnAvailableMoves(NetworkMessage msg)
        {
            Debug.Log("Available moves calculated");
            AvailableMovesResponseMessage message = msg.ReadMessage<AvailableMovesResponseMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            GameState.ConfirmAvailableActions(message.AvailableMoveNodeIds.ToList(), message.AvailableAttackNodeIds.ToList(), message.ActionNumber, message.SubActionNumber);

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

            GameState.ConfirmAttackKillResult((AttackResult)message.AttackResultId, message.AttackingMonsterTypeId, message.DefendingMonsterTypeId, message.ActionNumber, message.SubActionNumber);
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

        private void OnGameStateResponse(NetworkMessage msg)
        {
            GameStateMessage message = msg.ReadMessage<GameStateMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            //Convert json strings to objects
            Dictionary<int, int> friendlyMonsterState = IsHost ? JsonConvert.DeserializeObject<Dictionary<int, int>>(message.HostMonsterState) : JsonConvert.DeserializeObject<Dictionary<int, int>>(message.GuestMonsterState);
            Dictionary<int, int> enemyMonsterState = IsHost ? JsonConvert.DeserializeObject<Dictionary<int, int>>(message.GuestMonsterState) : JsonConvert.DeserializeObject<Dictionary<int, int>>(message.HostMonsterState);


            GameState.UpdateGameState(message.ActionNumber, message.SubActionNumber, friendlyMonsterState, enemyMonsterState);
        }

        private void OnGameStateSyncResponse(NetworkMessage msg)
        {
            GameStateSyncMessage message = msg.ReadMessage<GameStateSyncMessage>();

            message.ActionNumber = AdjustActionNumber(message.ActionNumber);

            //Convert json strings to objects
            Dictionary<int, int> friendlyMonsterState = IsHost ? JsonConvert.DeserializeObject<Dictionary<int, int>>(message.HostMonsterState) : JsonConvert.DeserializeObject<Dictionary<int, int>>(message.GuestMonsterState);
            Dictionary<int, int> enemyMonsterState = IsHost ? JsonConvert.DeserializeObject<Dictionary<int, int>>(message.GuestMonsterState) : JsonConvert.DeserializeObject<Dictionary<int, int>>(message.HostMonsterState);

            List<int> movementPathIds = message.MovementPathIds != null ? JsonConvert.DeserializeObject<List<int>>(message.MovementPathIds) : null;

            IEnumerable<int> availablePushDestinationIds =
                JsonConvert.DeserializeObject<IEnumerable<int>>(message.AvailablePushDestinationIds);            

            GameState.SyncGameState(friendlyMonsterState, enemyMonsterState, movementPathIds, availablePushDestinationIds, message.ActionNumber, message.SubActionNumber, message.SelectedMonsterTypeId, message.SelectedAttackNodeId,  message.DestinationNodeId);
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

        private void OnGameEnd(NetworkMessage msg)
        {
            GameEndMessage message = msg.ReadMessage<GameEndMessage>();
            bool isConclusive = message.IsHostWinner || message.IsGuestWinner;
            bool isWinResult = IsHost ? message.IsHostWinner : message.IsGuestWinner;

            if (!isConclusive || !IsPlayer)
            {
                SceneManager.LoadSceneAsync("endgame");
            }
            else if (isWinResult)
            {
                SceneManager.LoadSceneAsync("wingame");
            }
            else
            {
                SceneManager.LoadSceneAsync("losegame");
            }
        }

        private void OnGameStart(NetworkMessage netMsg)
        {
            GameStartMessage gameStartMessage = netMsg.ReadMessage<GameStartMessage>();

            //Convert json strings to objects
            Dictionary<int, int> friendlyMonsterIds = IsHost? JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.HostMonsters) : JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.GuestMonsters);
            Dictionary<int, int> enemyMonsterIds = IsHost ? JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.GuestMonsters) : JsonConvert.DeserializeObject<Dictionary<int, int>>(gameStartMessage.HostMonsters);

            GameManager gameManager = GameManager.Instance;
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


        public int AdjustActionNumber(int actionNumber)
        {
            if (!IsHost)
            {
                return (actionNumber + 1) % 4 + 1;
            }

            return actionNumber;
        }

    }
}
