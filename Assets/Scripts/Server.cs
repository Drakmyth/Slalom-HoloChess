using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts
{
    public class Server : MonoBehaviour
    {

        public int Port = 1300;

        private List<ServerClient> _clients;
        private List<ServerClient> _disconnectList;

        private TcpListener _server;
        private bool _isServerStarted;

        public void Init()
        {
            DontDestroyOnLoad(gameObject);
            _clients = new List<ServerClient>();
            _disconnectList = new List<ServerClient>();

            try
            {
                _server = new TcpListener(IPAddress.Any, Port);
                _server.Start();

                StartListening();
                _isServerStarted = true;
            }
            catch (Exception e)
            {
                Debug.Log("Socket error: " + e.Message);
            }
        }

        void Update()
        {
            if (!_isServerStarted)
            {
                return;
            }

            foreach (ServerClient client in _clients)
            {
                if (!IsConnected(client.TcpClient))
                {
                    client.TcpClient.Close();
                    _disconnectList.Add(client);
                    continue;
                }
                else
                {
                    NetworkStream stream = client.TcpClient.GetStream();
                    if (stream.DataAvailable)
                    {
                        StreamReader reader = new StreamReader(stream, true);
                        string data = reader.ReadLine();

                        if (data != null)
                        {
                            OnIncomingData(client, data);
                        }
                    }
                }

                for (int i = 0; i < _disconnectList.Count; i++)
                {
                    _clients.Remove(_disconnectList[i]);
                    _disconnectList.RemoveAt(i);
                }
            }
        }


        private void StartListening()
        {
            _server.BeginAcceptTcpClient(AcceptTcpClient, _server);
        }

        private void AcceptTcpClient(IAsyncResult result)
        {
            TcpListener listener = (TcpListener) result.AsyncState;

            ServerClient client = new ServerClient(listener.EndAcceptTcpClient(result));

            _clients.Add(client);

            StartListening();

            Debug.Log("User has connected");

            Broadcast("User has connected", _clients[_clients.Count - 1]);
        }


        private bool IsConnected(TcpClient tcpClient)
        {
            try
            {
                if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
                {
                    if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        return tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) != 0;
                    }

                    return true;

                }

                return false;

            }
            catch
            {
                return false;
            }
        }


        private void Broadcast(string data, List<ServerClient> clients)
        {
            foreach (ServerClient client in clients)
            {
                Broadcast(data, client);
            }

        }

        private void Broadcast(string data, ServerClient client)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.TcpClient.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        //TODO: MessageModels instead of string parsing
        private void OnIncomingData(ServerClient client, string data)
        {
            string[] parsedData = data.Split('|');

            switch (parsedData[0])
            {
                case "cli.connect":
                    Broadcast("srv.playerConnected|" + client.ClientName + " has connected", _clients);
                    break;
            }



            Debug.Log(data);
        }

    }

    public class ServerClient
    {
        public string ClientName;
        public TcpClient TcpClient;

        public ServerClient(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }
    }
}
