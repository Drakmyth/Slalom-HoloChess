using UnityEngine;
using UnityEngine.UI;

public class NumPad : MonoBehaviour
{

    public InputField InputField;
    public Text Preview;

    public string Text;

    private Vector3 _initialScale;
    private bool _hasChanged = false;

	// Use this for initialization
	void Start ()
	{
	    _initialScale = transform.localScale;
        SetVisible(false);
        Text = "";
	}
	
	// Update is called once per frame
	void Update () {
	    if (_hasChanged)
	    {
            Preview.text = Text;
	        _hasChanged = false;

	    }
	}

    public void Type(string input)
    {
        Text += input;
        _hasChanged = true;
    }

    public void Delete()
    {
        if (Text.Length > 0)
        {
            Text = Text.Substring(0, Text.Length - 1);
            _hasChanged = true;
        }
    }

    public void Clear()
    {
        Text = "";
        _hasChanged = true;
    }

    public void Done()
    {
        InputField.text = Text;
        SetVisible(false);
    }

    public void SetVisible(bool isVisible)
    {
        transform.localScale = isVisible? _initialScale : Vector3.zero;
    }
}
