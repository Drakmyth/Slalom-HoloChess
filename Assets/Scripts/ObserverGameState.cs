using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using DejarikLibrary;
using Assets.Scripts.Monsters;
using Random = System.Random;

namespace Assets.Scripts
{

    public class ObserverGameState: ClientGameState
    {

        private readonly Random _random = new Random();

        private Node SelectedAttackNode { get; set; }
        private NodePath SelectedMovementPath { get; set; }

        //TODO: we can probably do better than this
        private IEnumerable<Node> AvailablePushDestinations { get; set; }

        void Start()
        {

            Client = GameManager.Instance.Client;

            if (Client.IsPlayer)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            GameGraph = new BoardGraph();
            BoardSpaces = new Dictionary<int, BoardSpace>();
            FriendlyMonsters = new List<Monster>();
            EnemyMonsters = new List<Monster>();
            AttackCalculator = new AttackCalculator();
            MoveCalculator = new MoveCalculator();

            AvailablePushDestinations = new List<Node>();

            BattleSmoke.gameObject.SetActive(false);

            Client.GameState = this;

            float attackResultTextRotationOffset = Client.IsHost ? 180 : 0;

            AttackResultTextPrefab.YRotationOffset = attackResultTextRotationOffset;

            _actionNumber = 1;

            if (!Client.IsHost)
            {
                _actionNumber = 3;

                var table = GameObject.Find("Table");
                table.transform.localRotation = Quaternion.Euler(table.transform.localRotation.eulerAngles.x,
                    table.transform.localRotation.eulerAngles.y + 180, table.transform.localRotation.eulerAngles.z);
            }

            _subActionNumber = 1;

            for (int i = 0; i < SpacePrefabs.Count; i++)
            {
                BoardSpace spacePrefab = SpacePrefabs[i];
                if (!BoardSpaces.ContainsKey(i))
                {
                    spacePrefab.Node = GameGraph.Nodes[i];
                    BoardSpaces.Add(i, spacePrefab);
                }
            }
        }

        void Update()
        {
            if (!GameManager.Instance.gameObject.activeSelf)
            {
                //Something has killed the game loop. DontDestroyOnLoad doesn't protect from deactivation
                GameManager.Instance.gameObject.SetActive(true);
            }

        }

        public override void ConfirmAvailableMonsters(List<int> availableMonsterNodeIds, int actionNumber,
                int subActionNumber)
        {
            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            BattleSmoke.SetActive(false);

        }

        public override void ConfirmSelectMonster(int selectedMonsterId, int actionNumber, int subActionNumber)
        {
            SelectedMonster = FriendlyMonsters.SingleOrDefault(m => m.MonsterTypeId == selectedMonsterId) ?? EnemyMonsters.SingleOrDefault(m => m.MonsterTypeId == selectedMonsterId);
            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;
        }

        public override void ConfirmAvailableActions(List<int> availableMoveActionNodeIds, List<int> availableAttackActionNodeIds, int actionNumber, int subActionNumber)
        {
            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

        }

        public override void ConfirmSelectMoveAction(List<int> pathToDestination, int destinationNodeId, int actionNumber, int subActionNumber)
        {

            SelectedMovementPath = new NodePath(pathToDestination.Select(i => GameGraph.Nodes[i]).ToList(), GameGraph.Nodes[destinationNodeId]);
            SelectedAttackNode = null;

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            ProcessMoveAction(SelectedMonster, SelectedMovementPath);

        }

        public override void ConfirmSelectAttackAction(int attackNodeId, int actionNumber, int subActionNumber)
        {
            SelectedAttackNode = GameGraph.Nodes[attackNodeId];
            SelectedMovementPath = null;

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            Monster opponent = EnemyMonsters.FirstOrDefault(monster => monster.CurrentNode.Equals(SelectedAttackNode)) ?? FriendlyMonsters.FirstOrDefault(monster => monster.CurrentNode.Equals(SelectedAttackNode));

            ProcessAttackAction(SelectedMonster, opponent);

        }

