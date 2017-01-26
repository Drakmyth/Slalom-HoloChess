using System.Collections;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionController : MonoBehaviour {

    public GameObject EasyButton;
    public GameObject HardButton;
    public Text Text;

    // Use this for initialization
    void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void SelectLevel(int level)
    {
        EasyButton.SetActive(false);
        HardButton.SetActive(false);

        // display flashing loading text
        Text.text = "Loading";
        Text.transform.localPosition = new Vector3(Text.transform.localPosition.x, .1f, Text.transform.localPosition.z);
        StartCoroutine(BlinkText());

        GameManager.Instance.SinglePlayerButton(level);        
    }

    public IEnumerator BlinkText()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);

            if (Text.text == "Loading")
            {
                Text.text = "Loading.";
            }
            else if (Text.text == "Loading.")
            {
                Text.text = "Loading..";
            }
            else if (Text.text == "Loading..")
            {
                Text.text = "Loading...";
            }
            else if (Text.text == "Loading...")
            {
                Text.text = "Loading";
            }
        }
    }
}
