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

            transform.localScale = new Vector3(transform.localScale.x*25f, transform.localScale.y*25f, transform.localScale.z*25f);

            transform.localPosition = new Vector3(-selectedMonster.transform.localPosition.x,
                selectedMonster.transform.localPosition.y - 4, -selectedMonster.transform.localPosition.z);

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