using UnityEngine;
using System.Collections;

public class AttackResultText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var mainCamera = GameObject.Find("Main Camera");
        Vector3 relativePos = mainCamera.transform.position - transform.position;
	    transform.rotation = Quaternion.LookRotation(relativePos);
	}

    void displayResult()
    {
        transform.Translate(Vector3.up * Time.deltaTime);
    }

    void hideResult()
    {
        transform.Translate(Vector3.down * Time.deltaTime);
    }
}
