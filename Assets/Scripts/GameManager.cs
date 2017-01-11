using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.AI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; set; }

        public GameObject LobbyMenu;
        public GameObject ConnectMenu;
        public GameObject HostMenu;
        public GameObject Tutorial;
        public GameObject MicropohoneGameObject;

        // monsterTypeIds keyed to initial position nodeIds
        public Dictionary<int, int> FriendlyMonsterInitialNodeIds;
        public Dictionary<int, int> EnemyMonsterInitialNodeIds;

        public Client Client;

        private bool _isScrolling; // We'll use this for debugging
        private float _rotation;   // Default 55deg, but read in from canvas
        public Server Server;

        private bool _isHosting;

        void Start()
        {

            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            ConnectMenu.SetActive(false);
            HostMenu.SetActive(false);
            Tutorial.SetActive(false);
            MicropohoneGameObject.SetActive(false);

            _isHosting = false;
            _isScrolling = false;

        }

        void Update()
        {
            if (Client != null && Client.NetClient.connection != null)
            {
                //Debug.Log("Address: " + Client.connection.address);
            }

            if (_isHosting)
            {
                Debug.Log("Port: " + NetworkServer.listenPort);
                Debug.Log("Connections: " + NetworkServer.connections.Count);
            }

            // If we are scrolling, perform update action
            if (_isScrolling && Tutorial != null)
            {
                // Get the current transform position of the panel
                Vector3 _currentUIPosition = Tutorial.gameObject.transform.position;

                // Increment the Y value of the panel 
                Vector3 _incrementYPosition =
                 new Vector3(_currentUIPosition.x,
                             _currentUIPosition.y + .001f * Mathf.Sin(Mathf.Deg2Rad * _rotation),
                             _currentUIPosition.z + .001f * Mathf.Cos(Mathf.Deg2Rad * _rotation));

                // Change the transform position to the new one
                Tutorial.gameObject.transform.position = _incrementYPosition;
            }

        }

        public void MultiplayerButton()
        {
            LobbyMenu.SetActive(false);
            ConnectMenu.SetActive(true);
            HostMenu.SetActive(false);
            Debug.Log("Multiplayer");
        }

        public void HostButton()
        {

            try
            {
                Server = gameObject.AddComponent<Server>();
                string ipAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
                Server.Init(ipAddress);

                Client = gameObject.AddComponent<Client>();
                Client.InitHost(Server.IpAddress);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            LobbyMenu.SetActive(false);
            HostMenu.SetActive(true);

            HostMenu.transform.FindChild("IpAddress").GetComponent<Text>().text = Server.IpAddress;

            ConnectMenu.SetActive(false);
            Debug.Log("Host");
        }

        public void DisplayGameRules()
        {
            LobbyMenu.SetActive(false);
            Tutorial.SetActive(true);
            StartCoroutine(ActivateMicrophone());
            
            // display logic
            _isScrolling = true;
            _rotation = Tutorial.gameObject.transform.eulerAngles.x;
        }

        private IEnumerator ActivateMicrophone()
        {
            yield return new WaitForSeconds(5);
            MicropohoneGameObject.SetActive(true);
        }

        public void OpenDifficultySelector()
        {
            SceneManager.LoadScene("selectlevel");
        }

        // The Game Difficulty:
        // 0= EASY
        // 1= HARD
        public void SinglePlayerButton(int difficulty)
        {
            try
            {
                Server = gameObject.AddComponent<Server>();
                Server.InitSinglePlayer();

                if (difficulty == 0)
                {
                    // easy has been selected, use dumb AI
                    GonkDroidAI gonkDroidAI = Server.gameObject.AddComponent<GonkDroidAI>();
                    gonkDroidAI.Init(Server.IpAddress);
                }
                else
                {
                    // hard has been selected. use intelligent AI
                    // TODO : Wesley to add intelliegent AI to replace GonkDroidAI
                    GonkDroidAI gonkDroidAI = Server.gameObject.AddComponent<GonkDroidAI>();
                    gonkDroidAI.Init(Server.IpAddress);
                }

                Client = gameObject.AddComponent<Client>();
                Client.Init(Server.IpAddress, 1300);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            Debug.Log("Host");
        }

        public void ResetGameManager()
        {
            if (Server != null)
            {
                var ai = Server.gameObject.GetComponent<GonkDroidAI>();
                if (ai != null)
                {
                    ai.NetClient.Shutdown();
                    Destroy(ai);
                }

                Destroy(Server);
            }

            if (Client == null) return;

            if (Client.NetClient != null)
            {
                Client.NetClient.Shutdown();
            }        
            Destroy(Client);

            _isScrolling = false;
        }

        public void ConnectToServerButton()
        {
            string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;

            if (string.IsNullOrEmpty(hostAddress))
            {
                hostAddress = "127.0.0.1";
            }

            try
            {
                Client = gameObject.AddComponent<Client>();
                Client.Init(hostAddress, 1300);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void BackButton()
        {
            LobbyMenu.SetActive(true);
            ConnectMenu.SetActive(false);
            HostMenu.SetActive(false);

            if (Client != null && Client.NetClient.isConnected)
            {
                Client.NetClient.Disconnect();
                Client = null;
            }

            if (_isHosting)
            {
                _isHosting = false;
                Server.ShutDown();
                Server = null;
            }

        }

        public void StartGame()
        {
            SceneManager.LoadScene("dejarik");
        }

    }
}
