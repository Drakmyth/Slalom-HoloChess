using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MenuBoard : MonoBehaviour
    {
        private float _rotationSpeed;

        // Use this for initialization
        void Start()
        {
            _rotationSpeed = -60;
        }

        // Update is called once per frame
        void Update()
        {

            if (_rotationSpeed > -.1 && (Mathf.Abs(transform.eulerAngles.x % 30) - 30) < .05)
            {
                return;
            }

            transform.Rotate(Vector3.down, _rotationSpeed);
            _rotationSpeed = _rotationSpeed * .991f;

        }
    }
}
