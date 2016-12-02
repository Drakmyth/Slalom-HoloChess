using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MessageModels;
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
        private ClientGameState _gameState;

        public void Init(string hostAddress, int port)
        {
            DontDestroyOnLoad(gameObject);

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
            List<int> friendlyMonsterIds = IsHost? JsonConvert.DeserializeObject<List<int>>(gameStartMessage.HostMonsters) : JsonConvert.DeserializeObject<List<int>>(gameStartMessage.GuestMonsters);
            List<int> enemyMonsterIds = IsHost ? JsonConvert.DeserializeObject<List<int>>(gameStartMessage.GuestMonsters) : JsonConvert.DeserializeObject<List<int>>(gameStartMessage.HostMonsters);

            IEnumerator enumerator = LoadGameScene();

            while (enumerator == null || enumerator.Current == null)
            {

            }

            _gameState = SceneManager.GetSceneByName("dejarik").GetRootGameObjects().Single(g => g.name == "GameState").GetComponent<ClientGameState>();

            _gameState.Init(this, friendlyMonsterIds, enemyMonsterIds);

            SceneManager.UnloadSceneAsync("lobby");


        }

        private IEnumerator LoadGameScene()
        {
            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync("dejarik");

            while (!loadSceneOperation.isDone)
            {
                Debug.Log("Loading the Game Scene");
                yield return null;
            }
        }

    }
}
