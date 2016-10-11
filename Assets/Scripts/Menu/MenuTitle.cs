using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class MenuTitle : MonoBehaviour
    {

        private float _textAlpha;
        private bool _isAppearing;

        // Use this for initialization
        void Start ()
        {
            var color = gameObject.GetComponent<Text>().color;

            _textAlpha = 0;

            gameObject.GetComponent<Text>().color = new Color(color.r, color.g, color.b, _textAlpha);


            _isAppearing = false;

        }

        // Update is called once per frame
        void Update () {

            if (_isAppearing)
            {
                var color = gameObject.GetComponent<Text>().color;

                _textAlpha = color.a + .005f;

                if (_textAlpha >= 1)
                {
                    _textAlpha = 1f;

                    _isAppearing = false;
                }

                gameObject.GetComponent<Text>().color = new Color(color.r, color.g, color.b, _textAlpha);

            }
        }

        void OnEmpireFaded ()
        {
            _isAppearing = true;
        }
    }
}
