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

        public int port = 1300;

        private List<ServerClient> clients;
        private List<ServerClient> disconnectList;

        private TcpListener server;
        private bool isServerStarted;

        public void Init()
        {
            DontDestroyOnLoad(gameObject);
            clients = new List<ServerClient>();
            disconnectList = new List<ServerClient>();

            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                StartListening();
                isServerStarted = true;
            }
            catch (Exception e)
            {
                Debug.Log("Socket error: " + e.Message);
            }
        }

        void Update()
        {
            if (!isServerStarted)
            {
                return;
            }

            foreach (ServerClient client in clients)
            {
                if (!IsConnected(client.tcpClient))
                {
                    client.tcpClient.Close();
                    disconnectList.Add(client);
                    continue;
                }
                else
                {
                    NetworkStream stream = client.tcpClient.GetStream();
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

                for (int i = 0; i < disconnectList.Count; i++)
                {
                    clients.Remove(disconnectList[i]);
                    disconnectList.RemoveAt(i);
                }
            }
        }


        private void StartListening()
        {
            server.BeginAcceptTcpClient(AcceptTcpClient, server);
        }

        private void AcceptTcpClient(IAsyncResult result)
        {
            TcpListener listener = (TcpListener) result.AsyncState;

            ServerClient client = new ServerClient(listener.EndAcceptTcpClient(result));

            clients.Add(client);

            StartListening();
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
                try
                {
                    StreamWriter writer = new StreamWriter(client.tcpClient.GetStream());
                    writer.WriteLine(data);
                    writer.Flush();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }

        }

        private void OnIncomingData(ServerClient client, string data)
        {
            Debug.Log(data);
        }
    }

    public class ServerClient
    {
        public string clientName;
        public TcpClient tcpClient;

        public ServerClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }
    }
}
