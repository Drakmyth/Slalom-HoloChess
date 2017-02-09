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
			Vector3 headPosition = Camera.main.transform.position;
			Vector3 gazeDirection = Vector3.down;
			const float scaleFactor = 25f;
            const float scaledTableOffset = 30f;

			RaycastHit hitInfo;
			float yDistance = transform.localPosition.y;
			if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMapping.PhysicsRaycastMask))
			{
				const float tableOffset = 1.388474f;
				yDistance = headPosition.y - hitInfo.point.y + tableOffset * scaleFactor;
			}

			transform.localScale *= scaleFactor;

			float xDistance = selectedMonster.transform.localPosition.x - transform.localPosition.x;
			float zDistance = selectedMonster.transform.localPosition.z - transform.localPosition.z;

			transform.localPosition = new Vector3(xDistance, -yDistance + scaledTableOffset, zDistance);

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