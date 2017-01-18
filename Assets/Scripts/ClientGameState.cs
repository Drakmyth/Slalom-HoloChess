using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.MessageModels;
using DejarikLibrary;
using Assets.Scripts.Monsters;
using Random = System.Random;

namespace Assets.Scripts
{

    public class ClientGameState: MonoBehaviour
    {
        public Client Client { get; set; }
        public BoardGraph GameGraph { get; set; }
        public AttackCalculator AttackCalculator { get; set; }
        public MoveCalculator MoveCalculator { get; set; }
        public Dictionary<int, BoardSpace> BoardSpaces { get; set; }
        public List<Monster> FriendlyMonsters { get; set; }
        public List<Monster> EnemyMonsters { get; set; }

        private readonly Random _random = new Random();

        //0 : GameEnded
        //1 : HostAction
        //2 : HostAction
        //3 : GuestAction
        //4 : GuestAction
        private int _actionNumber;

        //TODO: seems a decent candidate for an enum
        //1 : Highlight available monsters
        //2 : Select own monster node (await user input)
        //3 : Get available moves, update highlighting 
        //4 : Select Action node (await user input)
        //5 : Process Action
        //6 : Select Push result (await user input)
        //7 : Listen for CounterPush result (await opponent input)
        private int _subActionNumber;
        
        private bool _isAnimationRunning = false;
        private Monster SelectedMonster { get; set; }
        private Monster PreviewMonster { get; set; }
        private Node SelectedAttackNode { get; set; }
        private NodePath SelectedMovementPath { get; set; }

        //TODO: we can probably do better than this
        private IEnumerable<Node> AvailablePushDestinations { get; set; }

        public List<Monster> MonsterPrefabs;
        public List<BoardSpace> SpacePrefabs;
        public GameObject BattleSmoke;

        //TODO: consolidate these
        public AttackResultText PushResultTextPrefab;
        public AttackResultText KillResultTextPrefab;
        public AttackResultText CounterPushResultTextPrefab;
        public AttackResultText CounterKillResultTextPrefab;

        public List<AudioClip> AttackSounds;
        public List<AudioClip> MovementSounds;

        public static ClientGameState Instance;

