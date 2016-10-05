using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardSpace : MonoBehaviour {

    public int NodeId { get; set; }
    private Color OriginalColor { get; set; }
    private float OriginalColorAlpha { get; set; }

	// Use this for initialization
	void Start () {
        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
	    OriginalColor = meshRenderer.material.color;
	    OriginalColorAlpha = OriginalColor.a;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnAvailableMonsters(IEnumerable<int> availableNodeIds)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (availableNodeIds.Contains(NodeId))
        {
            meshRenderer.material.color = Color.cyan;
        }
        else
        {
            meshRenderer.material.color = OriginalColor;
        }
    }

    void OnAvailableAttacks(IEnumerable<int> availableNodeIds)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (availableNodeIds.Contains(NodeId))
        {
            meshRenderer.material.color = Color.red;
        }
        else
        {
            meshRenderer.material.color = OriginalColor;
        }
    }

    void OnAvailableMoves(IEnumerable<int> availableNodeIds)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (availableNodeIds.Contains(NodeId))
        {
            meshRenderer.material.color = Color.green;
        }
        else
        {
            meshRenderer.material.color = OriginalColor;
        }
    }

    void OnMonsterSelected(int nodeId)
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        if (nodeId == NodeId)
        {
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.b, meshRenderer.material.color.g, 200f);
        }
        else
        {
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.b, meshRenderer.material.color.g, OriginalColorAlpha);
        }
    }


    void OnClearHighlighting()
    {

        var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        meshRenderer.material.color = OriginalColor;
    }
}
