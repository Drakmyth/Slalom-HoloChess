using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using DejarikLibrary;
using Assets.Scripts.Monsters;
using Random = System.Random;

namespace Assets.Scripts
{

    public class GameState: MonoBehaviour
    {
        public BoardGraph GameGraph { get; set; }
        public AttackCalculator AttackCalculator { get; set; }
        public MoveCalculator MoveCalculator { get; set; }
        public Dictionary<int, BoardSpace> BoardSpaces { get; set; }
        public List<Monster> Player1Monsters { get; set; }
        public List<Monster> Player2Monsters { get; set; } 


        private readonly Random _random;
        //3, 4 are opponent actions, 1, 2 are player actions
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

        private bool _isHostPlayer = true;
        private bool _isEasyAI = true;
        private bool _isAnimationRunning = false;
        private Monster SelectedMonster { get; set; }
        private Node SelectedAttackNode { get; set; }
        private NodePath SelectedMovementPath { get; set; }

        //TODO: we can probably do better than this
        private IEnumerable<Node> AvailablePushDestinations { get; set; }

        public List<Monster> MonsterPrefabs;
        public List<BoardSpace> SpacePrefabs;
        public GameObject BattleSmoke;

        public GameState()
        {
            _random = new Random();

            //TODO:needs to be set by whatever function determines who starts
            _actionNumber = 1;
            _subActionNumber = 1;

            GameGraph = new BoardGraph();
            BoardSpaces = new Dictionary<int, BoardSpace>();
            Player1Monsters = new List<Monster>();
            Player2Monsters = new List<Monster>();
            AttackCalculator = new AttackCalculator();
            MoveCalculator = new MoveCalculator();

            AvailablePushDestinations = new List<Node>();
        }


        void Start()
        {
            DisplayBoardSpaces();

            if (_isHostPlayer)
            {
                AssignMonstersToPlayers();
            }

        }

