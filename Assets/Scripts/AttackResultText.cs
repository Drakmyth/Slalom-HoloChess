using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class AttackResultText : MonoBehaviour  {

        public Vector3 LerpDestination { get; set; }
        private float _currentLerp;
        private const float LerpRate = .2f;
        private bool _isReset = false;

        // Use this for initialization
        void Start () {
            _currentLerp = 0;
            LerpDestination = transform.localPosition + Vector3.up * 1.6f;
            RotateToCamera();
        }
	
        // Update is called once per frame
        void Update () {
            if (isActiveAndEnabled)
            {
                var text = GetComponent<TextMesh>();

                if (_isReset)
                {
                    _isReset = false;
                    _currentLerp = 0;
                    LerpDestination = transform.localPosition + Vector3.up * 1.6f;
                    RotateToCamera();
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
                }

                _currentLerp += Time.deltaTime*LerpRate;

                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a * .98f - .01f);

                transform.localPosition = Vector3.Lerp(transform.localPosition, LerpDestination, _currentLerp);

                RotateToCamera();

                if (_currentLerp > 1 || text.color.a <= 0)
                {
                    _isReset = true;
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
            transform.localRotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y + 180, lookRotation.eulerAngles.z);

        }

    }
}
