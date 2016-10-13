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
            _currentAlpha = 1f;
            _rotationSpeed = -.004f;
            _deltaAlpha = 0.002f;
        }

        // Update is called once per frame
        void Update()
        {
            if (isActiveAndEnabled)
            {
                if (_currentAlpha < .01f)
                {
                    _currentAlpha = 0;
                    gameObject.SetActive(false);
                    GameObject.Find("MenuTitle").SendMessage("OnEmpireFaded");
                    GameObject.Find("MenuTitleAurebesh").SendMessage("OnEmpireFaded");
                }
                Material material = gameObject.GetComponent<MeshRenderer>().material;
                material.color = new Color(material.color.r, material.color.g, material.color.b, _currentAlpha);
                transform.Rotate(Vector3.down, _rotationSpeed);
                if (_rotationSpeed > -10)
                {
                    _rotationSpeed = _rotationSpeed*1.04f;
                }
                else if (_currentAlpha > .01f)
                {
                    _currentAlpha = material.color.a - _deltaAlpha;
                    _deltaAlpha += .05f * _deltaAlpha;
                }
            }
        }
    }
}