        void Update()
        {

            if (_actionNumber < 1)
            {
                //This should be unreachable
                return;
            }

            if (_isAnimationRunning)
            {
                return;
            }

            if (_actionNumber == 1 || _actionNumber == 2)
            {
                if (_subActionNumber == 1)
                {
                    IEnumerable<BoardSpace> availableSpaces =
                        BoardSpaces.Values.Where(s => Player1Monsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();

                    foreach (BoardSpace space in availableSpaces)
                    {
                        space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
                    }

                    _subActionNumber = 2;
                }

                if (_subActionNumber == 2 && SelectedMonster == null)
                {
                    //Wait for user to select from available monsters
                    return;
                }

                if (_subActionNumber == 3)
                {
                    if (SelectedMonster != null)
                    {
                        IEnumerable<Node> friendlyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();
                        IEnumerable<Node> enemyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();

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

                if (_subActionNumber == 4)
                {
                    //Wait for user to select from available actions
                    return;
                }

                if (_subActionNumber == 5)
                {
                    if (SelectedMonster == null)
                    {
                        _subActionNumber = 2;
                        return;
                    }

                    if (SelectedAttackNode != null)
                    {

                        Monster opponent = GetEnemyAtNode(SelectedAttackNode, true);

                        ProcessAttackAction(SelectedMonster, opponent, true);
                    }
                    else if (SelectedMovementPath != null)
                    {
                        ProcessMoveAction(SelectedMonster, SelectedMovementPath);
                    }
                }
            }

            if ((_actionNumber == 3 || _actionNumber == 4) && _isEasyAI)
            {
                ProcessAIUpdate();
            }


            if (Player1Monsters.Count == 0)
            {
                EndGameWin();
            } else if (Player2Monsters.Count == 0)
            {
                EndGameLose();
            }

            if (_actionNumber == 4 && _subActionNumber == 0)
            {
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnClearHighlighting");
                }
                _actionNumber = 1;
                _subActionNumber = 1;
                _isHostPlayer = true;
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
                foreach (BoardSpace space in BoardSpaces.Values)
                {
                    space.SendMessage("OnClearHighlighting");
                }
                _actionNumber++;
                _subActionNumber = 1;
                _isHostPlayer = false;
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
                            BoardSpaces.Values.Where(s => Player1Monsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();
                        space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
                    }
                    else
                    {
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

            Node selectedNode = GameGraph.Nodes[nodeId];

            if (_actionNumber == 1 || _actionNumber == 2)
            {
                if (_subActionNumber == 2)
                {
                    SelectedMonster = Player1Monsters.SingleOrDefault(m => m.CurrentNode.Id == nodeId);
                    if (SelectedMonster != null)
                    {
                        foreach (BoardSpace space in BoardSpaces.Values)
                        {
                            space.SendMessage("OnMonsterSelected", nodeId);
                        }
                        _subActionNumber = 3;
                    }

                }

                if (_subActionNumber == 4)
                {
                    if (SelectedMonster == null)
                    {
                        _subActionNumber  = 2;
                        return;
                    }

                    IEnumerable<Node> friendlyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();
                    IEnumerable<Node> enemyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();

                    IEnumerable<NodePath> movementPaths = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                        SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes));

                    IEnumerable<Node> availableMoveActions = movementPaths.Select(p => p.DestinationNode);

                    IEnumerable <Node> availableAttackActions = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                        enemyOccupiedNodes);

                    if (friendlyOccupiedNodes.Contains(selectedNode))
                    {
                        SelectedMonster = Player1Monsters.Single(m => m.CurrentNode.Id == nodeId);
                        SelectedAttackNode = null;

                        foreach (BoardSpace space in BoardSpaces.Values)
                        {
                            space.SendMessage("OnMonsterSelected", nodeId);
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

                if (_subActionNumber == 6 && AvailablePushDestinations.Any(apd => apd.Id == nodeId))
                {
                    Monster pushedMonster = Player2Monsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id);
                    List<Node> pathToDestination = new List<Node> {selectedNode};
                    NodePath movementPath = new NodePath(pathToDestination, selectedNode);
                    _isAnimationRunning = true;
                    pushedMonster.SendMessage("OnBeginMoveAnimation", movementPath);

                    pushedMonster.CurrentNode = selectedNode;

                    _subActionNumber = 0;

                }

            }

            if (_actionNumber == 3 || _actionNumber == 4)
            {
                if (_subActionNumber == 2)
                {
                    SelectedMonster = Player2Monsters.SingleOrDefault(m => m.CurrentNode.Id == nodeId);
                    if (SelectedMonster != null)
                    {
                        foreach (BoardSpace space in BoardSpaces.Values)
                        {
                            space.SendMessage("OnMonsterSelected", nodeId);
                        }
                        _subActionNumber = 3;
                    }

                }

                if (_subActionNumber == 4)
                {
                    if (SelectedMonster == null)
                    {
                        _subActionNumber--;
                        return;
                    }

                    IEnumerable<Node> friendlyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();
                    IEnumerable<Node> enemyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();

                    IEnumerable<NodePath> movementPaths = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                        SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes));

                    IEnumerable<Node> availableMoveActions = movementPaths.Select(p => p.DestinationNode);

                    IEnumerable<Node> availableAttackActions = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                        enemyOccupiedNodes);

                    if (friendlyOccupiedNodes.Contains(selectedNode))
                    {
                        SelectedMonster = Player2Monsters.Single(m => m.CurrentNode.Id == nodeId);
                        SelectedAttackNode = null;

                        foreach (BoardSpace space in BoardSpaces.Values)
                        {
                            space.SendMessage("OnMonsterSelected", nodeId);
                        }

                        _subActionNumber = 3;
                    }
                    else if (availableAttackActions.Contains(selectedNode))
                    {
                        SelectedAttackNode = selectedNode;
                        SelectedMovementPath = null;
                        _subActionNumber = 5;
                    } else if (availableMoveActions.Contains(selectedNode))
                    {
                        SelectedMovementPath = movementPaths.Single(m => m.DestinationNode.Equals(selectedNode));
                        SelectedAttackNode = null;
                        _subActionNumber = 5;
                    }

                }

                if (_subActionNumber == 6 && AvailablePushDestinations.Any(apd => apd.Id == nodeId))
                {
                    Player2Monsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id).CurrentNode = selectedNode;
                    _subActionNumber = 0;

                }
                if (_subActionNumber == 7)
                {
                    //if result from opponent exists, push SelectedMonster
                    _subActionNumber = 0;
                }

            }

        }