        public override void ConfirmAttackKillResult(AttackResult attackResult, int attackingMonsterTypeId, int defendingMonsterTypeId, int actionNumber, int subActionNumber)
        {
            bool isFriendlyMonster = _actionNumber == 1 || _actionNumber == 2;

            Monster attacker = FriendlyMonsters.SingleOrDefault(m => m.MonsterTypeId == attackingMonsterTypeId) ??
                               EnemyMonsters.SingleOrDefault(m => m.MonsterTypeId == attackingMonsterTypeId);

            Monster defender = FriendlyMonsters.SingleOrDefault(m => m.MonsterTypeId == defendingMonsterTypeId) ??
                   EnemyMonsters.SingleOrDefault(m => m.MonsterTypeId == defendingMonsterTypeId);
            
            Vector3 battlePosition = new Vector3(BattleSmoke.transform.localPosition.x, BattleSmoke.transform.localPosition.y, BattleSmoke.transform.localPosition.z);

            if (attackResult == AttackResult.Kill)
            {
                AttackResultTextPrefab.GetComponent<TextMesh>().text = "KiLL";
                AttackResultTextPrefab.gameObject.SetActive(true);
                AttackResultTextPrefab.transform.localPosition = battlePosition;

                AttackResultTextPrefab.LerpDestination = AttackResultTextPrefab.transform.localPosition + Vector3.up;

                ProcessKill(defender, !isFriendlyMonster);
            } else if (attackResult == AttackResult.CounterKill)
            {
                AttackResultTextPrefab.GetComponent<TextMesh>().text = "Counter KiLL";
                AttackResultTextPrefab.gameObject.SetActive(true);
                AttackResultTextPrefab.transform.localPosition = battlePosition;

                AttackResultTextPrefab.LerpDestination = AttackResultTextPrefab.transform.localPosition + Vector3.up;

                ProcessKill(attacker, isFriendlyMonster);
                SelectedMonster = null;
            }

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

        }

        public override void ConfirmAttackPushResult(AttackResult attackResult, IEnumerable<int> availablePushDestinationIds, int attackingMonsterTypeId, int defendingMonsterTypeId, int actionNumber, int subActionNumber)
        {
            Vector3 battlePosition = new Vector3(BattleSmoke.transform.localPosition.x, BattleSmoke.transform.localPosition.y, BattleSmoke.transform.localPosition.z);

            if (availablePushDestinationIds.Any())
            {
                AvailablePushDestinations = availablePushDestinationIds.Select(n => GameGraph.Nodes[n]);
            }

            if (attackResult == AttackResult.Push)
            {
                AttackResultTextPrefab.GetComponent<TextMesh>().text = "push";
                AttackResultTextPrefab.gameObject.SetActive(true);
                AttackResultTextPrefab.transform.localPosition = battlePosition;

                AttackResultTextPrefab.LerpDestination = AttackResultTextPrefab.transform.localPosition + Vector3.up;

                BattleSmoke.SetActive(false);

            }
            else if (attackResult == AttackResult.CounterPush)
            {
                AttackResultTextPrefab.GetComponent<TextMesh>().text = "Counter push";
                AttackResultTextPrefab.gameObject.SetActive(true);
                AttackResultTextPrefab.transform.localPosition = battlePosition;

                AttackResultTextPrefab.LerpDestination = AttackResultTextPrefab.transform.localPosition + Vector3.up;

                BattleSmoke.SetActive(false);

            }

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;
        }

        public override void ConfirmPushDestination(int[] pathToDestinationNodeIds, int destinationNodeId, int actionNumber, int subActionNumber)
        {
            bool enemyPush = (_actionNumber == 3 || _actionNumber == 4) && _subActionNumber == 6;
            bool enemyCounterPush = (_actionNumber == 1 || _actionNumber == 2) && _subActionNumber == 7;

            bool push = (_actionNumber == 1 || _actionNumber == 2) && _subActionNumber == 6;
            bool counterPush = (_actionNumber == 3 || _actionNumber == 4) && _subActionNumber == 7;

            Monster pushedMonster;

            if (push)
            {
                pushedMonster = EnemyMonsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id);
            }
            else if (counterPush)
            {
                pushedMonster = EnemyMonsters.Single(m => m.CurrentNode.Id == SelectedMonster.CurrentNode.Id);
            }
            else if (enemyPush)
            {
                pushedMonster = FriendlyMonsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id);
            }
            else if (enemyCounterPush)
            {
                pushedMonster = FriendlyMonsters.Single(m => m.CurrentNode.Id == SelectedMonster.CurrentNode.Id);
            }
            else
            {
                return;
            }
            Node selectedNode = GameGraph.Nodes[destinationNodeId];
            List<Node> pathToDestination = new List<Node> { selectedNode };
            NodePath movementPath = new NodePath(pathToDestination, selectedNode);
            _isAnimationRunning = true;
            pushedMonster.SendMessage("OnBeginMoveAnimation", movementPath);

