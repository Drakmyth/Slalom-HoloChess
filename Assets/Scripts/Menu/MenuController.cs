using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class MenuController : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
            SceneManager.UnloadScene("dejarik");
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        void OnSelected(GameObject gameObject)
        {
            SceneManager.LoadScene("dejarik");
        }
    }
}
