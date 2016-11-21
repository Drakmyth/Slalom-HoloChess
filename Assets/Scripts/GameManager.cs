using System;
using UnityEngine;
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

        public GameObject ServerPrefab;
        public GameObject ClientPrefab;

        // Use this for initialization
        void Start()
        {
            Instance = this;

            ConnectMenu.SetActive(false);
            HostMenu.SetActive(false);

            DontDestroyOnLoad(gameObject);

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ConnectButton()
        {
            LobbyMenu.SetActive(false);
            ConnectMenu.SetActive(true);
            HostMenu.SetActive(false);
            Debug.Log("Connect");
        }

        public void HostButton()
        {
            try
            {
                Server server = Instantiate(ServerPrefab).GetComponent<Server>();
                server.Init();

                Client client = Instantiate(ClientPrefab).GetComponent<Client>();
                client.IsHost = true;
                client.ClientName = "host";
                client.ConnectToServer("127.0.0.1", 1300);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            LobbyMenu.SetActive(false);
            HostMenu.SetActive(true);
            ConnectMenu.SetActive(false);
            Debug.Log("Host");
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
                Client client = Instantiate(ClientPrefab).GetComponent<Client>();
                client.ConnectToServer(hostAddress, 1300);
                ConnectMenu.SetActive(false);
                //enter dejarik scene
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

            Server server = FindObjectOfType<Server>();
            if (server != null)
            {
                Destroy(server.gameObject);
            }

            Client client = FindObjectOfType<Client>();
            if (client != null)
            {
                Destroy(client.gameObject);
            }

        }

        public void StartGame()
        {
            SceneManager.LoadScene("dejarik");
        }
    }
}
