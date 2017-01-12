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

        //TODO: find a way to remove this work-around for rotation
        public bool BelongsToHost { get; set; }
        public float YRotationAdjustment { get; set; }

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

        private const float VerticalMovementPerSecond = .4f;

        private const float RotationRate = .1f;

        private GameObject _battleSmokeInstance;

        private AudioSource _audioSource;

        void Start()
        {
            _initialXRotation = transform.localRotation.eulerAngles.x;
            _initialYRotation = transform.localRotation.eulerAngles.y + YRotationAdjustment;
            _initialZRotation = transform.localRotation.eulerAngles.z;
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

                Vector3 newPosition = Vector3.Lerp(transform.localPosition, new Vector3(MovementNodes[0].XPosition, transform.localPosition.y,
                MovementNodes[0].YPosition), _movementDelta);

                Vector3 deltaVector = newPosition - transform.localPosition;

                transform.localPosition = newPosition;

                if (_movementDelta >= 1)
                {
                    _movementDelta = 0;
                    _rotationDelta = 0;

                    MovementNodes.RemoveAt(0);

                    if (MovementNodes.Count == 0)
                    {
                        GameObject.Find("GameState").GetComponent<ClientGameState>().SendMessage("OnAnimationComplete");
                    }

                }
                else if (_rotationDelta < 1)
                {

                    Quaternion lookRotation = Quaternion.LookRotation(deltaVector.normalized);

                    float yAdjustment = 180f;

                    lookRotation = Quaternion.Euler(lookRotation.eulerAngles.x + _initialXRotation, lookRotation.eulerAngles.y + _initialYRotation + yAdjustment, lookRotation.eulerAngles.z + _initialZRotation);

                    transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRotation, _rotationDelta);

                }
            }
            else if (_isInCombat)
            {
                _rotationDelta += RotationRate;

                Vector3 deltaVector = new Vector3(CombatTarget.XPosition, 0, CombatTarget.YPosition) - new Vector3(CurrentNode.XPosition, 0, CurrentNode.YPosition);

                Quaternion lookRotation = Quaternion.LookRotation(deltaVector.normalized);

                float yAdjustment = 180f;

                lookRotation = Quaternion.Euler(lookRotation.eulerAngles.x + _initialXRotation, lookRotation.eulerAngles.y + _initialYRotation + yAdjustment, lookRotation.eulerAngles.z + _initialZRotation);

                transform.localRotation = Quaternion.Slerp(transform.localRotation, lookRotation, _rotationDelta);

                if (_rotationDelta >= 1)
                {
                    _isInCombat = false;
                }

            }
            else if (!_isAlive)
            {
                if (gameObject.transform.localPosition.y < -1.2)
                {
                    if (_battleSmokeInstance != null)
                    {
                        _battleSmokeInstance.SetActive(false);
                    }

                    gameObject.SetActive(false);

                    GameObject.Find("GameState").GetComponent<ClientGameState>().SendMessage("OnAnimationComplete");

                }

                Vector3 updatedPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - Time.deltaTime * VerticalMovementPerSecond, transform.localPosition.z);

                transform.localPosition = updatedPosition;
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
