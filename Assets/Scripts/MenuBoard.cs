using UnityEngine;

namespace Assets.Scripts
{
    public class MenuBoard : MonoBehaviour
    {

        private float _currentAlpha;

        // Use this for initialization
        void Start () {
            Material material = gameObject.GetComponent<Material>();
            _currentAlpha = material.color.a;


        }

        // Update is called once per frame
        void Update ()
        {
            Material material = gameObject.GetComponent<Material>();
            _currentAlpha = material.color.a - .001f;
            material.color = new Color(material.color.r, material.color.g, material.color.b, _currentAlpha);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + 5, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
