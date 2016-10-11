using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MenuBoard : MonoBehaviour
    {
        private float _currentAlpha;
        private float _deltaAlpha;
        private float _rotationSpeed;

        // Use this for initialization
        void Start()
        {
            Material material = gameObject.GetComponent<MeshRenderer>().material;
            _currentAlpha = 0;
            material.color = new Color(material.color.r, material.color.g, material.color.b, _currentAlpha);
            _rotationSpeed = -110;
            _deltaAlpha = 0;

        }

        // Update is called once per frame
        void Update()
        {

            if (_rotationSpeed > -.1 && Mathf.Abs(transform.eulerAngles.x - 300f) < .01)
            {
                return;
            }

            Material material = gameObject.GetComponent<MeshRenderer>().material;
            _currentAlpha = material.color.a + _deltaAlpha;
            material.color = new Color(material.color.r, material.color.g, material.color.b, _currentAlpha);
            transform.Rotate(Vector3.down, _rotationSpeed);
            _rotationSpeed = _rotationSpeed * .995f;

            if (_currentAlpha < 1)
            {
                _deltaAlpha += .000002f;
            }

        }
    }
}
