using Assets.Scripts;
using UnityEngine;

public class LevelSelectionController : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectLevel(int level)
    {
        GameManager.Instance.SinglePlayerButton(level);
    }

}
