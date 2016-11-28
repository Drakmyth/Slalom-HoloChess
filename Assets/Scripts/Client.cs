using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Client : NetworkClient
    {

        public bool IsHost = false;
        public string ClientName = "client";

        public void Init(string hostAddress, int port = 1300)
        {

            IsHost = false;
            try
            {

                RegisterHandler(MsgType.Connect, OnConnected);

                RegisterHandler(MsgType.Disconnect, OnDisconnected);

                RegisterHandler(MsgType.Error, OnError);

                Connect(hostAddress, port);
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

    }
}
