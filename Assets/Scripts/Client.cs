using System;
using System.Collections.Generic;
using Assets.Scripts.Monsters;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Client : NetworkClient
    {

        public bool IsHost = false;
        public string ClientName = "client";
        private ClientGameState _gameState;

        public void Init(string hostAddress, int port = 1300)
        {

            IsHost = false;
            ClientName = "guest";
            try
            {

                RegisterHandler(MsgType.Connect, OnConnected);

                RegisterHandler(MsgType.Disconnect, OnDisconnected);

                RegisterHandler(MsgType.Error, OnError);

                Connect(hostAddress, port);
                Debug.Log("Guest");
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
                var localClient = ClientScene.ConnectLocalServer();

                Init(localClient.serverIp, localClient.serverPort);
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
        }

        private void OnDisconnected(NetworkMessage msg)
        {
            Debug.Log("Disconnected from server");
        }

        private void OnError(NetworkMessage msg)
        {
            Debug.Log("Error connecting with code " + msg.reader.ReadString());
        }

        private void OnSceneChange(NetworkMessage netMsg)
        {
            string sceneName = netMsg.reader.ReadString();
            SceneManager.LoadSceneAsync(sceneName);
        }

        private void OnGameStart(NetworkMessage netMsg)
        {
            //TODO: Net get monsters from message
            List<Monster> friendlyMonsters = new List<Monster>();
            List<Monster> enemyMonsters = new List<Monster>();

            _gameState = new ClientGameState(this, friendlyMonsters, enemyMonsters);
        }

    }
}
