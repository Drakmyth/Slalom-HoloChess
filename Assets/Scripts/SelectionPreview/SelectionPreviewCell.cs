using Assets.Scripts.Monsters;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPreviewCell : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    gameObject.GetComponent<Text>().text = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnUpdate(string text)
    {
        gameObject.GetComponent<Text>().text = text;
    }

    void OnClear()
    {
        gameObject.GetComponent<Text>().text = "";
    }
}
