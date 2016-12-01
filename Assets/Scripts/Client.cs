using System;
using System.Collections.Generic;
using Assets.Scripts.MessageModels;
using Assets.Scripts.Monsters;
using UnityEngine;
using UnityEngine.Networking;

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
            List<Monster> friendlyMonsters = IsHost? JsonUtility.FromJson<List<Monster>>(gameStartMessage.HostMonsters) : JsonUtility.FromJson<List<Monster>>(gameStartMessage.GuestMonsters);
            List<Monster> enemyMonsters = IsHost ? JsonUtility.FromJson<List<Monster>>(gameStartMessage.GuestMonsters) : JsonUtility.FromJson<List<Monster>>(gameStartMessage.HostMonsters);

            _gameState = new ClientGameState(this, friendlyMonsters, enemyMonsters);
        }

    }
}
