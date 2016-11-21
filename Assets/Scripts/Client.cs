using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts
{
    public class Client : MonoBehaviour
    {
        public bool IsHost = false;
        public string ClientName = "client";

        private bool _isSocketReady;
        private TcpClient _socket;
        private NetworkStream _stream;
        private StreamWriter _writer;
        private StreamReader _reader;

        private List<GameClient> _players = new List<GameClient>();

        public bool ConnectToServer(string host, int port)
        {
            if (_isSocketReady)
            {
                return false;
            }

            try
            {
                _socket = new TcpClient(host, port);
                _stream = _socket.GetStream();
                _writer = new StreamWriter(_stream);
                _reader = new StreamReader(_stream);

                _isSocketReady = true;

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            return _isSocketReady;

        }

        public void Send(string data)
        {
            if (!_isSocketReady)
            {
                return;
            }

            _writer.WriteLine(data);
            _writer.Flush();
        }

        //TODO: MessageModels instead of string parsing
        private void OnIncomingData(string data)
        {
            string[] parsedData = data.Split('|');

            switch (parsedData[0])
            {
                case "srv.connect":
                    Send("cli.connect|" + ClientName + "|" + ((IsHost)?1:0));
                    break;
                case "srv.playerConnected":
                    break;
            }



            Debug.Log(data);
        }

        private void UserConnected(string clientName)
        {
            GameClient gameClient = new GameClient {Name = clientName };

            _players.Add(gameClient);

            if (_players.Count == 2)
            {
                GameManager.Instance.StartGame();
            }
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
            if (!_isSocketReady)
            {
                return;
            }

            _writer.Close();
            _reader.Close();
            _socket.Close();

            _isSocketReady = false;
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (_isSocketReady && _stream.DataAvailable)
            {
                string data = _reader.ReadLine();
                if (data != null)
                {
                    OnIncomingData(data);
                }
            }
        }

    }

    public class GameClient
    {
        public string Name;
        public bool IsHost;
    }
}
