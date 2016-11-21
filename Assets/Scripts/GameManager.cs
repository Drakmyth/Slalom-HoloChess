using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour {

        public static GameManager Instance { get; set; }

        public GameObject LobbyMenu;
        public GameObject ConnectMenu;
        public GameObject HostMenu;

        public GameObject ServerPrefab;
        public GameObject ClientPrefab;

        // Use this for initialization
        void Start ()
        {
            Instance = this;

            ConnectMenu.SetActive(false);
            HostMenu.SetActive(false);

            DontDestroyOnLoad(gameObject);

        }
	
        // Update is called once per frame
        void Update () {
	
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
            LobbyMenu.SetActive(false);
            HostMenu.SetActive(true);
            ConnectMenu.SetActive(false);
            Debug.Log("Host");
        }

        public void ConnectToServerButton()
        {
            
        }

        public void BackButton()
        {
            LobbyMenu.SetActive(true);
            ConnectMenu.SetActive(false);
            HostMenu.SetActive(false);
        }
    }
}
