using UnityEngine;

namespace Assets.Scripts
{
    public class AttackResultText : MonoBehaviour {

        private Vector3 LerpDestination { get; set; }
        private const float LerpRate = .2f;

        // Use this for initialization
        void Start () {
            gameObject.SetActive(false);	
        }
	
        // Update is called once per frame
        void Update () {
            if (isActiveAndEnabled)
            {
                RotateToCamera();

                transform.position = Vector3.Lerp(transform.position, LerpDestination, Time.deltaTime*LerpRate);

            }
        }

        void OnActive(Vector3 initialPosition)
        {
            gameObject.SetActive(true);
            LerpDestination = initialPosition + Vector3.up * .4f;

        }

        private void RotateToCamera()
        {
            GameObject mainCamera = GameObject.Find("Main Camera");
            Vector3 relativePos = mainCamera.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y + 180, lookRotation.eulerAngles.z);

        }

    }
}
