using UnityEngine;

namespace Assets.Scripts
{
    public class TapToPlace : MonoBehaviour
    {
        private bool _placing = false;

        public static TapToPlace Instance;

        void Start()
        {
            Instance = this;
        }

        // Called by GazeGestureManager when the user performs a Select gesture
        void OnSelected()
        {
            // On each Select gesture, toggle whether the user is in _placing mode.
            UpdatePlacingStatus();
        }

        // Update is called once per frame
        void Update()
        {
            // If the user is in _placing mode,
            // update the placement to match the user's gaze.

            if (_placing)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                Vector3 headPosition = Camera.main.transform.position;
                Vector3 gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
	            if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMapping.PhysicsRaycastMask)) return;

	            // Move this object to
	            // where the raycast hit the Spatial Mapping mesh.
	            float height = GetComponent<Renderer>().bounds.size.y;
	            transform.position = hitInfo.point + new Vector3(0, height, 0);
            }
        }

        //Allow for explicit or implicit toggle of placing status
        public void UpdatePlacingStatus(bool? placingStatus = null)
        {
            if (!placingStatus.HasValue)
            {
                placingStatus = !_placing;
            }

            _placing = placingStatus.Value;
            
            // If the user is in _placing mode, display the spatial mapping mesh.
            // If the user is not in _placing mode, hide the spatial mapping mesh.
            SpatialMapping.Instance.DrawVisualMeshes = _placing;
        }
    }
}