        void Start()
        {
            Instance = this;

            GameGraph = new BoardGraph();
            BoardSpaces = new Dictionary<int, BoardSpace>();
            FriendlyMonsters = new List<Monster>();
            EnemyMonsters = new List<Monster>();
            AttackCalculator = new AttackCalculator();
            MoveCalculator = new MoveCalculator();

            AvailablePushDestinations = new List<Node>();

            BattleSmoke.gameObject.SetActive(false);

            Client = GameManager.Instance.Client;

            Client.GameState = this;

            _actionNumber = 1;

            if (!Client.IsHost)
            {
                _actionNumber = 3;

                Camera.main.transform.parent.localPosition = new Vector3(Camera.main.transform.parent.localPosition.x, Camera.main.transform.parent.localPosition.y, Camera.main.transform.parent.localPosition.z * -1);
                Camera.main.transform.parent.localRotation =
                    Quaternion.Euler(new Vector3(Camera.main.transform.parent.localRotation.eulerAngles.x,
                        Camera.main.transform.parent.localRotation.eulerAngles.y + 180,
                        Camera.main.transform.parent.localRotation.eulerAngles.z));

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

            List<Monster> friendlyMonsters = new List<Monster>();
            List<Monster> enemyMonsters = new List<Monster>();
            foreach (Monster monster in MonsterPrefabs)
            {
                if (GameManager.Instance.FriendlyMonsterInitialNodeIds.ContainsKey(monster.MonsterTypeId))
                {
                    monster.CurrentNode = GameGraph.Nodes[GameManager.Instance.FriendlyMonsterInitialNodeIds[monster.MonsterTypeId]];
                    friendlyMonsters.Add(monster);
                }
                else
                {
                    monster.CurrentNode = GameGraph.Nodes[GameManager.Instance.EnemyMonsterInitialNodeIds[monster.MonsterTypeId]];
                    enemyMonsters.Add(monster);
                }

            }

            DisplayMonsters(friendlyMonsters, enemyMonsters);

            Client.SendStateAck(GetAdjustedActionNumber(), _subActionNumber);

        }

        void Update()
        {
            if (!GameManager.Instance.gameObject.activeSelf)
            {
                //Something has killed the game loop. DontDestroyOnLoad doesn't protect from deactivation
                GameManager.Instance.gameObject.SetActive(true);
            }

            if (Client == null || _actionNumber < 1)
            {
                return;
            }

            if (_isAnimationRunning)
            {
                if (_subActionNumber == 5 && SelectedMonster != null && FriendlyMonsters.Contains(SelectedMonster))
                {
                    IEnumerable<BoardSpace> availableSpaces =
                        BoardSpaces.Values.Where(b => FriendlyMonsters.Select(f => f.CurrentNode.Id).Contains(b.Node.Id));
                    foreach (BoardSpace space in availableSpaces)
                    {
                        space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
                    }
                    BoardSpaces[SelectedMonster.CurrentNode.Id].SendMessage("OnClearHighlightingWithSelection",
                        SelectedMonster.CurrentNode);
                }

            }
        }

        void OnSpaceSelected(int nodeId)
        {
            Node selectedNode = GameGraph.Nodes[nodeId];

            if (_actionNumber == 1 || _actionNumber == 2)
            {
                if (_subActionNumber == 2)
                {
                    SelectMonster(nodeId);
                }

                if (_subActionNumber == 4)
                {
                    SelectAction(selectedNode);
                }

                if (_subActionNumber == 6)
                {
                    SelectPushDestination(selectedNode);
                }
            }
            else if (_subActionNumber == 7 && (_actionNumber == 3 || _actionNumber == 4))
            {
                SelectPushDestination(selectedNode);
            }

        }

        void OnPreviewEnter(int nodeId)
        {
            if (_actionNumber == 1 || _actionNumber == 2)
            {
                PreviewMonster = EnemyMonsters.FirstOrDefault(m => m.CurrentNode.Id == nodeId) ?? FriendlyMonsters.FirstOrDefault(m => m.CurrentNode.Id == nodeId);

                UpdatePreviewMenu();

                if (SelectedMonster != null && EnemyMonsters.Any(m => m.CurrentNode.Id == nodeId))
                {
                    UpdateAttackResultPreview();
                }
                else if (SelectedMonster != null && FriendlyMonsters.Any(m => m.CurrentNode.Id == nodeId))
                {
                    ClearAttackResultPreview();
                }
            }
        }

        void OnAnimationComplete()
        {
            _isAnimationRunning = false;
            Client.SendStateAck(GetAdjustedActionNumber(), _subActionNumber);
        }

        public void ConfirmAvailableMonsters(List<int> availableMonsterNodeIds, int actionNumber,
                int subActionNumber)
        {
            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            var availableSpaces = availableMonsterNodeIds.Select(s => BoardSpaces[s]);

            if (_actionNumber == 1 || _actionNumber == 2)
            {
                foreach (BoardSpace space in availableSpaces)
                {
                    space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
                }
            }

            Client.SendStateAck(GetAdjustedActionNumber(), _subActionNumber);

        }

        private void SelectMonster(int nodeId)
        {
            SelectedMonster = FriendlyMonsters.SingleOrDefault(m => m.CurrentNode.Id == nodeId);
            if (SelectedMonster != null)
            {
                Client.Send(CustomMessageTypes.SelectMonsterRequest, new SelectMonsterRequestMessage
                {
                    ActionNumber = GetAdjustedActionNumber(),
                    SubActionNumber = _subActionNumber,
                    Message = SelectedMonster.Name,
                    MessageTypeId = CustomMessageTypes.SelectMonsterRequest,
                    SelectedMonsterTypeId = SelectedMonster.MonsterTypeId
                });
            }
        }

        public void ConfirmSelectMonster(int selectedMonsterId, int actionNumber, int subActionNumber)
        {
            SelectedMonster = FriendlyMonsters.SingleOrDefault(m => m.MonsterTypeId == selectedMonsterId) ?? EnemyMonsters.Single(m => m.MonsterTypeId == selectedMonsterId);
            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            if (SelectedMonster != null && (_actionNumber == 1 || _actionNumber == 2))
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnMonsterSelected", SelectedMonster.CurrentNode.Id);
                }
            }
            Client.SendStateAck(GetAdjustedActionNumber(), _subActionNumber);

        }

