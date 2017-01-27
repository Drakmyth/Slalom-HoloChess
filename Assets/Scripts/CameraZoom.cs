using UnityEngine;

namespace Assets.Scripts
{
    public class CameraZoom : MonoBehaviour
    {
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Vector3 _originalScale;
        private bool _isZoomed;

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
            
            var parent = Camera.main.transform.parent;

            // update camera location & rotation
            _originalPosition = parent.localPosition;
            _originalRotation = parent.localRotation;
            _originalScale = parent.localScale;

			parent.localScale = new Vector3(parent.localScale.x/25f, parent.localScale.y / 25f, parent.localScale.z / 25f);
            parent.transform.localPosition = new Vector3(selectedMonster.transform.localPosition.x, 
                selectedMonster.transform.localPosition.y + .1f, selectedMonster.transform.localPosition.z);

            _isZoomed = true;
        }

        private void ZoomOut()
        {
            if (!_isZoomed) return;

			var parent = Camera.main.transform.parent;

            // update camera location
            parent.localScale = _originalScale;
            parent.transform.localPosition = _originalPosition;
            parent.transform.localRotation = _originalRotation;

            _isZoomed = false;
        }
    }
}