        void OnAnimationComplete()
        {
            _isAnimationRunning = false;
        }

        private Monster GetEnemyAtNode(Node node, bool isHostPlayer)
        {
            List<Monster> enemyMonsters = isHostPlayer ? Player2Monsters : Player1Monsters;

            return enemyMonsters.FirstOrDefault(monster => monster.CurrentNode.Equals(node));
        }

        private void AssignMonstersToPlayers()
        {
            List<Node> player1StartingNodes = new List<Node>
            {
                GameGraph.Nodes[23],
                GameGraph.Nodes[24],
                GameGraph.Nodes[13],
                GameGraph.Nodes[14]
            };

            List<Node> player2StartingNodes = new List<Node>
            {
                GameGraph.Nodes[17],
                GameGraph.Nodes[18],
                GameGraph.Nodes[19],
                GameGraph.Nodes[20]
            };

            List<Monster> availableMonsters = new List<Monster>(MonsterPrefabs);
            while (availableMonsters.Any())
            {
                int monsterIndex = _random.Next(0, availableMonsters.Count);
                   
                Monster currentMonster = availableMonsters[monsterIndex];

                Monster monsterPrefab = MonsterPrefabs[MonsterPrefabs.IndexOf(currentMonster)];

                Quaternion monsterQuaternion;

                if(availableMonsters.Count % 2 == 0)
                {
                    currentMonster.CurrentNode = player1StartingNodes[0];
                    player1StartingNodes.RemoveAt(0);
                    monsterQuaternion = Quaternion.Euler(monsterPrefab.transform.rotation.eulerAngles.x, monsterPrefab.transform.rotation.eulerAngles.y + 180, monsterPrefab.transform.rotation.eulerAngles.z); 
                    Monster monsterInstance =
                        Instantiate(monsterPrefab,
                            new Vector3(currentMonster.CurrentNode.XPosition, 0, currentMonster.CurrentNode.YPosition),
                            monsterQuaternion) as Monster;
                    if (monsterInstance != null)
                    {
                        monsterInstance.CurrentNode = currentMonster.CurrentNode;
                        Player1Monsters.Add(monsterInstance);
                    }
                }
                else
                {
                    currentMonster.CurrentNode = player2StartingNodes[0];
                    player2StartingNodes.RemoveAt(0);
                    monsterQuaternion = monsterPrefab.transform.rotation;
                    Monster monsterInstance = 
                        Instantiate(monsterPrefab,
                            new Vector3(currentMonster.CurrentNode.XPosition, 0, currentMonster.CurrentNode.YPosition),
                            monsterQuaternion) as Monster;
                    if (monsterInstance != null)
                    {
                        monsterInstance.CurrentNode = currentMonster.CurrentNode;
                        Player2Monsters.Add(monsterInstance);
                    }
                }


                availableMonsters.RemoveAt(monsterIndex);

            }

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

        private void ProcessAttackAction(Monster attacker, Monster defender, bool isHostAttacker)
        {
            Vector3 battleSmokePosition = new Vector3((attacker.CurrentNode.XPosition + defender.CurrentNode.XPosition)/2f, 0, (attacker.CurrentNode.YPosition + defender.CurrentNode.YPosition) / 2f);
            GameObject battleSmokeInstance = Instantiate(BattleSmoke, battleSmokePosition, Quaternion.identity) as GameObject;

            AttackResult attackResult = AttackCalculator.Calculate(attacker.AttackRating, defender.DefenseRating);
            IEnumerable<Node> friendlyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();
            IEnumerable<Node> enemyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();

            switch (attackResult)
            {
                case AttackResult.Kill:
                    MonsterPrefabs.Remove(defender);
                    if (isHostAttacker)
                    {
                        Player2Monsters.Remove(defender);
                    }
                    else
                    {
                        Player1Monsters.Remove(defender);
                    }
                    _isAnimationRunning = true;
                    defender.SendMessage("OnLoseBattle", battleSmokeInstance);
                    _subActionNumber = 0;
                    break;
                case AttackResult.CounterKill:
                    MonsterPrefabs.Remove(attacker);
                    if (isHostAttacker)
                    {                        
                        Player1Monsters.Remove(attacker);
                    }
                    else
                    {
                        Player2Monsters.Remove(attacker);
                    }
                    _isAnimationRunning = true;
                    attacker.SendMessage("OnLoseBattle", battleSmokeInstance);
                    _subActionNumber = 0;
                    break;
                case AttackResult.Push:
                    //TODO:Movement animation!
                    AvailablePushDestinations = MoveCalculator.FindMoves(defender.CurrentNode, 1,
                        friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(m => m.DestinationNode);
                    //TODO: wait until push action is complete
                    Destroy(battleSmokeInstance);

                    //_subActionNumber = 6;                
                    break;
                case AttackResult.CounterPush:
                    //TODO:Get user input to select which node
                    //TODO:Movement animation!

                    //TODO: wait until push action is complete
                    Destroy(battleSmokeInstance);

                    //_subActionNumber = 7;
                    //send network message with available push nodes

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void ProcessMoveAction(Monster selectedMonster, NodePath path)
        {
            selectedMonster.CurrentNode = GameGraph.Nodes[path.DestinationNode.Id];

            _isAnimationRunning = true;

            selectedMonster.SendMessage("OnBeginMoveAnimation", path);

            _subActionNumber = 0;
        }

        private void ProcessAIUpdate()
        {
            // AI will have reversed friendly/enemy occupied nodes
            if (_subActionNumber == 1)
            {
                IEnumerable<BoardSpace> availableSpaces =
                    BoardSpaces.Values.Where(s => Player2Monsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();

                foreach (BoardSpace space in availableSpaces)
                {
                    space.SendMessage("OnAvailableMonsters", availableSpaces.Select(s => s.Node.Id));
                }

                _subActionNumber = 2;
            }

            if (_subActionNumber == 2 && SelectedMonster == null)
            {
                IEnumerable<BoardSpace> availableSpaces =
                    BoardSpaces.Values.Where(s => Player2Monsters.Select(m => m.CurrentNode.Id).Contains(s.Node.Id)).ToList();
                BoardSpace aiChoice = availableSpaces.ElementAt(_random.Next(availableSpaces.Count()));

                OnSpaceSelected(aiChoice.Node.Id);
                return;
            }

            if (_subActionNumber == 3)
            {
                if (SelectedMonster != null)
                {
                    IEnumerable<Node> friendlyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();
                    IEnumerable<Node> enemyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();

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

            if (_subActionNumber == 4)
            {
                IEnumerable<Node> friendlyOccupiedNodes = Player2Monsters.Select(monster => monster.CurrentNode).ToList();
                IEnumerable<Node> enemyOccupiedNodes = Player1Monsters.Select(monster => monster.CurrentNode).ToList();

                IEnumerable<int> availableMoveActionNodeIds = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                    SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(a => a.DestinationNode.Id);

                IEnumerable<int> availableAttackActionNodeIds = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                    enemyOccupiedNodes).Select(a => a.Id);
                int aiActionNodeId;
                if (availableAttackActionNodeIds.Count() > 0)
                {
                    aiActionNodeId = availableAttackActionNodeIds.ElementAt(_random.Next(availableAttackActionNodeIds.Count()));
                } else if (availableMoveActionNodeIds.Count() > 0)
                {
                    aiActionNodeId = availableMoveActionNodeIds.ElementAt(_random.Next(availableMoveActionNodeIds.Count()));
                } else
                {
                    return;
                }

                OnSpaceSelected(aiActionNodeId);

                return;
            }

            if (_subActionNumber == 5)
            {
                if (SelectedMonster == null)
                {
                    _subActionNumber = 2;
                    return;
                }

                if (SelectedAttackNode != null)
                {
                    Monster opponent = GetEnemyAtNode(SelectedAttackNode, false);

                    ProcessAttackAction(SelectedMonster, opponent, false);
                }
                else if (SelectedMovementPath != null)
                {
                    ProcessMoveAction(SelectedMonster, SelectedMovementPath);
                }
            }
        }

        private void GonkDroidAI()
        {

        }

        private void EndGameWin()
        {
            return;
        }

        private void EndGameLose()
        {
            return;
        }

    }

}