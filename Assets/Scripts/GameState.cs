using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.MessageModels;
using DejarikLibrary;
using Assets.Scripts.Monsters.ServerObjects;
using Newtonsoft.Json;
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

        private readonly Random _random = new Random();

        private Server _hostServer;

        //0 : GameEnded
        //1 : HostAction
        //2 : HostAction
        //3 : GuestAction
        //4 : GuestAction
        public int ActionNumber;


        //TODO: seems a decent candidate for an enum
        //1 : Highlight available monsters
        //2 : Select own monster node (await user input)
        //3 : Get available moves, update highlighting 
        //4 : Select Action node (await user input)
        //5 : Process Action
        //6 : Select Push result (await user input)
        //7 : Listen for CounterPush result (await opponent input)
        public int SubActionNumber;
        
        public Monster SelectedMonster { get; set; }
        public Node SelectedAttackNode { get; set; }
        public NodePath SelectedMovementPath { get; set; }

        //TODO: we can probably do better than this
        public IEnumerable<Node> AvailablePushDestinations { get; set; }

        void Start()
        {
            DontDestroyOnLoad(gameObject);

            ActionNumber = 1;
            SubActionNumber = 1;

            GameGraph = new BoardGraph();
            HostMonsters = new List<Monster>();
            GuestMonsters = new List<Monster>();
            AttackCalculator = new AttackCalculator();
            MoveCalculator = new MoveCalculator();

            AvailablePushDestinations = new List<Node>();

            GameManager gameManager = GameManager.Instance;

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

            AssignMonstersToPlayers();

            GameStartMessage gameStartMessage = new GameStartMessage
            {
                HostMonsters = JsonConvert.SerializeObject(HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id}).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                GuestMonsters = JsonConvert.SerializeObject(GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                ActionNumber = 1,
                SubActionNumber = 1
            };

            _hostServer.SendToAll(gameStartMessage.MessageTypeId, gameStartMessage);

        }

        void Update()
        {
            if (_hostServer == null || ActionNumber < 1 || !_hostServer.ClientsAreReady())
            {
                //The game should end at this point
                return;
            }

            if (HostMonsters.Count == 0)
            {
                ActionNumber = 0;
                _hostServer.SendToAll(CustomMessageTypes.GameEnd, new GameEndMessage
                {
                    IsHostWinner = false,
                    IsGuestWinner = true
                });
            }
            else if (GuestMonsters.Count == 0)
            {
                ActionNumber = 0;
                _hostServer.SendToAll(CustomMessageTypes.GameEnd, new GameEndMessage
                {
                    IsHostWinner = true,
                    IsGuestWinner = false
                });
            }

            switch (SubActionNumber)
            {
                case 0:
                    IncrementActionNumber();
                    break;
                case 1:
                    SubActionOne();
                    break;
                case 2:
                    //Wait for user to select from available actions
                    return;
                case 3:
                    SubActionThree();
                    break;
                case 4:
                    //Wait for user to select from available actions
                    break;
                case 5:
                    SubActionFive();
                    break;
                case 6:
                    //Wait for user to select push destination
                    break;
                case 7:
                    //Wait for user to select counter push destination
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

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
                int monsterIndex = _random.Next(0, availableMonsters.Count - 1);
                   
                Monster currentMonster = availableMonsters[monsterIndex];

                if(availableMonsters.Count % 2 == 0)
                {
                    currentMonster.CurrentNode = hostStartingNodes[0];
                    hostStartingNodes.RemoveAt(0);
                    HostMonsters.Add(currentMonster);
                }
                else
                {
                    currentMonster.CurrentNode = guestStartingNodes[0];
                    guestStartingNodes.RemoveAt(0);
                    GuestMonsters.Add(currentMonster);
                }

                availableMonsters.RemoveAt(monsterIndex);

            }

        }

        public void SelectMonster(int selectedMonsterTypeId)
        {
            if (ActionNumber == 1 || ActionNumber == 2)
            {
                SelectedMonster = HostMonsters.SingleOrDefault(m => m.MonsterTypeId == selectedMonsterTypeId);
            }
            else
            {
                SelectedMonster = GuestMonsters.SingleOrDefault(m => m.MonsterTypeId == selectedMonsterTypeId);
            }

            if (SelectedMonster == null)
            {
                SubActionNumber = 1;
            }
            else
            {
                SubActionNumber = 3;
            }
        }



        public void SelectAction(int selectedNodeId)
        {
            if (selectedNodeId < 0)
            {
                SubActionNumber = 0;
                _hostServer.SendToAll(CustomMessageTypes.GameState, new PassActionMessage
                {
                    ActionNumber = ActionNumber,
                    SubActionNumber = SubActionNumber,
                    Message = "No available moves.",
                    HostMonsterState = JsonConvert.SerializeObject(HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                    GuestMonsterState = JsonConvert.SerializeObject(GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id))
                });
            }

            IEnumerable<Node> friendlyOccupiedNodes;
            IEnumerable<Node> enemyOccupiedNodes;

            List<Monster> friendlyMonsters = ActionNumber == 1 || ActionNumber == 2 ? HostMonsters : GuestMonsters;
            List<Monster> enemyMonsters = ActionNumber == 1 || ActionNumber == 2 ? GuestMonsters : HostMonsters;

            friendlyOccupiedNodes = friendlyMonsters.Select(monster => monster.CurrentNode).ToList();
            enemyOccupiedNodes = enemyMonsters.Select(monster => monster.CurrentNode).ToList();

            IEnumerable<NodePath> movementPaths = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes));

            IEnumerable<Node> availableMoveActions = movementPaths.Select(p => p.DestinationNode);

            IEnumerable<Node> availableAttackActions = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                enemyOccupiedNodes);

            if (friendlyOccupiedNodes.Select(n => n.Id).Contains(selectedNodeId))
            {
                SelectedMonster = friendlyMonsters.Single(m => m.CurrentNode.Id == selectedNodeId);
                SelectedAttackNode = null;

                SubActionNumber = 3;

                _hostServer.SendToAll(CustomMessageTypes.SelectMonsterResponse, new SelectMonsterResponseMessage
                {
                    ActionNumber = ActionNumber,
                    SubActionNumber = SubActionNumber,
                    SelectedMonsterTypeId = SelectedMonster.MonsterTypeId,
                    Message = SelectedMonster.Name,
                });

            }
            else if (availableAttackActions.Select(a => a.Id).Contains(selectedNodeId))
            {
                SelectedAttackNode = availableAttackActions.Single(a => a.Id == selectedNodeId);
                SelectedMovementPath = null;
                SubActionNumber = 5;

                _hostServer.SendToAll(CustomMessageTypes.SelectAttackActionResponse, new SelectAttackResponseMessage
                {
                    ActionNumber = ActionNumber,
                    SubActionNumber = SubActionNumber,

                    Message = "Attack selected",
                    AttackNodeId = SelectedAttackNode.Id
                });
            }
            else if (availableMoveActions.Select(m => m.Id).Contains(selectedNodeId))
            {
                SelectedMovementPath = movementPaths.Single(m => m.DestinationNode.Id ==selectedNodeId);
                SelectedAttackNode = null;
                SubActionNumber = 5;

                _hostServer.SendToAll(CustomMessageTypes.SelectMoveActionResponse, new SelectMoveResponseMessage
                {
                    ActionNumber = ActionNumber,
                    SubActionNumber = SubActionNumber,

                    Message = SelectedMovementPath.ToString(),
                    MovementPathIds = SelectedMovementPath.PathToDestination.Select(n => n.Id).ToArray(),
                    DestinationNodeId = SelectedMovementPath.DestinationNode.Id
                });
            }
        }

        public void ProcessAttackAction(int attackingMonsterTypeId, int defendingMonsterTypeId)
        {
            Monster attacker;
            Monster defender;
            bool isHostAttacker = ActionNumber == 1 || ActionNumber == 2;
            if (isHostAttacker)
            {
                attacker = HostMonsters.Single(m => m.MonsterTypeId == attackingMonsterTypeId);
                defender = GuestMonsters.Single(m => m.MonsterTypeId == defendingMonsterTypeId);
            }
            else
            { 
                attacker = GuestMonsters.Single(m => m.MonsterTypeId == attackingMonsterTypeId);
                defender = HostMonsters.Single(m => m.MonsterTypeId == defendingMonsterTypeId);
            }

            AttackResult attackResult = AttackCalculator.Calculate(attacker.AttackRating, defender.DefenseRating);
            IEnumerable<Node> friendlyOccupiedNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
            IEnumerable<Node> enemyOccupiedNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();

            switch (attackResult)
            {
                case AttackResult.Kill:
                    if (isHostAttacker)
                    {
                        GuestMonsters.Remove(defender);
                    }
                    else
                    {
                        HostMonsters.Remove(defender);
                    }
                    SubActionNumber = 0;

                    _hostServer.SendToAll(CustomMessageTypes.AttackKillResponse, new AttackKillResponseMessage
                    {
                        ActionNumber = ActionNumber,
                        SubActionNumber = SubActionNumber,
                        AttackingMonsterTypeId = attackingMonsterTypeId,
                        DefendingMonsterTypeId = defendingMonsterTypeId,
                        AttackResultId = (int)AttackResult.Kill,
                        Message = "Kill"
                    });
                    break;
                case AttackResult.CounterKill:
                    if (isHostAttacker)
                    {
                        HostMonsters.Remove(attacker);
                    }
                    else
                    {
                        GuestMonsters.Remove(attacker);
                    }
                    SelectedMonster = null;
                    SubActionNumber = 0;

                    _hostServer.SendToAll(CustomMessageTypes.AttackKillResponse, new AttackKillResponseMessage
                    {
                        ActionNumber = ActionNumber,
                        SubActionNumber = SubActionNumber,
                        AttackingMonsterTypeId = attackingMonsterTypeId,
                        DefendingMonsterTypeId = defendingMonsterTypeId,
                        AttackResultId = (int)AttackResult.CounterKill,
                        Message = "Counter Kill"
                    });
                    break;

                case AttackResult.Push:

                    AvailablePushDestinations = MoveCalculator.FindMoves(defender.CurrentNode, 1,
                        friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(m => m.DestinationNode);

                    if (!AvailablePushDestinations.Any())
                    {
                        SubActionNumber = 0;
                        _hostServer.SendToAll(CustomMessageTypes.GameState, new GameStateMessage
                        {
                            ActionNumber = ActionNumber,
                            SubActionNumber = SubActionNumber,
                            Message = "No available push destinations.",
                            HostMonsterState = JsonConvert.SerializeObject(HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                            GuestMonsterState = JsonConvert.SerializeObject(GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id))
                        });
                    }
                    else
                    {
                        SubActionNumber = 6;
                        _hostServer.SendToAll(CustomMessageTypes.AttackPushResponse, new AttackPushResponseMessage
                        {
                            ActionNumber = ActionNumber,
                            SubActionNumber = SubActionNumber,
                            AttackingMonsterTypeId = attackingMonsterTypeId,
                            DefendingMonsterTypeId = defendingMonsterTypeId,
                            AvailablePushDestinationIds = AvailablePushDestinations.Select(d => d.Id).ToArray(),
                            AttackResultId = (int)AttackResult.Push,
                            Message = "Push"
                        });

                    }

                    break;
                case AttackResult.CounterPush:

                    AvailablePushDestinations = MoveCalculator.FindMoves(attacker.CurrentNode, 1,
                        friendlyOccupiedNodes.Union(enemyOccupiedNodes)).Select(m => m.DestinationNode);

                    if (!AvailablePushDestinations.Any())
                    {
                        SubActionNumber = 0;
                        _hostServer.SendToAll(CustomMessageTypes.GameState, new GameStateMessage
                        {
                            ActionNumber = ActionNumber,
                            SubActionNumber = SubActionNumber,
                            Message = "No available push destinations.",
                            HostMonsterState = JsonConvert.SerializeObject(HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                            GuestMonsterState = JsonConvert.SerializeObject(GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id))
                        });
                    }
                    else
                    {
                        SubActionNumber = 7;
                        _hostServer.SendToAll(CustomMessageTypes.AttackPushResponse, new AttackPushResponseMessage
                        {
                            ActionNumber = ActionNumber,
                            SubActionNumber = SubActionNumber,
                            AttackingMonsterTypeId = attackingMonsterTypeId,
                            DefendingMonsterTypeId = defendingMonsterTypeId,
                            AvailablePushDestinationIds = AvailablePushDestinations.Select(d => d.Id).ToArray(),
                            AttackResultId = (int)AttackResult.CounterPush,
                            Message = "Counter Push"
                        });
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }


        private void ProcessMoveAction(Monster selectedMonster, NodePath path)
        {
            selectedMonster.CurrentNode = GameGraph.Nodes[path.DestinationNode.Id];

            SubActionNumber = 0;
        }

        private void SubActionOne()
        {
            IEnumerable<int> availableNodeIds;
            if (ActionNumber == 1 || ActionNumber == 2)
            {
                var enemyNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();
                availableNodeIds =
                    GameGraph.Nodes.Where(n => HostMonsters.Select(m => m.CurrentNode.Id).Contains(n.Id))
                        .OrderByDescending(t => MoveCalculator.FindAttackMoves(t, enemyNodes).Count())           // 1. one with the most attack moves
                        .ThenBy(m => m.Id > 12 ? 0 : 1)                                                          // 2. Monsters on the outer circle 
                        .ThenByDescending(r => HostMonsters.First(w => w.CurrentNode.Id == r.Id).AttackRating)   // 3. the attack level of the monster
                        .Select(n => n.Id);
            }
            else
            {
                var enemyNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
                availableNodeIds =
                    GameGraph.Nodes.Where(n => GuestMonsters.Select(m => m.CurrentNode.Id).Contains(n.Id))
                        .OrderByDescending(t => MoveCalculator.FindAttackMoves(t, enemyNodes).Count())            // 1. one with the most attack moves
                        .ThenBy(m => m.Id > 12 ? 0 : 1)                                                           // 2. Monsters on the outer circle
                        .ThenByDescending(r => GuestMonsters.First(w => w.CurrentNode.Id == r.Id).AttackRating)   // 3. the attack level of the monster
                        .Select(n => n.Id);
            }

            SubActionNumber = 2;

            _hostServer.SendToAll(CustomMessageTypes.AvailableMonstersResponse, new AvailableMonstersResponseMessage
            {
                ActionNumber = ActionNumber,
                SubActionNumber = SubActionNumber,
                AvailableMonsterNodeIds = availableNodeIds.ToArray(),
                Message = "Monsters available for selection"
            });

        }

        private void SubActionThree()
        {
            if (SelectedMonster != null)
            {
                IEnumerable<Node> friendlyOccupiedNodes;
                IEnumerable<Node> enemyOccupiedNodes;

                if (ActionNumber == 1 || ActionNumber == 2)
                {
                    friendlyOccupiedNodes = HostMonsters.Select(monster => monster.CurrentNode).ToList();
                    enemyOccupiedNodes = GuestMonsters.OrderBy(t => t.DefenseRating).Select(monster => monster.CurrentNode).ToList(); // order enemies defense rating
                }
                else
                {
                    friendlyOccupiedNodes = GuestMonsters.Select(monster => monster.CurrentNode).ToList();
                    enemyOccupiedNodes = HostMonsters.OrderBy(t => t.DefenseRating).Select(monster => monster.CurrentNode).ToList(); // order enemies defense rating
                }


                IEnumerable<int> availableMoveActionNodeIds = MoveCalculator.FindMoves(SelectedMonster.CurrentNode,
                    SelectedMonster.MovementRating, friendlyOccupiedNodes.Union(enemyOccupiedNodes))
                    .Select(a => a.DestinationNode.Id)
                    .OrderBy(a => a); // order nodes by id so center nodes are prioritized

                IEnumerable<int> availableAttackActionNodeIds = MoveCalculator.FindAttackMoves(SelectedMonster.CurrentNode,
                    enemyOccupiedNodes).Select(a => a.Id);

                SubActionNumber = 4;

                if (friendlyOccupiedNodes.Count() <= 1 && !availableAttackActionNodeIds.Any() &&
                    !availableMoveActionNodeIds.Any())
                {
                    SubActionNumber = 0;
                    _hostServer.SendToAll(CustomMessageTypes.GameState, new PassActionMessage
                    {
                        ActionNumber = ActionNumber,
                        SubActionNumber = SubActionNumber,
                        Message = "No available moves.",
                        HostMonsterState = JsonConvert.SerializeObject(HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                        GuestMonsterState = JsonConvert.SerializeObject(GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id))
                    });
                }
                else
                {
                    if (friendlyOccupiedNodes.Count() == 1 && availableAttackActionNodeIds.Any())
                    {
                        availableMoveActionNodeIds = new List<int>();
                    }

                    _hostServer.SendToAll(CustomMessageTypes.AvailableMovesResponse, new AvailableMovesResponseMessage
                    {
                        ActionNumber = ActionNumber,
                        SubActionNumber = SubActionNumber,
                        AvailableAttackNodeIds = availableAttackActionNodeIds.ToArray(),
                        AvailableMoveNodeIds = availableMoveActionNodeIds.ToArray(),
                        Message = "Available actions",
                        SelectedMonsterTypeId = SelectedMonster.MonsterTypeId
                    });
                }
            }

        }

    
        private void SubActionFive()
        {
            bool isHostPlayer = ActionNumber == 1 || ActionNumber == 2;

            if (SelectedMonster == null)
            {
                SubActionNumber = 2;
                return;
            }

            if (SelectedAttackNode != null)
            {
                Monster opponent = GetEnemyAtNode(SelectedAttackNode, isHostPlayer);

                ProcessAttackAction(SelectedMonster.MonsterTypeId, opponent.MonsterTypeId);
            }
            else if (SelectedMovementPath != null)
            {
                ProcessMoveAction(SelectedMonster, SelectedMovementPath);
            }

        }


        public void ProcessPushDestination(int selectedNodeId)
        {
            bool guestPush = (ActionNumber == 3 || ActionNumber == 4) && SubActionNumber == 6;
            bool guestCounterPush = (ActionNumber == 1 || ActionNumber == 2) && SubActionNumber == 7;

            bool hostPush = (ActionNumber == 1 || ActionNumber == 2) && SubActionNumber == 6;
            bool hostCounterPush = (ActionNumber == 3 || ActionNumber == 4) && SubActionNumber == 7;

            
            Monster pushedMonster;

            if (hostPush)
            {
                pushedMonster = GuestMonsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id);
            }
            else if (hostCounterPush)
            {
                pushedMonster = GuestMonsters.Single(m => m.CurrentNode.Id == SelectedMonster.CurrentNode.Id);
            }
            else if (guestPush)
            {
                pushedMonster = HostMonsters.Single(m => m.CurrentNode.Id == SelectedAttackNode.Id);
            }
            else if (guestCounterPush)
            {
                pushedMonster = HostMonsters.Single(m => m.CurrentNode.Id == SelectedMonster.CurrentNode.Id);
            }
            else
            {
                SubActionNumber = 0;
                return;
            }

            Node selectedNode = GameGraph.Nodes[selectedNodeId];

            pushedMonster.CurrentNode = selectedNode;

            int[] pathToDestination = { selectedNode.Id };

            SubActionNumber = 0;

            _hostServer.SendToAll(CustomMessageTypes.PushDestinationResponse, new PushDestinationResponseMessage
            {
                ActionNumber = ActionNumber,
                SubActionNumber = SubActionNumber,
                DestinationNodeId = selectedNodeId,
                PushedMonsterTypeId = pushedMonster.MonsterTypeId,
                PathToDestinationNodeIds = pathToDestination
            });

        }

        private void IncrementActionNumber()
        {
            SubActionNumber = 1;
            SelectedAttackNode = null;
            SelectedMovementPath = null;

            if (ActionNumber == 4)
            {
                ActionNumber = 1;
                SelectedMonster = null;
            }
            else if (ActionNumber == 3)
            {
                ActionNumber++;
                SubActionNumber = SelectedMonster != null ? 3 : 1;
            }
            else if (ActionNumber == 2)
            {
                ActionNumber++;
                SelectedMonster = null;
            }
            else if (ActionNumber == 1)
            {
                ActionNumber++;
                SubActionNumber = SelectedMonster != null ? 3 : 1;
            }

            _hostServer.SendToAll(CustomMessageTypes.GameState, new GameStateMessage
            {
                ActionNumber = ActionNumber,
                SubActionNumber = SubActionNumber,
                Message = "New action started",
                HostMonsterState = JsonConvert.SerializeObject(HostMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id)),
                GuestMonsterState = JsonConvert.SerializeObject(GuestMonsters.Select(m => new { m.MonsterTypeId, m.CurrentNode.Id }).ToDictionary(k => k.MonsterTypeId, v => v.Id))
            });
        }
    }

}