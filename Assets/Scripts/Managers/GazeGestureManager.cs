using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Assets.Scripts
{
    public class GazeGestureManager : MonoBehaviour
    {
        public static GazeGestureManager Instance { get; private set; }

        // Represents the hologram that is currently being gazed at.
        public GameObject FocusedObject { get; private set; }

        GestureRecognizer _recognizer;

        // Use this for initialization
        void Start()
        {
            Instance = this;

            // Set up a GestureRecognizer to detect Select gestures.
            _recognizer = new GestureRecognizer();
            _recognizer.TappedEvent += (source, tapCount, ray) =>
            {
                // Send an OnSelect message to the focused object and its ancestors.
                if (FocusedObject != null)
                {
                    FocusedObject.SendMessage("OnSelected", gameObject);
                }
            };
            _recognizer.StartCapturingGestures();
        }

        // Update is called once per frame
        void Update()
        {
            // Figure out which hologram is focused this frame.
            GameObject oldFocusObject = FocusedObject;

            // Do a raycast into the world based on the user's
            // head position and orientation.
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            FocusedObject = Physics.Raycast(headPosition, gazeDirection, out hitInfo) ? hitInfo.collider.gameObject : null;

            // If the focused object changed this frame,
            // start detecting fresh gestures again.
            if (FocusedObject != oldFocusObject)
            {
                _recognizer.CancelGestures();
                _recognizer.StartCapturingGestures();
            }

            if (FocusedObject != null && oldFocusObject != null && FocusedObject.name != oldFocusObject.name)
            {
                FocusedObject.SendMessage("OnPreviewEnter", gameObject);
            }
        }

        void OnDestroy()
        {
            _recognizer.CancelGestures();
            _recognizer.StopCapturingGestures();
        }
    }
}