            pushedMonster.CurrentNode = selectedNode;

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

        }

        private void ProcessAttackAction(Monster attacker, Monster defender)
        {
            if (attacker == null && defender == null)
            {
                return;
            }

            Vector3 battleSmokePosition = new Vector3((attacker.CurrentNode.XPosition + defender.CurrentNode.XPosition) / 2f, 0, (attacker.CurrentNode.YPosition + defender.CurrentNode.YPosition) / 2f);
            BattleSmoke.SetActive(true);
            BattleSmoke.transform.localPosition = battleSmokePosition;

            attacker.SendMessage("OnBeginBattle", defender.CurrentNode);
            defender.SendMessage("OnBeginBattle", attacker.CurrentNode);

            int number = _random.Next(0, AttackSounds.Count - 1);
            int number2 = _random.Next(0, AttackSounds.Count - 1);
            attacker.PlaySound(AttackSounds[number]);
            defender.PlaySound(AttackSounds[number2]);
        }

        private void ProcessKill(Monster killed, bool isFriendlyMonster)
        {
            MonsterPrefabs.Remove(killed);

            if (isFriendlyMonster)
            {
                FriendlyMonsters.Remove(killed);
            }
            else
            {
                EnemyMonsters.Remove(killed);
            }

            _isAnimationRunning = true;
            killed.SendMessage("OnLoseBattle", BattleSmoke);
        }

        private void ProcessMoveAction(Monster selectedMonster, NodePath path)
        {
            if (selectedMonster != null)
            {
                int number = _random.Next(0, MovementSounds.Count - 1);
                selectedMonster.PlaySound(MovementSounds[number]);

                selectedMonster.CurrentNode = GameGraph.Nodes[path.DestinationNode.Id];

                _isAnimationRunning = true;

                selectedMonster.SendMessage("OnBeginMoveAnimation", path);
            }
        }

        public override void SyncGameState(Dictionary<int, int> friendlyMonsterState, Dictionary<int, int> enemyMonsterState, IEnumerable<int> movementPathIds, IEnumerable<int> availablePushDestinationIds, int actionNumber, int subActionNumber, int selectedMonsterTypeId, int selectedAttackNodeId, int destinationNodeId)
        {
            //Need to wait for animations to finish or things start to look weird
            if (_isAnimationRunning)
            {
                return;
            }

            UpdateGameState(actionNumber, subActionNumber, friendlyMonsterState, enemyMonsterState, true);

            SelectedMonster = MonsterPrefabs.SingleOrDefault(m => m.MonsterTypeId == selectedMonsterTypeId);
            SelectedAttackNode = GameGraph.Nodes[selectedAttackNodeId];

            SelectedMovementPath = movementPathIds == null ? null : new NodePath
            {
                DestinationNode = GameGraph.Nodes[destinationNodeId],
                PathToDestination = GameGraph.Nodes.Where(n => movementPathIds.Contains(n.Id)).ToList()
            };

            AvailablePushDestinations = GameGraph.Nodes.Where(n => availablePushDestinationIds.Contains(n.Id)).ToList();
        }

        public override void UpdateGameState(int actionNumber, int subActionNumber, IDictionary<int, int> friendlyMonsterState, IDictionary<int, int> enemyMonsterState, bool isFullSync = false)
        {
            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            foreach (int monsterTypeId in friendlyMonsterState.Keys)
            {
                if (FriendlyMonsters.All(m => m.MonsterTypeId != monsterTypeId))
                {
                    Monster monster = MonsterPrefabs.SingleOrDefault(m => m.MonsterTypeId == monsterTypeId);
                    monster.CurrentNode = GameGraph.Nodes[friendlyMonsterState[monsterTypeId]];
                    FriendlyMonsters.Add(monster);
                }
            }

            foreach (int monsterTypeId in enemyMonsterState.Keys)
            {
                if (EnemyMonsters.All(m => m.MonsterTypeId != monsterTypeId))
                {
                    Monster monster = MonsterPrefabs.SingleOrDefault(m => m.MonsterTypeId == monsterTypeId);
                    monster.CurrentNode = GameGraph.Nodes[enemyMonsterState[monsterTypeId]];
                    EnemyMonsters.Add(monster);
                }
            }

            foreach (Monster monster in FriendlyMonsters)
            {
                if (!friendlyMonsterState.ContainsKey(monster.MonsterTypeId))
                {
                    FriendlyMonsters.Remove(monster);
                    MonsterPrefabs.Remove(monster);
                }
                else if (monster.CurrentNode.Id != friendlyMonsterState[monster.MonsterTypeId])
                {
                    monster.CurrentNode = GameGraph.Nodes[friendlyMonsterState[monster.MonsterTypeId]];
                }
            }

            foreach (Monster monster in EnemyMonsters)
            {
                if (!enemyMonsterState.ContainsKey(monster.MonsterTypeId))
                {
                    EnemyMonsters.Remove(monster);
                    MonsterPrefabs.Remove(monster);
                }
                else if (monster.CurrentNode.Id != enemyMonsterState[monster.MonsterTypeId])
                {
                    monster.CurrentNode = GameGraph.Nodes[enemyMonsterState[monster.MonsterTypeId]];
                }
            }

            DisplayMonsters(FriendlyMonsters, EnemyMonsters);

            if (!isFullSync)
            {
                if (_actionNumber == 1 || _actionNumber == 3)
                {
                    SelectedMonster = null;
                }

                SelectedAttackNode = null;
                SelectedMovementPath = null;
            }
            
        }

        public override Monster GetSelectedMonsterPrefab()
        {
            return SelectedMonster == null ? null : MonsterPrefabs.First(t => t.Name == SelectedMonster.Name);
        }

        public override int GetAdjustedActionNumber()
        {
            if (!Client.IsHost)
            {
                return (_actionNumber + 1) % 4 + 1;
            }

            return _actionNumber;
        }


        private void DisplayMonsters(List<Monster> friendlyMonsters, List<Monster> enemyMonsters)
        {
            foreach (Monster monster in friendlyMonsters)
            {

                float yRotationAdjustment = Client.IsHost ? 0 : 180;

                Quaternion monsterQuaternion = Quaternion.Euler(monster.transform.localRotation.eulerAngles.x, yRotationAdjustment, monster.transform.localRotation.eulerAngles.z);
                Vector3 monsterPosition = new Vector3(monster.CurrentNode.XPosition, 0, monster.CurrentNode.YPosition);

                monster.transform.localPosition = monsterPosition;
                monster.transform.localRotation = monsterQuaternion;

                monster.gameObject.SetActive(true);

                monster.BelongsToHost = false;
                monster.YRotationAdjustment = yRotationAdjustment;
                monster.ShouldActivate();
                if (FriendlyMonsters.All(m => m.MonsterTypeId != monster.MonsterTypeId))
                {
                    FriendlyMonsters.Add(monster);
                }
            }

            foreach (Monster enemyMonster in enemyMonsters)
            {
                float yRotationAdjustment = Client.IsHost ? 180 : 0;

                Quaternion enemyMonsterQuaternion = Quaternion.Euler(enemyMonster.transform.localRotation.eulerAngles.x, yRotationAdjustment, enemyMonster.transform.localRotation.eulerAngles.z);
                Vector3 enemyMonsterPosition = new Vector3(enemyMonster.CurrentNode.XPosition, 0, enemyMonster.CurrentNode.YPosition);

                enemyMonster.transform.localPosition = enemyMonsterPosition;
                enemyMonster.transform.localRotation = enemyMonsterQuaternion;

                enemyMonster.BelongsToHost = true;
                enemyMonster.YRotationAdjustment = yRotationAdjustment;

                enemyMonster.ShouldActivate();
                if (EnemyMonsters.All(m => m.MonsterTypeId != enemyMonster.MonsterTypeId))
                {
                    EnemyMonsters.Add(enemyMonster);
                }
            }
        }
    }
}