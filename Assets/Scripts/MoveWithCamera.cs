using UnityEngine;

public class MoveWithCamera : MonoBehaviour
{

    private float _previousXPosition;
    private float _previousYPosition;
    private float _previousZPosition;

    // Use this for initialization
    void Start ()
    {
        _previousXPosition = Camera.main.transform.localPosition.x;
        _previousYPosition = Camera.main.transform.localPosition.y;
        _previousZPosition = Camera.main.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update () {

        float currentXPosition = Camera.main.transform.localPosition.x;
        float currentYPosition = Camera.main.transform.localPosition.y;
        float currentZPosition = Camera.main.transform.localPosition.z;

        Vector3 cameraDelta = new Vector3(currentXPosition - _previousXPosition, currentYPosition - _previousYPosition, currentZPosition - _previousZPosition);

        gameObject.transform.localPosition = gameObject.transform.localPosition + cameraDelta;

        _previousXPosition = Camera.main.transform.localPosition.x;
        _previousYPosition = Camera.main.transform.localPosition.y;
        _previousZPosition = Camera.main.transform.localPosition.z;

    }
}
