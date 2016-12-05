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
        public abstract string Name { get; }
        public abstract int MonsterTypeId { get; }

        public Node CurrentNode { get; set; }

        //TODO: remove this work-around for rotation
        public bool BelongsToHost { get; set; }

        private List<Node> MovementNodes { get; set; }

        private Node CombatTarget { get; set; }

        private bool _isAlive;

        private bool _isInCombat;

        private float _movementDelta;
        private float _rotationDelta;

        private float _initialXRotation;
        private float _initialYRotation;
        private float _initialZRotation;

        private const float HorizontalMovementRate = 1f;

        private const float VerticalMovementPerSecond = .05f;

        private const float RotationRate = .1f;

        private GameObject _battleSmokeInstance;

        private AudioSource _audioSource;

        void Start()
        {
            _initialXRotation = transform.rotation.eulerAngles.x;
            _initialYRotation = transform.rotation.eulerAngles.y;
            _initialZRotation = transform.rotation.eulerAngles.z;
            _movementDelta = 0;
            _rotationDelta = 0;
            MovementNodes = new List<Node>();
            _isAlive = true;
        }

        void Update()
        {
            if (MovementNodes.Count > 0)
            {

                _movementDelta += Time.deltaTime * HorizontalMovementRate;
                _rotationDelta += RotationRate;

                if (_movementDelta >= 1)
                {
                    _movementDelta = 1;
                }

                Vector3 newPosition = Vector3.Lerp(transform.position, new Vector3(MovementNodes[0].XPosition, transform.position.y,
                MovementNodes[0].YPosition), _movementDelta);

                Vector3 deltaVector = newPosition - transform.position;

                transform.position = newPosition;

                if (_movementDelta >= 1)
                {
                    _movementDelta = 0;
                    _rotationDelta = 0;

                    MovementNodes.RemoveAt(0);

                    if (MovementNodes.Count == 0)
                    {
                        GameObject.Find("GameState").SendMessage("OnAnimationComplete");
                    }

                }
                else if (_rotationDelta < 1)
                {

                    Quaternion lookRotation = Quaternion.LookRotation(deltaVector.normalized);

                    float yAdjustment = 0f;

                    if (!BelongsToHost)
                    {
                        yAdjustment = 180f;
                    }

                    lookRotation = Quaternion.Euler(lookRotation.eulerAngles.x + _initialXRotation, lookRotation.eulerAngles.y + _initialYRotation + yAdjustment, lookRotation.eulerAngles.z + _initialZRotation);

                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationDelta);

                }
            }
            else if (_isInCombat)
            {
                _rotationDelta += RotationRate;

                Vector3 deltaVector = new Vector3(CombatTarget.XPosition, 0, CombatTarget.YPosition) - new Vector3(CurrentNode.XPosition, 0, CurrentNode.YPosition);

                Quaternion lookRotation = Quaternion.LookRotation(deltaVector.normalized);

                float yAdjustment = 0f;

                if (!BelongsToHost)
                {
                    yAdjustment = 180f;
                }

                lookRotation = Quaternion.Euler(lookRotation.eulerAngles.x + _initialXRotation, lookRotation.eulerAngles.y + _initialYRotation + yAdjustment, lookRotation.eulerAngles.z + _initialZRotation);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _rotationDelta);

                if (_rotationDelta >= 1)
                {
                    _isInCombat = false;
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

        void OnBeginBattle(Node combatTarget)
        {
            CombatTarget = combatTarget;
            _isInCombat = true;
        }

        void OnLoseBattle(GameObject battleSmokeInstance)
        {
            _battleSmokeInstance = battleSmokeInstance;
            _isAlive = false;
        }

        void OnBeginMoveAnimation(NodePath currentPath)
        {
            MovementNodes = currentPath.PathToDestination;
            _movementDelta = 0;
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

        public void PlaySound(AudioClip soundToPlay)
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            }

            _audioSource.clip = soundToPlay;
            _audioSource.PlayDelayed(0);
        }
    }
}
