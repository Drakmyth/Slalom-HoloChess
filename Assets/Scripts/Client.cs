using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts
{
    public class Client : MonoBehaviour
    {

        private bool isSocketReady;
        private TcpClient socket;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        public bool ConnectToServer(string host, int port)
        {
            if (isSocketReady)
            {
                return false;
            }

            try
            {
                socket = new TcpClient(host, port);
                stream = socket.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);

                isSocketReady = true;

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            return isSocketReady;

        }

        public void Send(string data)
        {
            if (!isSocketReady)
            {
                return;
            }

            writer.WriteLine(data);
            writer.Flush();
        }

        private void OnIncomingData(string data)
        {
            Debug.Log(data);
        }

        private void OnApplicationQuit()
        {
            CloseSocket();
        }

        private void OnDisable()
        {
            CloseSocket();
        }

        private void CloseSocket()
        {
            if (!isSocketReady)
            {
                return;
            }

            writer.Close();
            reader.Close();
            socket.Close();

            isSocketReady = false;
        }

        void Update()
        {
            if (isSocketReady && stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                {
                    OnIncomingData(data);
                }
            }
        }

    }

    public class GameClient
    {
        public string name;
        public bool isHost;
    }
}
