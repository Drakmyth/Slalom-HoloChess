using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public abstract class Monster : MonoBehaviour
    {
        public abstract int AttackRating { get; }
        public abstract int DefenseRating { get; }
        public abstract int MovementRating { get; }

        public Node CurrentNode { get; set; }

        private bool _isAlive;

        private const float BaseFrameMovement = .002f;

        private GameObject _battleSmokeInstance;

        void Start()
        {
            _isAlive = true;
        }

        void Update()
        {
            if (!_isAlive)
            {
                if (gameObject.transform.position.y < -.1)
                {
                    if (_battleSmokeInstance != null)
                    {
                        Destroy(_battleSmokeInstance);
                    }

                    Destroy(gameObject);
                }

                Vector3 updatedPosition = new Vector3(transform.position.x, transform.position.y - BaseFrameMovement, transform.position.z);

                transform.position = updatedPosition;
            }

        }

        void OnLoseBattle(GameObject battleSmokeInstance)
        {
            _battleSmokeInstance = battleSmokeInstance;
            _isAlive = false;
        }
    }
}
