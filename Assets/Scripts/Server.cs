using System;
using System.Linq;
using Assets.Scripts.MessageModels;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Assets.Scripts
{
    public class Server : NetworkBehaviour
    {

        public int Port = 1300;
        public bool IsServerStarted;
        private GameState _gameState;
        private Client _hostClient;
        private Client _guestClient;
        
        void Update()
        {
            if (!IsServerStarted)
            {
                return;
            }
        }

        public void Init(string ipAddress = "127.0.0.1")
        {
            DontDestroyOnLoad(gameObject);

            try
            {

                NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);

                NetworkServer.Listen(ipAddress, Port);

                IsServerStarted = true;
            }
            catch (Exception e)
            {
                Debug.Log("Server error: " + e.Message);
            }
        }

        public void SendToAll(GameStateMessage msg)
        {
            NetworkServer.SendToAll(msg.MessageTypeId, msg);
        }

        public void SendToClient(string data, Client client, short messageTypeId = 0)
        {
            try
            {
                NetworkServer.SendToClient(client.NetClient.connection.connectionId, messageTypeId, new StringMessage(data));
            }
            catch (NullReferenceException npe)
            {
                Debug.Log("Connection does not exist for " + client.ClientName);
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

        private void OnClientConnected(NetworkMessage netMsg)
        {
            Debug.Log("Client has been connected to host");
            if (NetworkServer.connections.Count(c => c != null) == 1) //TODO: Net this should be 2
            {
                _gameState = gameObject.AddComponent<GameState>();
            }

        }

    }
}
