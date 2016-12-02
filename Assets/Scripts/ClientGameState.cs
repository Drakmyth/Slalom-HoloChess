using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using DejarikLibrary;
using Assets.Scripts.Monsters;
using UnityEngine.SceneManagement;
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
        public GameObject PushResultTextPrefab;
        public GameObject KillResultTextPrefab;
        public GameObject CounterPushResultTextPrefab;
        public GameObject CounterKillResultTextPrefab;

        public List<AudioClip> AttackSounds;
        public List<AudioClip> MovementSounds;

        public void Init(Client client, List<int> friendlyMonsterIds, List<int> enemyMonsterIds)
        {
            Client = client;

            DontDestroyOnLoad(gameObject);

            _actionNumber = 1;

            if (!Client.IsHost)
            {
                _actionNumber = 3;
            }

            _subActionNumber = 1;

            GameGraph = new BoardGraph();
            BoardSpaces = new Dictionary<int, BoardSpace>();
            FriendlyMonsters = new List<Monster>();
            EnemyMonsters = new List<Monster>();
            AttackCalculator = new AttackCalculator();
            MoveCalculator = new MoveCalculator();

            AvailablePushDestinations = new List<Node>();

            List<Monster> friendlyMonsters = MonsterPrefabs.Where(m => friendlyMonsterIds.Contains(m.MonsterTypeId)).ToList();
            List<Monster> enemyMonsters = MonsterPrefabs.Where(m => enemyMonsterIds.Contains(m.MonsterTypeId)).ToList();

            DisplayBoardSpaces();
            DisplayMonsters(friendlyMonsters, enemyMonsters);

        }

        void Update()
        {      
            if (Client == null || _actionNumber < 1 || _isAnimationRunning)
            {
                return;
            }

            if (!FriendlyMonsters.Any() || !EnemyMonsters.Any())
            {
                _actionNumber = 0;
            }

            switch (_subActionNumber)
            {
                case 1:
                    SubActionOne();
                    break;
                case 2:
                    //Wait for user to select from available actions
                    return;
//TODO: Net remove?                    AwaitSubActionTwoSelection();
//                    break;
                case 3:
                    UpdateSelectionMenu();
                    SubActionThree();
                    break;
                case 4:
                    //Wait for user to select from available actions
                    return;
                    //TODO: Net remove?                    AwaitSubActionFourSelection();
                    //break;
                case 5:
                    SubActionFive();
                    break;
                case 6:
                    AwaitSubActionSixSelection();
                    break;
                case 7:
                    AwaitSubActionSevenSelection();
                    break;

            }

            if (_actionNumber == 4 && _subActionNumber == 0)
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnClearHighlighting");
                }
                _actionNumber = 1;
                _subActionNumber = 1;
                SelectedMonster = null;
                SelectedAttackNode = null;
                SelectedMovementPath = null;

            }
            else if (_actionNumber == 3 && _subActionNumber == 0)
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnClearHighlighting");
                }
                _actionNumber ++;
                _subActionNumber = 1;
               
                SelectedMonster = null;
                SelectedAttackNode = null;
                SelectedMovementPath = null;
            }
            else if(_actionNumber == 2 && _subActionNumber == 0)
            {
                ClearSelectionMenu();
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnClearHighlighting");
                }
                _actionNumber++;
                _subActionNumber = 1;
                SelectedMonster = null;
                SelectedAttackNode = null;
                SelectedMovementPath = null;
            }
            else if (_actionNumber == 1 && _subActionNumber == 0)
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    Node selectedNode = null;

                    if (SelectedMonster != null)
                    {
                        selectedNode = SelectedMonster.CurrentNode;
                        _subActionNumber = 3;
                        IEnumerable<BoardSpace> availableSpaces =
                            BoardSpaces.Values.Where(s => FriendlyMonsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();
                        space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
                    }
                    else
                    {
                        ClearSelectionMenu();
                        space.SendMessage("OnClearHighlighting");
                        _subActionNumber = 1;
                    }

                    space.SendMessage("OnClearHighlightingWithSelection", selectedNode);
                }

                SelectedAttackNode = null;
                SelectedMovementPath = null;

                _actionNumber++;

            }
        }

        void OnSpaceSelected(int nodeId)
        {
            if (_actionNumber == 1 || _actionNumber == 2)
            {
                Node selectedNode = GameGraph.Nodes[nodeId];

                if (_subActionNumber == 2)
                {
                    SubActionTwo(nodeId);
                }

                if (_subActionNumber == 4)
                {
                    SubActionFour(selectedNode);
                }

                if (_subActionNumber == 6 && AvailablePushDestinations.Any(apd => apd.Id == nodeId))
                {
                    SubActionSix(selectedNode);
                }

                if (_subActionNumber == 7 && AvailablePushDestinations.Any(apd => apd.Id == nodeId))
                {
                    SubActionSeven(selectedNode);
                }

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
        }

        private void DisplayBoardSpaces()
        {

            for(int i = 0; i < SpacePrefabs.Count; i ++)
            {
                BoardSpace spacePrefab = SpacePrefabs[i];
                float yAngleOffset = 30 * ((i - 1) % 12);
                Quaternion spaceQuaternion = Quaternion.Euler(spacePrefab.transform.rotation.eulerAngles.x, spacePrefab.transform.rotation.eulerAngles.y + yAngleOffset, spacePrefab.transform.rotation.eulerAngles.z);
                if (!BoardSpaces.ContainsKey(i))
                {
                    BoardSpace space =
                        Instantiate(spacePrefab,
                            new Vector3(spacePrefab.transform.position.x, spacePrefab.transform.position.y -.005f,
                                spacePrefab.transform.position.z), spaceQuaternion) as BoardSpace;
                    if (space != null)
                    {
                        space.Node = GameGraph.Nodes[i];

                        BoardSpaces.Add(i, space);
                    }
                }
            }

        }

        //TODO: do we even need to instantiate here? We could just as well reposition them.
        private void DisplayMonsters(List<Monster> friendlyMonsters, List<Monster> enemyMonsters)
        {
            foreach (Monster monster in friendlyMonsters)
            {
                var monsterQuaternion = Quaternion.Euler(monster.transform.rotation.eulerAngles.x, monster.transform.rotation.eulerAngles.y + 180, monster.transform.rotation.eulerAngles.z);
                Monster monsterInstance =
                    Instantiate(monster,
                        new Vector3(monster.CurrentNode.XPosition, 0, monster.CurrentNode.YPosition),
                        monsterQuaternion) as Monster;
                if (monsterInstance != null)
                {
                    monsterInstance.BelongsToHost = true;
                    monsterInstance.CurrentNode = monster.CurrentNode;
                    FriendlyMonsters.Add(monsterInstance);
                }
            }

            foreach (Monster monster in enemyMonsters)
            {
                Monster monsterInstance =
                    Instantiate(monster,
                        new Vector3(monster.CurrentNode.XPosition, 0, monster.CurrentNode.YPosition),
                        monster.transform.rotation) as Monster;
                if (monsterInstance != null)
                {
                    monsterInstance.BelongsToHost = false;
                    monsterInstance.CurrentNode = monster.CurrentNode;
                    FriendlyMonsters.Add(monsterInstance);
                }
            }


        }

        private void ProcessAttackAction(Monster attacker, Monster defender)
        {
            Quaternion battleSmokeQuaternion = Quaternion.Euler(BattleSmoke.transform.rotation.eulerAngles.x, BattleSmoke.transform.rotation.eulerAngles.y, BattleSmoke.transform.rotation.eulerAngles.z);
            Vector3 battleSmokePosition = new Vector3((attacker.CurrentNode.XPosition + defender.CurrentNode.XPosition)/2f, 0, (attacker.CurrentNode.YPosition + defender.CurrentNode.YPosition) / 2f);
            GameObject battleSmokeInstance = Instantiate(BattleSmoke, battleSmokePosition, battleSmokeQuaternion) as GameObject;


            IEnumerable<Node> friendlyOccupiedNodes = FriendlyMonsters.Select(monster => monster.CurrentNode).ToList();
            IEnumerable<Node> enemyOccupiedNodes = EnemyMonsters.Select(monster => monster.CurrentNode).ToList();

            attacker.SendMessage("OnBeginBattle", defender.CurrentNode);
            defender.SendMessage("OnBeginBattle", attacker.CurrentNode);

            int number = _random.Next(0, AttackSounds.Count);
            int number2 = _random.Next(0, AttackSounds.Count);
            attacker.PlaySound(AttackSounds[number]);
            defender.PlaySound(AttackSounds[number2]);

            //TODO: Net awaitResponse
            bool isFriendlyMonster = _actionNumber == 1 || _actionNumber == 2;
            AttackResult attackResult = AttackCalculator.Calculate(attacker.AttackRating, defender.DefenseRating);
            switch (attackResult)
            {
                case AttackResult.Kill:
                    AttackResultText attackKillResultText = Instantiate(KillResultTextPrefab as UnityEngine.Object, battleSmokePosition,
                        battleSmokeQuaternion) as AttackResultText;

                    if (attackKillResultText != null)
                    {
                        attackKillResultText.SendMessage("OnActivate", battleSmokePosition);
                    }

                    ProcessKill(defender, !isFriendlyMonster, battleSmokeInstance);
                    break;
                case AttackResult.CounterKill:
                    AttackResultText attackCounterKillResultText = Instantiate(CounterKillResultTextPrefab as UnityEngine.Object, battleSmokePosition,
                        battleSmokeQuaternion) as AttackResultText;


                    if (attackCounterKillResultText != null)
                    {
                    attackCounterKillResultText.LerpDestination = attackCounterKillResultText.transform.position + Vector3.up;
                        attackCounterKillResultText.SendMessage("OnActivate", battleSmokePosition);
                    }

                    ProcessKill(attacker, isFriendlyMonster, battleSmokeInstance);
                    SelectedMonster = null;
                    break;
                case AttackResult.Push:

                    AvailablePushDestinations = MoveCalculator.FindMoves(defender.CurrentNode, 1,
                        friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(m => m.DestinationNode);

                    AttackResultText attackPushResultText = Instantiate(PushResultTextPrefab as UnityEngine.Object, battleSmokePosition,
                        battleSmokeQuaternion) as AttackResultText;


                    if (attackPushResultText != null)
                    {
                        attackPushResultText.LerpDestination = attackPushResultText.transform.position + Vector3.up;
                        attackPushResultText.SendMessage("OnActivate", battleSmokePosition);
                    }

                    foreach (BoardSpace space in BoardSpaces.Values)
                    {
                        space.SendMessage("OnAvailableMoves", AvailablePushDestinations.Select(n => n.Id));
                    }

                    Destroy(battleSmokeInstance);

                    _subActionNumber = 6;

                    if (!AvailablePushDestinations.Any())
                    {
                        _subActionNumber = 0;
                    }

                    break;
                case AttackResult.CounterPush:

                    AttackResultText attackCounterPushResultText = Instantiate(CounterPushResultTextPrefab as UnityEngine.Object, battleSmokePosition,
                        battleSmokeQuaternion) as AttackResultText;

                    if (attackCounterPushResultText != null)
                    {
                        attackCounterPushResultText.LerpDestination = attackCounterPushResultText.transform.position + Vector3.up;
                        attackCounterPushResultText.SendMessage("OnActivate", battleSmokePosition);
                    }

                    AvailablePushDestinations = MoveCalculator.FindMoves(attacker.CurrentNode, 1,
                        friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(m => m.DestinationNode);

                    foreach (BoardSpace space in BoardSpaces.Values)
                    {
                        space.SendMessage("OnAvailableMoves", AvailablePushDestinations.Select(n => n.Id));
                    }

                    //TODO: wait until push action is complete
                    Destroy(battleSmokeInstance);

                    _subActionNumber = 7;
                    //send network message with available push nodes

                    if (!AvailablePushDestinations.Any())
                    {
                        _subActionNumber = 0;
                    }


                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void ProcessKill(Monster killed, bool isFriendlyMonster, GameObject battleSmokeInstance)
        {
            //TODO: Net wait for verification
//            MonsterPrefabs.Remove(killed);
            if (isFriendlyMonster)
            {
                FriendlyMonsters.Remove(killed);
            }
            else
            {
                EnemyMonsters.Remove(killed);
            }
            _isAnimationRunning = true;
            killed.SendMessage("OnLoseBattle", battleSmokeInstance);
            _subActionNumber = 0;
        }

        private void ProcessMoveAction(Monster selectedMonster, NodePath path)
        {
            int number = _random.Next(0, MovementSounds.Count);
            selectedMonster.PlaySound(MovementSounds[number]);

            selectedMonster.CurrentNode = GameGraph.Nodes[path.DestinationNode.Id];

            _isAnimationRunning = true;

            selectedMonster.SendMessage("OnBeginMoveAnimation", path);

            _subActionNumber = 0;
        }

        private void SubActionOne()
        {
            IEnumerable<BoardSpace> availableSpaces =
            BoardSpaces.Values.Where(s => FriendlyMonsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();

            foreach (BoardSpace space in availableSpaces)
            {
                space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
            }

            _subActionNumber = 2;
        }

        /*
        private void AwaitSubActionTwoSelection()
        {
            if (_actionNumber == 3 || _actionNumber == 4)
            {
                IEnumerable<BoardSpace> availableSpaces =
                    BoardSpaces.Values.Where(s => Player2Monsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();
                //TODO: bake this into gonk droid et al
                if (availableSpaces.Any())
                {
                    BoardSpace aiChoice = availableSpaces.ElementAt(_random.Next(availableSpaces.Count()));

                    OnSpaceSelected(aiChoice.Node.Id);

                }
            }

        }
        */

        private void SubActionTwo(int nodeId)
        {

            //TODO: Net request
            SelectedMonster = FriendlyMonsters.SingleOrDefault(m => m.CurrentNode.Id == nodeId);


            //TODO: Net onResponse
            if (SelectedMonster != null)
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnMonsterSelected", nodeId);
                }
                _subActionNumber = 3;
            }
        }

        private void SubActionThree()
        {

            //TODO: Net awaitResponse
            if (SelectedMonster != null)
            {
                IEnumerable<Node> friendlyOccupiedNodes;
                IEnumerable<Node> enemyOccupiedNodes;

                friendlyOccupiedNodes = FriendlyMonsters.Select(monster => monster.CurrentNode).ToList();
                enemyOccupiedNodes = EnemyMonsters.Select(monster => monster.CurrentNode).ToList();

                IEnumerable<int> availableMoveActionNodeIds = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                    SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(a => a.DestinationNode.Id);

                IEnumerable<int> availableAttackActionNodeIds = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                    enemyOccupiedNodes).Select(a => a.Id);

                //Update board highlighting
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnAvailableAttacks", availableAttackActionNodeIds);
                    space.SendMessage("OnAvailableMoves", availableMoveActionNodeIds);
                }

                _subActionNumber = 4;
            }

        }

        private void AwaitSubActionFourSelection()
        {
            /*
            if (_actionNumber == 3 || _actionNumber == 4)
            {
                IEnumerable<Node> friendlyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();
                IEnumerable<Node> enemyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();

                IEnumerable<int> availableMoveActionNodeIds = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                    SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(a => a.DestinationNode.Id);

                IEnumerable<int> availableAttackActionNodeIds = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                    enemyOccupiedNodes).Select(a => a.Id);
                //TODO: bake this into gonk droid et al
                int aiActionNodeId;
                if (availableAttackActionNodeIds.Any())
                {
                    aiActionNodeId = availableAttackActionNodeIds.ElementAt(_random.Next(availableAttackActionNodeIds.Count()));
                }
                else if (availableMoveActionNodeIds.Any())
                {
                    aiActionNodeId = availableMoveActionNodeIds.ElementAt(_random.Next(availableMoveActionNodeIds.Count()));
                }
                else
                {
                    _subActionNumber = 2;
                    return;
                }

                OnSpaceSelected(aiActionNodeId);

            }
            */
        }

        private void SubActionFour(Node selectedNode)
        {

            //TODO: Net awaitResponse
            if (SelectedMonster == null)
            {
                _subActionNumber = 2;
                return;
            }

            IEnumerable<Node> friendlyOccupiedNodes;
            IEnumerable<Node> enemyOccupiedNodes;

            friendlyOccupiedNodes = FriendlyMonsters.Select(monster => monster.CurrentNode).ToList();
            enemyOccupiedNodes = EnemyMonsters.Select(monster => monster.CurrentNode).ToList();

            IEnumerable<NodePath> movementPaths = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes));

            IEnumerable<Node> availableMoveActions = movementPaths.Select(p => p.DestinationNode);

            IEnumerable<Node> availableAttackActions = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                enemyOccupiedNodes);

            if (friendlyOccupiedNodes.Contains(selectedNode))
            {
                SelectedMonster = FriendlyMonsters.Single(m => m.CurrentNode.Equals(selectedNode));
                SelectedAttackNode = null;

                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnMonsterSelected", selectedNode.Id);
                }
                _subActionNumber = 3;
            }
            else if (availableAttackActions.Contains(selectedNode))
            {
                SelectedAttackNode = selectedNode;
                SelectedMovementPath = null;
                _subActionNumber = 5;
            }
            else if (availableMoveActions.Contains(selectedNode))
            {
                SelectedMovementPath = movementPaths.Single(m => m.DestinationNode.Equals(selectedNode));
                SelectedAttackNode = null;
                _subActionNumber = 5;
            }

        }

        private void SubActionFive()
        {
            if (SelectedMonster == null)
            {
                _subActionNumber = 2;
                return;
            }

            //TODO: Net awaitResult

            if (SelectedAttackNode != null)
            {
                Monster opponent = EnemyMonsters.FirstOrDefault(monster => monster.CurrentNode.Equals(SelectedAttackNode));

                ProcessAttackAction(SelectedMonster, opponent);
            }
            else if (SelectedMovementPath != null)
            {
                ProcessMoveAction(SelectedMonster, SelectedMovementPath);
            }

        }

        private void AwaitSubActionSixSelection()
        {
            /*
            if (_actionNumber == 3 || _actionNumber == 4)
            {               
                //TODO: bake this into gonk droid et al
                Node aiActionNode;
                if (AvailablePushDestinations.Any())
                {
                    aiActionNode = AvailablePushDestinations.ElementAt(_random.Next(AvailablePushDestinations.Count()));
                }
                else
                {
                    _subActionNumber = 0;
                    return;
                }

                OnSpaceSelected(aiActionNode.Id);

            }
            */
        }



        private void SubActionSix(Node selectedNode)
        {
            Monster pushedMonster = EnemyMonsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id);

            List<Node> pathToDestination = new List<Node> { selectedNode };
            NodePath movementPath = new NodePath(pathToDestination, selectedNode);
            _isAnimationRunning = true;
            pushedMonster.SendMessage("OnBeginMoveAnimation", movementPath);

            pushedMonster.CurrentNode = selectedNode;

            _subActionNumber = 0;

        }


        private void AwaitSubActionSevenSelection()
        {
            //TODO: Net awaitResult
            /*
            if (_actionNumber == 1 || _actionNumber == 2)
            {

                //TODO: bake this into gonk droid et al
                Node aiActionNode;
                if (AvailablePushDestinations.Any())
                {
                    aiActionNode = AvailablePushDestinations.ElementAt(_random.Next(AvailablePushDestinations.Count()));
                }
                else
                {
                    _subActionNumber = 0;
                    return;
                }

                OnSpaceSelected(aiActionNode.Id);

            }
            */
        }



        private void SubActionSeven(Node selectedNode)
        {
            Monster pushedMonster = EnemyMonsters.Single(m => m.CurrentNode.Id == SelectedMonster.CurrentNode.Id);

            List<Node> pathToDestination = new List<Node> { selectedNode };
            NodePath movementPath = new NodePath(pathToDestination, selectedNode);
            _isAnimationRunning = true;
            pushedMonster.SendMessage("OnBeginMoveAnimation", movementPath);

            pushedMonster.CurrentNode = selectedNode;

            _subActionNumber = 0;

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



        private void EndGameWin()
        {
            SceneManager.LoadScene("wingame");
        }

        private void EndGameLose()
        {
            SceneManager.LoadScene("losegame");
        }
    }

}