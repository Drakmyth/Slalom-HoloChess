using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.MessageModels;
using DejarikLibrary;
using Assets.Scripts.Monsters;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Assets.Scripts
{

    public class GameState: MonoBehaviour
    {
        public BoardGraph GameGraph { get; set; }
        public AttackCalculator AttackCalculator { get; set; }
        public MoveCalculator MoveCalculator { get; set; }
        public List<Monster> HostMonsters { get; set; }
        public List<Monster> GuestMonsters { get; set; } 

        private readonly Random _random;

        //TODO: Net
        private Server _hostServer;
        public bool IsGameStateUpdated = false;

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
        
        private Monster SelectedMonster { get; set; }
        private Node SelectedAttackNode { get; set; }
        private NodePath SelectedMovementPath { get; set; }

        //TODO: we can probably do better than this
        private IEnumerable<Node> AvailablePushDestinations { get; set; }

        public GameState()
        {
            _random = new Random();

        }

        void Start()
        {
            _actionNumber = 1;
            _subActionNumber = 1;

            GameGraph = new BoardGraph();
            HostMonsters = new List<Monster>();
            GuestMonsters = new List<Monster>();
            AttackCalculator = new AttackCalculator();
            MoveCalculator = new MoveCalculator();

            AvailablePushDestinations = new List<Node>();

            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            if (gameManager == null || gameManager.Server == null)
            {
                throw new InvalidOperationException("Server must exist to begin game");
            }

            _hostServer = gameManager.Server;

            if (_hostServer == null || !_hostServer.IsServerStarted)
            {
                throw new InvalidOperationException("Server must be running to begin game");
            }

            if (SceneManager.GetSceneByName("startup").isLoaded)
            {
                SceneManager.UnloadSceneAsync("startup");
            }

            if (SceneManager.GetSceneByName("lobby").isLoaded)
            {
                SceneManager.UnloadSceneAsync("lobby");
            }

            AssignMonstersToPlayers();

            GameStartMessage gameStartMessage = new GameStartMessage(HostMonsters, GuestMonsters);

            _hostServer.SendToAll(gameStartMessage);

        }

        void Update()
        {

            if (_hostServer == null || _actionNumber < 1)
            {
                //The game should end at this point
                return;
            }

            if (HostMonsters.Count == 0)
            {
                _actionNumber = 0;
                //TODO: Net sendResponse EndGame

            }
            else if (GuestMonsters.Count == 0)
            {
                _actionNumber = 0;
                //TODO: Net sendResponse EndGame

            }


            switch (_subActionNumber)
            {
                case 1:
                    SubActionOne();
                    break;
                case 2:
                    //Wait for user to select from available actions
                    //TODO: Net awaitResponse
                    return;
                case 3:
                    SubActionThree();
                    break;
                case 4:
                    //Wait for user to select from available actions
                    //TODO: Net awaitResponse
                    break;
                case 5:
                    SubActionFive();
                    break;
                case 6:
                    //Wait for user to select push destination
                    //TODO: Net awaitResponse
                    break;
                case 7:
                    //Wait for user to select counter push destination
                    //TODO: Net awaitResponse
                    break;

            }

            if (_actionNumber == 4 && _subActionNumber == 0)
            {
                _actionNumber = 1;
                _subActionNumber = 1;
                SelectedMonster = null;
                SelectedAttackNode = null;
                SelectedMovementPath = null;

            }
            else if (_actionNumber == 3 && _subActionNumber == 0)
            {
                _actionNumber ++;
                _subActionNumber = 1;
               
                SelectedMonster = null;
                SelectedAttackNode = null;
                SelectedMovementPath = null;
            }
            else if(_actionNumber == 2 && _subActionNumber == 0)
            {
                _actionNumber++;
                _subActionNumber = 1;
                SelectedMonster = null;
                SelectedAttackNode = null;
                SelectedMovementPath = null;
            }
            else if (_actionNumber == 1 && _subActionNumber == 0)
            {

                _subActionNumber = SelectedMonster != null ? 3 : 1;

                SelectedAttackNode = null;
                SelectedMovementPath = null;

                _actionNumber++;

            }
        }

        private Monster GetEnemyAtNode(Node node, bool isHostPlayer)
        {
            List<Monster> enemyMonsters = isHostPlayer ? GuestMonsters : HostMonsters;

            return enemyMonsters.FirstOrDefault(monster => monster.CurrentNode.Equals(node));
        }

        private void AssignMonstersToPlayers()
        {
            List<Node> hostStartingNodes = new List<Node>
            {
                GameGraph.Nodes[23],
                GameGraph.Nodes[24],
                GameGraph.Nodes[13],
                GameGraph.Nodes[14]
            };

            List<Node> guestStartingNodes = new List<Node>
            {
                GameGraph.Nodes[17],
                GameGraph.Nodes[18],
                GameGraph.Nodes[19],
                GameGraph.Nodes[20]
            };

            List<Monster> availableMonsters = new List<Monster>
            {
                new Ghhhk(),
                new Houjix(),
                new Klorslug(),
                new Molator(),
                new Monnok(),
                new Ngok(),
                new Savrip(),
                new Strider()
            };

            while (availableMonsters.Any())
            {
                int monsterIndex = _random.Next(0, availableMonsters.Count);
                   
                Monster currentMonster = availableMonsters[monsterIndex];

                if(availableMonsters.Count % 2 == 0)
                {
                    currentMonster.CurrentNode = hostStartingNodes[0];
                    hostStartingNodes.RemoveAt(0);
                    currentMonster.BelongsToHost = true;
                    HostMonsters.Add(currentMonster);
                }
                else
                {
                    currentMonster.CurrentNode = guestStartingNodes[0];
                    guestStartingNodes.RemoveAt(0);
                    currentMonster.BelongsToHost = false;
                    GuestMonsters.Add(currentMonster);
                }

                availableMonsters.RemoveAt(monsterIndex);

            }

        }

        private void ProcessAttackAction(Monster attacker, Monster defender, bool isHostAttacker)
        {
            AttackResult attackResult = AttackCalculator.Calculate(attacker.AttackRating, defender.DefenseRating);
            IEnumerable<Node> friendlyOccupiedNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
            IEnumerable<Node> enemyOccupiedNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();

            switch (attackResult)
            {
                case AttackResult.Kill:
                    ProcessKill(defender, !isHostAttacker);
                    break;
                case AttackResult.CounterKill:
                    ProcessKill(attacker, isHostAttacker);
                    SelectedMonster = null;
                    break;
                case AttackResult.Push:

                    AvailablePushDestinations = MoveCalculator.FindMoves(defender.CurrentNode, 1,
                        friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(m => m.DestinationNode);

                    _subActionNumber = 6;

                    if (!AvailablePushDestinations.Any())
                    {
                        _subActionNumber = 0;
                    }

                    break;
                case AttackResult.CounterPush:

                    AvailablePushDestinations = MoveCalculator.FindMoves(attacker.CurrentNode, 1,
                        friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(m => m.DestinationNode);

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

        private void ProcessKill(Monster killed, bool belongsToHost)
        {
            if (belongsToHost)
            {
                HostMonsters.Remove(killed);
            }
            else
            {
                GuestMonsters.Remove(killed);
            }
            _subActionNumber = 0;
        }

        private void ProcessMoveAction(Monster selectedMonster, NodePath path)
        {
            selectedMonster.CurrentNode = GameGraph.Nodes[path.DestinationNode.Id];

            _subActionNumber = 0;
        }

        //TODO: Net awaitRequest
        private void SubActionOne()
        {
            IEnumerable<int> availableNodeIds =
            GameGraph.Nodes.Where(n => HostMonsters.Select(m => m.CurrentNode.Id).Contains(n.Id)).Select(n => n.Id);

            _subActionNumber = 2;
            //TODO: Net sendResponse AvailableSpaces | subActionOneResponse

        }

        //TODO: Net awaitRequest
        private void SubActionThree()
        {
            if (SelectedMonster != null)
            {
                IEnumerable<Node> friendlyOccupiedNodes;
                IEnumerable<Node> enemyOccupiedNodes;

                if (_actionNumber == 1 || _actionNumber == 2)
                {
                    friendlyOccupiedNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
                    enemyOccupiedNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();
                }
                else
                {
                    friendlyOccupiedNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();
                    enemyOccupiedNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
                }


                IEnumerable<int> availableMoveActionNodeIds = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                    SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(a => a.DestinationNode.Id);

                IEnumerable<int> availableAttackActionNodeIds = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                    enemyOccupiedNodes).Select(a => a.Id);

                _subActionNumber = 4;

                //TODO: Net sendResponse availableActions | subActionThreeResponse
            }

        }

        //TODO: Net awaitRequest
        private void SubActionFour(Node selectedNode, bool isHostPlayer)
        {
            if (SelectedMonster == null)
            {
                _subActionNumber = 2;
                return;
            }

            IEnumerable<Node> friendlyOccupiedNodes;
            IEnumerable<Node> enemyOccupiedNodes;
            if (isHostPlayer)
            {
                friendlyOccupiedNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
                enemyOccupiedNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();
            }
            else
            {
                friendlyOccupiedNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();
                enemyOccupiedNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
            }

            IEnumerable<NodePath> movementPaths = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes));

            IEnumerable<Node> availableMoveActions = movementPaths.Select(p => p.DestinationNode);

            IEnumerable<Node> availableAttackActions = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                enemyOccupiedNodes);

            if (friendlyOccupiedNodes.Contains(selectedNode))
            {
                SelectedMonster = HostMonsters.Single(m => m.CurrentNode.Equals(selectedNode));
                SelectedAttackNode = null;
                
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

            //TODO: Net sendResponse stateUpdated

        }

        private void SubActionFive()
        {
            bool isHostPlayer = _actionNumber == 1 || _actionNumber == 2;

            if (SelectedMonster == null)
            {
                _subActionNumber = 2;
                return;
            }

            if (SelectedAttackNode != null)
            {
                Monster opponent = GetEnemyAtNode(SelectedAttackNode, isHostPlayer);

                ProcessAttackAction(SelectedMonster, opponent, isHostPlayer);
            }
            else if (SelectedMovementPath != null)
            {
                ProcessMoveAction(SelectedMonster, SelectedMovementPath);
            }

        }


        private void SubActionSix(Node selectedNode, bool isHostPlayer)
        {
            Monster pushedMonster = isHostPlayer ? GuestMonsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id) : HostMonsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id);

            List<Node> pathToDestination = new List<Node> { selectedNode };
            NodePath movementPath = new NodePath(pathToDestination, selectedNode);

            pushedMonster.CurrentNode = selectedNode;

            _subActionNumber = 0;

        }


        private void SubActionSeven(Node selectedNode, bool isHostPlayer)
        {
            Monster pushedMonster = isHostPlayer ? GuestMonsters.Single(m => m.CurrentNode.Id == SelectedMonster.CurrentNode.Id) : HostMonsters.Single(m => m.CurrentNode.Id == SelectedMonster.CurrentNode.Id);

            List<Node> pathToDestination = new List<Node> { selectedNode };
            NodePath movementPath = new NodePath(pathToDestination, selectedNode);

            pushedMonster.CurrentNode = selectedNode;

            _subActionNumber = 0;

        }

    }

}