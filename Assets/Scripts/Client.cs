using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MessageModels;
using Assets.Scripts.Monsters;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Client : NetworkBehaviour
    {

        public bool IsHost = false;
        //TODO: Net make this private and abstract necessary functionality out
        public NetworkClient _netClient;
        public string ClientName = "client";
        private ClientGameState _gameState;

        public void Init(string hostAddress, int port)
        {

            if (!IsHost)
            {
                ClientName = "guest";
            }
                        
            try
            {
                _netClient = new NetworkClient();

                _netClient.RegisterHandler(MsgType.Connect, OnConnected);

                _netClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);

                _netClient.RegisterHandler(MsgType.Error, OnError);

                _netClient.RegisterHandler(CustomMessageTypes.GameStart, OnGameStart);

                _netClient.Connect(hostAddress, port);

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

        private void OnConnected(NetworkMessage netMsg)
        {
            Debug.Log("Connected to server");
            //Send(MsgType.Connect, new StringMessage(ClientName));
        }

        private void OnDisconnected(NetworkMessage msg)
        {
            Debug.Log("Disconnected from server");
        }

        private void OnError(NetworkMessage msg)
        {
            Debug.Log("Error connecting with code " + msg.reader.ReadString());
        }

        private void OnGameStart(NetworkMessage netMsg)
        {
            GameStartMessage gameStartMessage = netMsg.ReadMessage<GameStartMessage>();

            //Convert json strings to objects
            List<Monster> friendlyMonsters = IsHost? JsonUtility.FromJson<MonsterListWrapper>(gameStartMessage.HostMonsters).Monsters : JsonUtility.FromJson<MonsterListWrapper>(gameStartMessage.GuestMonsters).Monsters;
            List<Monster> enemyMonsters = IsHost ? JsonUtility.FromJson<MonsterListWrapper>(gameStartMessage.GuestMonsters).Monsters : JsonUtility.FromJson<MonsterListWrapper>(gameStartMessage.HostMonsters).Monsters;

            StartGame(friendlyMonsters, enemyMonsters);


        }

        private IEnumerator StartGame(List<Monster> friendlyMonsters, List<Monster> enemyMonsters)
        {
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync("dejarik");

            while (!loadSceneOperation.isDone)
            {
                print("Loading the Scene");
                yield return null;
            }

            _gameState = SceneManager.GetSceneByName("dejarik").GetRootGameObjects().Single(g => g.name == "GameState").GetComponent<ClientGameState>();

            _gameState.Init(this, friendlyMonsters, enemyMonsters);

            SceneManager.UnloadSceneAsync("lobby");
        }

    }
}
