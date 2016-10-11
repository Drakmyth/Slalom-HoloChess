using System;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class MenuEmpire : MonoBehaviour
    {


        private float _currentAlpha;
        private float _deltaAlpha;
        private float _rotationSpeed;

        // Use this for initialization
        void Start()
        {
            Material material = gameObject.GetComponent<MeshRenderer>().material;
            _currentAlpha = material.color.a;
            _rotationSpeed = -.01f;
            _deltaAlpha = 0.0001f;
        }

        // Update is called once per frame
        void Update()
        {
            if (isActiveAndEnabled && Math.Abs(_currentAlpha) < .01)
            {
                gameObject.SetActive(false);
                GameObject.Find("MenuTitle").SendMessage("OnEmpireFaded");
                GameObject.Find("MenuTitleAurebesh").SendMessage("OnEmpireFaded");

            }

            Material material = gameObject.GetComponent<MeshRenderer>().material;
            _currentAlpha = material.color.a - _deltaAlpha;
            material.color = new Color(material.color.r, material.color.g, material.color.b, _currentAlpha);
            transform.Rotate(Vector3.down, _rotationSpeed);
            _rotationSpeed = _rotationSpeed * 1.02f;
            _deltaAlpha += .005f * _deltaAlpha;
        }
    }
}
