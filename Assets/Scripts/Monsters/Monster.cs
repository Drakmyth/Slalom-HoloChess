using System.Collections.Generic;
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

        private List<Node> MovementNodes { get; set; }

        private float _remainingX;

        private float _remainingZ;

        private bool _isAlive;


        private const float HorizontalMovementPerSecond = .5f;

        private const float VerticalMovementPerSecond = .05f;

        private GameObject _battleSmokeInstance;

        void Start()
        {
            _remainingX = 0;
            _remainingZ = 0;
            MovementNodes = new List<Node>();
            _isAlive = true;
        }

        void Update()
        {
            if (MovementNodes.Count > 0)
            {
                
            }
            else if (!_isAlive)
            {
                if (gameObject.transform.position.y < -.1)
                {
                    if (_battleSmokeInstance != null)
                    {
                        Destroy(_battleSmokeInstance);
                    }

                    Destroy(gameObject);
                }

                Vector3 updatedPosition = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * VerticalMovementPerSecond, transform.position.z);

                transform.position = updatedPosition;
            }

        }

        void OnLoseBattle(GameObject battleSmokeInstance)
        {
            _battleSmokeInstance = battleSmokeInstance;
            _isAlive = false;
        }

        void OnBeginMoveAnimation(NodePath currentPath)
        {
            
        }
    }
}
