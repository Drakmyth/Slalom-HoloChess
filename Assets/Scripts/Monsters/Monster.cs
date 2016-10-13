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

        private float _initialXRotation;
        private float _initialYRotation;
        private float _initialZRotation;

        private const float HorizontalMovementPerSecond = .2f;

        private const float VerticalMovementPerSecond = .05f;

        private const float RotationPerSecond = 180;

        private GameObject _battleSmokeInstance;

        void Start()
        {
            _initialXRotation = transform.rotation.eulerAngles.x;
            _initialYRotation = transform.rotation.eulerAngles.y;
            _initialZRotation = transform.rotation.eulerAngles.z;
            _remainingX = 0;
            _remainingZ = 0;
            MovementNodes = new List<Node>();
            _isAlive = true;
        }

        void Update()
        {
            if (MovementNodes.Count > 0)
            {
                float delta = Time.deltaTime * HorizontalMovementPerSecond;

                float zDelta = _remainingZ / Mathf.Abs(_remainingX) * delta;
                float xDelta = _remainingX / Mathf.Abs(_remainingZ) * delta;

                if (Mathf.Abs(zDelta) >= Mathf.Abs(_remainingZ) || Mathf.Abs(xDelta) >= Mathf.Abs(_remainingX))
                {
                    _remainingZ = 0;
                    _remainingX = 0;

                    transform.position = new Vector3(MovementNodes[0].XPosition, transform.position.y, MovementNodes[0].YPosition);

                    MovementNodes.RemoveAt(0);

                    if (MovementNodes.Count == 0)
                    {
                        GameObject.Find("GameState").SendMessage("OnAnimationComplete");
                    }
                    else
                    {
                        _remainingZ = MovementNodes[0].YPosition - transform.position.z;
                        _remainingX = MovementNodes[0].XPosition - transform.position.x;
                    }

                }
                else
                {
                    _remainingZ -= zDelta;
                    _remainingX -= xDelta;

                    Vector3 deltaVector = new Vector3(xDelta, 0, zDelta);

                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z) + deltaVector;

                    Quaternion lookRotation = Quaternion.LookRotation(deltaVector.normalized);

                    lookRotation = Quaternion.Euler(lookRotation.eulerAngles.x + _initialXRotation, lookRotation.eulerAngles.y + _initialYRotation, lookRotation.eulerAngles.z + _initialZRotation);

                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * RotationPerSecond);

                }
            }
            else if (!_isAlive)
            {
                if (gameObject.transform.position.y < -.2)
                {
                    if (_battleSmokeInstance != null)
                    {
                        Destroy(_battleSmokeInstance);
                    }

                    Destroy(gameObject);

                    GameObject.Find("GameState").SendMessage("OnAnimationComplete");

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
            MovementNodes = currentPath.PathToDestination;
            _remainingZ = MovementNodes[0].YPosition - transform.position.z;
            _remainingX = MovementNodes[0].XPosition - transform.position.x;
        }

        public override bool Equals(object o)
        {
            return Equals(o as Monster);
        }

        public bool Equals(Monster monster)
        {
            return monster.AttackRating == AttackRating && monster.DefenseRating == DefenseRating &&
                   monster.MovementRating == MovementRating;
        }

        public override int GetHashCode()
        {
            return AttackRating.GetHashCode() + DefenseRating.GetHashCode() + MovementRating.GetHashCode();
        }
    }
}
