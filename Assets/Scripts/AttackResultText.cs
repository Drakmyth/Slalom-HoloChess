using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AttackResultText : MonoBehaviour  {

        public Vector3 LerpDestination { get; set; }
        private float _currentLerp;
        private const float LerpRate = .05f;

        // Use this for initialization
        void Start () {
            _currentLerp = 0;
            LerpDestination = transform.position + Vector3.up * .4f;
            RotateToCamera();
        }
	
        // Update is called once per frame
        void Update () {
            if (isActiveAndEnabled)
            {

                _currentLerp += Time.deltaTime*LerpRate;

                var text = GetComponent<TextMesh>();
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a * .98f - .01f);

                transform.position = Vector3.Lerp(transform.position, LerpDestination, _currentLerp);

                RotateToCamera();

                if (_currentLerp > 1 || text.color.a <= 0)
                {
                    gameObject.SetActive(false);
                }

            }
        }

        private void RotateToCamera()
        {
            GameObject mainCamera =
                GameObject.FindGameObjectsWithTag("MainCamera").Single(o => o.GetComponent<Camera>().enabled);
            Vector3 relativePos = mainCamera.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y + 180, lookRotation.eulerAngles.z);

        }

    }
}
