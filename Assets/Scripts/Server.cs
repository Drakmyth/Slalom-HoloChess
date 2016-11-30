using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts
{
    public class Server : MonoBehaviour
    {

        public int Port = 1300;
        private bool _isServerStarted;
        private GameState _gameState;
        private Client _hostClient;
        private Client _guestClient;
        
        void Update()
        {
            if (!_isServerStarted)
            {
                return;
            }







        }

        public void StartGame()
        {
            _gameState = new GameState();
        }


        public void Init(string ipAddress = "127.0.0.1")
        {
            DontDestroyOnLoad(gameObject);

            try
            {
                NetworkServer.Listen(ipAddress, Port);

                _isServerStarted = true;
            }
            catch (Exception e)
            {
                Debug.Log("Server error: " + e.Message);
            }
        }

        public void SendToAll(string data, short messageTypeId = 0)
        {
            NetworkServer.SendToAll(messageTypeId, new StringMessage(data));
        }

        public void SendToClient(string data, Client client, short messageTypeId = 0)
        {
            try
            {
                NetworkServer.SendToClient(client.connection.connectionId, messageTypeId, new StringMessage(data));
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void ShutDown()
        {
            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
        }

        private void OnConnected(NetworkMessage netMsg)
        {
            Debug.Log("Client has been connected to host");
            if (NetworkServer.connections.Count == 2)
            {
                NetworkServer.SendToAll(MsgType.Scene, new StringMessage("dejarik"));
            }

        }

    }
}
