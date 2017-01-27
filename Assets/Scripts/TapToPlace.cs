using UnityEngine;

namespace Assets.Scripts
{
    public class TapToPlace : MonoBehaviour
    {
        private bool _placing = false;

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
                var headPosition = Camera.main.transform.fposition;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                    30.0f, SpatialMapping.PhysicsRaycastMask))
                {
                    // Move this object to
                    // where the raycast hit the Spatial Mapping mesh.
                    this.transform.position = hitInfo.point + new Vector3(0, .5f, 0); // TODO: account for this's size

                    // Rotate this object's parent object to face the user.
                    // TODO: camera rotation or parent rotation?
                    // TODO: need to offset by 180 degrees?
                    Quaternion toQuat = Camera.main.transform.parent.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    this.transform.rotation = toQuat;
                }
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