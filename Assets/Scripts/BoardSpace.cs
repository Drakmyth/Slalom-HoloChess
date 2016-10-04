using UnityEngine;

public class BoardSpace : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {
        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.cyan;
    }
}