        public void ConfirmAvailableActions(List<int> availableMoveActionNodeIds, List<int> availableAttackActionNodeIds, int actionNumber, int subActionNumber)
        {

            if (_actionNumber == 1 || _actionNumber == 2)
            {
                UpdateSelectionMenu();

                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnAvailableAttacks", availableAttackActionNodeIds);
                    space.SendMessage("OnAvailableMoves", availableMoveActionNodeIds);
                }

            }

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;
            Client.SendStateAck(GetAdjustedActionNumber(), _subActionNumber);

        }

        private void SelectAction(Node selectedNode)
        {
            Client.Send(CustomMessageTypes.SelectActionRequest, new SelectActionRequestMessage
            {
                SelectedNodeId = selectedNode.Id,
                ActionNumber = GetAdjustedActionNumber(),
                SubActionNumber = _subActionNumber,
                Message = SelectedMonster.Name,
                MessageTypeId = CustomMessageTypes.SelectMonsterRequest,
            });
        }

        public void ConfirmSelectMoveAction(List<int> pathToDestination, int destinationNodeId, int actionNumber, int subActionNumber)
        {

            SelectedMovementPath = new NodePath(pathToDestination.Select(i => GameGraph.Nodes[i]).ToList(), GameGraph.Nodes[destinationNodeId]);
            SelectedAttackNode = null;

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            ProcessMoveAction(SelectedMonster, SelectedMovementPath);

        }

        public void ConfirmSelectAttackAction(int attackNodeId, int actionNumber, int subActionNumber)
        {
            SelectedAttackNode = GameGraph.Nodes[attackNodeId];
            SelectedMovementPath = null;

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

            Monster opponent = EnemyMonsters.FirstOrDefault(monster => monster.CurrentNode.Equals(SelectedAttackNode)) ?? FriendlyMonsters.FirstOrDefault(monster => monster.CurrentNode.Equals(SelectedAttackNode));

            ProcessAttackAction(SelectedMonster, opponent);

            Client.SendStateAck(GetAdjustedActionNumber(), _subActionNumber);

        }

        public void ConfirmAttackKillResult(AttackResult attackResult, int attackingMonsterTypeId, int defendingMonsterTypeId, int actionNumber, int subActionNumber)
        {
            bool isFriendlyMonster = _actionNumber == 1 || _actionNumber == 2;

            Monster attacker = FriendlyMonsters.SingleOrDefault(m => m.MonsterTypeId == attackingMonsterTypeId) ??
                               EnemyMonsters.SingleOrDefault(m => m.MonsterTypeId == attackingMonsterTypeId);

            Monster defender = FriendlyMonsters.SingleOrDefault(m => m.MonsterTypeId == defendingMonsterTypeId) ??
                   EnemyMonsters.SingleOrDefault(m => m.MonsterTypeId == defendingMonsterTypeId);
            
            Vector3 battlePosition = new Vector3(BattleSmoke.transform.localPosition.x, BattleSmoke.transform.localPosition.y, BattleSmoke.transform.localPosition.z);

            if (attackResult == AttackResult.Kill)
            {
                KillResultTextPrefab.gameObject.SetActive(true);
                KillResultTextPrefab.transform.localPosition = battlePosition;

                KillResultTextPrefab.LerpDestination = KillResultTextPrefab.transform.localPosition + Vector3.up;

                ProcessKill(defender, !isFriendlyMonster);
            } else if (attackResult == AttackResult.CounterKill)
            {
                CounterKillResultTextPrefab.gameObject.SetActive(true);
                CounterKillResultTextPrefab.transform.localPosition = battlePosition;

                CounterKillResultTextPrefab.LerpDestination = CounterKillResultTextPrefab.transform.localPosition + Vector3.up;

                ProcessKill(attacker, isFriendlyMonster);
                SelectedMonster = null;
            }

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

        }

        public void ConfirmAttackPushResult(AttackResult attackResult, IEnumerable<int> availablePushDestinationIds, int attackingMonsterTypeId, int defendingMonsterTypeId, int actionNumber, int subActionNumber)
        {
            Vector3 battlePosition = new Vector3(BattleSmoke.transform.localPosition.x, BattleSmoke.transform.localPosition.y, BattleSmoke.transform.localPosition.z);

            if (availablePushDestinationIds.Any())
            {
                AvailablePushDestinations = availablePushDestinationIds.Select(n => GameGraph.Nodes[n]);
            }

            if (attackResult == AttackResult.Push)
            {
                PushResultTextPrefab.gameObject.SetActive(true);
                PushResultTextPrefab.transform.localPosition = battlePosition;

                PushResultTextPrefab.LerpDestination = PushResultTextPrefab.transform.localPosition + Vector3.up;

                if (_actionNumber == 1 || _actionNumber == 2)
                {
                    foreach (BoardSpace space in BoardSpaces.Values)
                    {
                        space.SendMessage("OnAvailableMoves", AvailablePushDestinations.Select(n => n.Id));
                    }

                }

                BattleSmoke.SetActive(false);

            }
            else if (attackResult == AttackResult.CounterPush)
            {
                CounterPushResultTextPrefab.gameObject.SetActive(true);
                CounterPushResultTextPrefab.transform.localPosition = battlePosition;

                CounterPushResultTextPrefab.LerpDestination = CounterPushResultTextPrefab.transform.localPosition + Vector3.up;

                if (_actionNumber == 3 || _actionNumber == 4)
                {
                    foreach (BoardSpace space in BoardSpaces.Values)
                    {
                        space.SendMessage("OnAvailableMoves", AvailablePushDestinations.Select(n => n.Id));
                    }
                }

                BattleSmoke.SetActive(false);

            }

            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;
        }

        private void SelectPushDestination(Node selectedNode)
        {
            if (AvailablePushDestinations.Contains(selectedNode))
            {
                Client.Send(CustomMessageTypes.PushDestinationRequest, new PushDestinationRequestMessage
                {
                    ActionNumber = GetAdjustedActionNumber(),
                    SubActionNumber = _subActionNumber,
                    SelectedNodeId = selectedNode.Id
                });
            }
        }

        public void ConfirmPushDestination(int[] pathToDestinationNodeIds, int destinationNodeId, int actionNumber, int subActionNumber)
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
            Vector3 battleSmokePosition = new Vector3((attacker.CurrentNode.XPosition + defender.CurrentNode.XPosition) / 2f, 0, (attacker.CurrentNode.YPosition + defender.CurrentNode.YPosition) / 2f);
            BattleSmoke.SetActive(true);
            BattleSmoke.transform.localPosition = battleSmokePosition;

            attacker.SendMessage("OnBeginBattle", defender.CurrentNode);
            defender.SendMessage("OnBeginBattle", attacker.CurrentNode);

            int number = _random.Next(0, AttackSounds.Count - 1);
            int number2 = _random.Next(0, AttackSounds.Count - 1);
            attacker.PlaySound(AttackSounds[number]);
            defender.PlaySound(AttackSounds[number2]);

            Client.Send(CustomMessageTypes.AttackRequest, new AttackRequestMessage
            {
                ActionNumber = GetAdjustedActionNumber(),
                SubActionNumber = _subActionNumber,
                AttackingMonsterTypeId = attacker.MonsterTypeId,
                DefendingMonsterTypeId = defender.MonsterTypeId,
                Message = "Requesting attack"
            });

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
            int number = _random.Next(0, MovementSounds.Count - 1);
            selectedMonster.PlaySound(MovementSounds[number]);

            selectedMonster.CurrentNode = GameGraph.Nodes[path.DestinationNode.Id];

            _isAnimationRunning = true;

            foreach (BoardSpace space in BoardSpaces.Values)
            {
                space.SendMessage("OnClearHighlighting");
            }

            selectedMonster.SendMessage("OnBeginMoveAnimation", path);

        }

        public void SyncGameState(Dictionary<int, int> friendlyMonsterState, Dictionary<int, int> enemyMonsterState, IEnumerable<int> movementPathIds, IEnumerable<int> availablePushDestinationIds, int actionNumber, int subActionNumber, int selectedMonsterTypeId, int selectedAttackNodeId, int destinationNodeId)
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

        public void UpdateGameState(int actionNumber, int subActionNumber, IDictionary<int, int> friendlyMonsterState, IDictionary<int, int> enemyMonsterState, bool isFullSync = false)
        {
            _actionNumber = actionNumber;
            _subActionNumber = subActionNumber;

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


            if (_actionNumber == 2 && !isFullSync)
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    Node selectedNode = null;

                    if (SelectedMonster != null)
                    {
                        selectedNode = SelectedMonster.CurrentNode;
                        IEnumerable<BoardSpace> availableSpaces =
                            BoardSpaces.Values.Where(
                                s => FriendlyMonsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();
                        space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
                    }
                    else
                    {
                        ClearSelectionMenu();
                        space.SendMessage("OnClearHighlighting");
                    }

                    space.SendMessage("OnClearHighlightingWithSelection", selectedNode);
                }
            }
            else if (!isFullSync)
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnClearHighlighting");
                }

                if (_actionNumber == 1 || _actionNumber == 3)
                {
                    SelectedMonster = null;
                }

                SelectedAttackNode = null;
                SelectedMovementPath = null;
            }

            if (_actionNumber == 3 || _actionNumber == 4)
            {
                ClearSelectionMenu();
            }

            Client.SendStateAck(GetAdjustedActionNumber(), _subActionNumber);
        }

        private void UpdateAttackResultPreview()
        {
            IDictionary<AttackResult, decimal> attackResultPercentages = AttackResultPreview.GetAttackResultPercentages(SelectedMonster.AttackRating, PreviewMonster.DefenseRating);

            GameObject.Find("KillResultPreview").SendMessage("OnUpdate", attackResultPercentages);
            GameObject.Find("PushResultPreview").SendMessage("OnUpdate", attackResultPercentages);
            GameObject.Find("CounterPushResultPreview").SendMessage("OnUpdate", attackResultPercentages);
            GameObject.Find("CounterKillResultPreview").SendMessage("OnUpdate", attackResultPercentages);
        }

        private void ClearAttackResultPreview()
        {
            GameObject.Find("KillResultPreview").SendMessage("OnClear");
            GameObject.Find("PushResultPreview").SendMessage("OnClear");
            GameObject.Find("CounterPushResultPreview").SendMessage("OnClear");
            GameObject.Find("CounterKillResultPreview").SendMessage("OnClear");
        }

        private void UpdatePreviewMenu()
        {
            if (PreviewMonster != null)
            {
                GameObject.Find("PreviewName").SendMessage("OnUpdate", PreviewMonster.Name);
                GameObject.Find("PreviewMovement").SendMessage("OnUpdate", PreviewMonster.MovementRating.ToString());
                GameObject.Find("PreviewAttack").SendMessage("OnUpdate", PreviewMonster.AttackRating.ToString());
                GameObject.Find("PreviewDefense").SendMessage("OnUpdate", PreviewMonster.DefenseRating.ToString());
            }
        }

        private void UpdateSelectionMenu()
        {
            if (SelectedMonster != null)
            {
                GameObject.Find("SelectionName").SendMessage("OnUpdate", SelectedMonster.Name);
                GameObject.Find("SelectionMovement").SendMessage("OnUpdate", SelectedMonster.MovementRating.ToString());
                GameObject.Find("SelectionAttack").SendMessage("OnUpdate", SelectedMonster.AttackRating.ToString());
                GameObject.Find("SelectionDefense").SendMessage("OnUpdate", SelectedMonster.DefenseRating.ToString());
            }
        }

        private void ClearSelectionMenu()
        {
            GameObject.Find("SelectionName").SendMessage("OnClear");
            GameObject.Find("SelectionMovement").SendMessage("OnClear");
            GameObject.Find("SelectionAttack").SendMessage("OnClear");
            GameObject.Find("SelectionDefense").SendMessage("OnClear");
        }

        public Monster GetSelectedMonsterPrefab()
        {
            return SelectedMonster == null ? null : MonsterPrefabs.First(t => t.Name == SelectedMonster.Name);
        }

        public int GetAdjustedActionNumber()
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

                float yRotationAdjustment = Client.IsHost ? 180 : 0;


                Quaternion monsterQuaternion = Quaternion.Euler(monster.transform.rotation.eulerAngles.x, monster.transform.rotation.eulerAngles.y + yRotationAdjustment, monster.transform.rotation.eulerAngles.z);
                Vector3 monsterPosition = new Vector3(monster.CurrentNode.XPosition, 0, monster.CurrentNode.YPosition);

                monster.transform.localPosition = monsterPosition;
                monster.transform.localRotation = monsterQuaternion;

                monster.gameObject.SetActive(true);

                monster.BelongsToHost = false;
                monster.YRotationAdjustment = yRotationAdjustment;
                monster.gameObject.SetActive(true);
                FriendlyMonsters.Add(monster);
            }

            foreach (Monster enemyMonster in enemyMonsters)
            {
                float yRotationAdjustment = Client.IsHost ? 0 : 180;

                Quaternion enemyMonsterQuaternion = Quaternion.Euler(enemyMonster.transform.rotation.eulerAngles.x, enemyMonster.transform.rotation.eulerAngles.y + yRotationAdjustment, enemyMonster.transform.rotation.eulerAngles.z);
                Vector3 enemyMonsterPosition = new Vector3(enemyMonster.CurrentNode.XPosition, 0, enemyMonster.CurrentNode.YPosition);

                enemyMonster.transform.localPosition = enemyMonsterPosition;
                enemyMonster.transform.localRotation = enemyMonsterQuaternion;

                enemyMonster.BelongsToHost = true;
                enemyMonster.YRotationAdjustment = yRotationAdjustment;
                enemyMonster.gameObject.SetActive(true);
                EnemyMonsters.Add(enemyMonster);
            }
        }
    }
}