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
        public List<Monster> Player1Monsters { get; set; }
        public List<Monster> Player2Monsters { get; set; } 


        private readonly Random _random;
        //3, 4 are opponent actions, 1, 2 are player actions
        private int _actionNumber;
        //TODO:not sure if or how this will be used yet
        private bool _isHostPlayer = true;

        public List<Monster> MonsterPrefabs;

        public Dictionary<int, Node> SpacePrefabs;

        public GameState()
        {
            _random = new Random();

            //TODO:needs to be set by whatever function determines who starts
            _actionNumber = 0;

            GameGraph = new BoardGraph();
            Player1Monsters = new List<Monster>();
            Player2Monsters = new List<Monster>();
        }


        void Start()
        {
            if (_isHostPlayer)
            {
                AssignMonstersToPlayers();
            }

        }

        void Update()
        {
            if (_actionNumber < 1)
            {
                return;
            }

            //TODO:Get user input to select from available monsters
            Monster selectedMonster = MonsterPrefabs[MonsterTypes.Ghhhk];

            //TODO:Get available nodes from Library for move or attack, possibly returning an action type as well?
            Node actionNode = GameGraph.Nodes[0];

            //TODO:Get user input to select from available actions

            Monster opponent = GetEnemyAtNode(actionNode);

            if (opponent != null)
            {
                //TODO:Should this be a static class? Otherwise, this should really be initialized outside of the turn loop
                //TODO:Battle animation!
                AttackCalculator attackCalculator = new AttackCalculator();
                var attackResult = attackCalculator.Calculate(selectedMonster.AttackRating, opponent.DefenseRating);

                switch (attackResult)
                {
                    case AttackResult.Kill:
                        //TODO:Kill animation!
                        Player2Monsters.Remove(opponent);
                        break;
                    case AttackResult.CounterKill:
                        //TODO:Kill animation!
                        Player1Monsters.Remove(selectedMonster);
                        break;
                    case AttackResult.Push:
                        //TODO:calculate availables spaces with movement 1 from opponent.CurrentNode
                        //TODO:Get user input to select which node
                        //TODO:Movement animation!
                        Node pushTo = GameGraph.Nodes[opponent.CurrentNode.Id + 1];
                        opponent.CurrentNode = pushTo;
                        break;
                    case AttackResult.CounterPush:
                        //TODO:calculate availables spaces with movement 1 from actionNode/selectedMonster.CurrentNode
                        //TODO:Get user input to select which node
                        //TODO:Movement animation!
                        Node counterPushTo = GameGraph.Nodes[actionNode.Id + 1];
                        opponent.CurrentNode = counterPushTo;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                //TODO:Get animation path from GameGraph.NodeMap and move peice from space to space
                //TODO:Movement animation!
                selectedMonster.CurrentNode = actionNode;
            }

            if (_actionNumber >= 2)
            {
                _actionNumber = 0;
                //TODO:launch other player's action pair
            }
            else
            {
                _actionNumber++;
            }

            //TODO:check for end of game

        }

        private Monster GetEnemyAtNode(Node node)
        {
            List<Monster> enemyMonsters = _isHostPlayer ? Player2Monsters : Player1Monsters;

            return enemyMonsters.FirstOrDefault(monster => monster.CurrentNode == node);
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

                GameObject monsterPrefab = MonsterPrefabs[MonsterPrefabs.IndexOf(currentMonster)].gameObject;

                Quaternion monsterQuaternion;

                if(availableMonsters.Count % 2 == 0)
                {
                    currentMonster.CurrentNode = player1StartingNodes[0];
                    player1StartingNodes.RemoveAt(0);
                    Player1Monsters.Add(currentMonster);
                    monsterQuaternion = monsterPrefab.transform.rotation;
                } else
                {
                    currentMonster.CurrentNode = player2StartingNodes[0];
                    player2StartingNodes.RemoveAt(0);
                    Player2Monsters.Add(currentMonster);                  
                    monsterQuaternion = Quaternion.Euler(monsterPrefab.transform.rotation.eulerAngles.x, monsterPrefab.transform.rotation.eulerAngles.y + 180, monsterPrefab.transform.rotation.eulerAngles.z);
                }

                GameObject monster = (GameObject)Instantiate(monsterPrefab, new Vector3(currentMonster.CurrentNode.XPosition, 0, currentMonster.CurrentNode.YPosition), monsterQuaternion);

                availableMonsters.RemoveAt(monsterIndex);

            }

        }
    }

}