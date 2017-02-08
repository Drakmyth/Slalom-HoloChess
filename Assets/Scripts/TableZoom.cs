using UnityEngine;

namespace Assets.Scripts
{
    public class TableZoom : MonoBehaviour
    {
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Vector3 _originalScale;
        private bool _isZoomed;

        public static TableZoom Instance;

        void Start()
        {
            Instance = this;
        }

        private ClientGameState GameState
        {
            get { return GameManager.Instance.Client.GameState; }
        }

        public void Zoom()
        {
            if (_isZoomed)
            {
                ZoomOut();
            }
            else
            {
                ZoomIn();
            }
        }

        private void ZoomIn()
        {
            var selectedMonster = GameState.GetSelectedMonsterPrefab();
            if (_isZoomed || selectedMonster == null) return;
            
            // update camera location & rotation
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;
            _originalScale = transform.localScale;

			// Do a raycast into the world that will only hit the Spatial Mapping mesh.
			var headPosition = Camera.main.transform.position;
			var gazeDirection = Vector3.down;

			RaycastHit hitInfo;
			float yDistance = transform.localPosition.y;
			if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMapping.PhysicsRaycastMask))
			{
				// Move this object to
				// where the raycast hit the Spatial Mapping mesh.
				yDistance = hitInfo.point.y - headPosition.y; // TODO: account for this's size
			}

			transform.localScale = new Vector3(transform.localScale.x*25f, transform.localScale.y*25f, transform.localScale.z*25f);

			var xDistance = selectedMonster.transform.localPosition.x - transform.localPosition.x;
			var zDistance = selectedMonster.transform.localPosition.z - transform.localPosition.z;

			transform.localPosition = new Vector3(xDistance, yDistance, zDistance);

            _isZoomed = true;
        }

        private void ZoomOut()
        {
            if (!_isZoomed) return;

            // update camera location
            transform.localScale = _originalScale;
            transform.localPosition = _originalPosition;
            transform.localRotation = _originalRotation;

            _isZoomed = false;
        }
    }
}