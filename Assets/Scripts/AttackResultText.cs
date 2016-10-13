using UnityEngine;
using System.Collections;

public class AttackResultText : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        Vector3 relativePos = mainCamera.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y + 180, lookRotation.eulerAngles.z);
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
