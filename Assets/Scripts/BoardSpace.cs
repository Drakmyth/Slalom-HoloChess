using System.Collections.Generic;
using System.Linq;
using DejarikLibrary;
using UnityEngine;

public class BoardSpace : MonoBehaviour {

    public Node Node { get; set; }
    public GameObject SelectionIndicatorPrefab;
    private GameObject SelectionIndicatorInstance;
    private Color OriginalColor { get; set; }

	// Use this for initialization
	void Start () {
        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
	    OriginalColor = meshRenderer.material.color;
        Quaternion selectionIndicatorQuaternion = Quaternion.Euler(SelectionIndicatorPrefab.transform.rotation.eulerAngles.x, SelectionIndicatorPrefab.transform.rotation.eulerAngles.y, SelectionIndicatorPrefab.transform.rotation.eulerAngles.z);
        SelectionIndicatorInstance = Instantiate(SelectionIndicatorPrefab,
	        new Vector3(Node.XPosition, SelectionIndicatorPrefab.transform.position.y, Node.YPosition), selectionIndicatorQuaternion) as GameObject;
        SelectionIndicatorInstance.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnAvailableMonsters(IEnumerable<int> availableNodeIds)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (availableNodeIds.Contains(Node.Id))
        {
            meshRenderer.material.color = Color.blue;
        }
        else
        {
            meshRenderer.material.color = OriginalColor;
        }
    }

    void OnAvailableAttacks(IEnumerable<int> availableNodeIds)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (availableNodeIds.Contains(Node.Id))
        {
            meshRenderer.material.color = Color.red;
        }
        else if (meshRenderer.material.color == Color.red)
        {
            meshRenderer.material.color = OriginalColor;
        }
    }

    void OnAvailableMoves(IEnumerable<int> availableNodeIds)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (availableNodeIds.Contains(Node.Id))
        {
            meshRenderer.material.color = Color.green;
        }
        else if (meshRenderer.material.color == Color.green)
        {
            meshRenderer.material.color = OriginalColor;
        }
    }

    void OnMonsterSelected(int nodeId)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (nodeId == Node.Id)
        {
            SelectionIndicatorInstance.SetActive(true);
        }
        else
        {
            SelectionIndicatorInstance.SetActive(false);
        }
    }


    void OnClearHighlighting()
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        meshRenderer.material.color = OriginalColor;
    }

    void OnSelected(GameObject gameStateObject)
    {
        gameStateObject.SendMessage("OnSpaceSelected", Node.Id);
    }
}
