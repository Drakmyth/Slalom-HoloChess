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
        public List<IMonster> Player1Monsters { get; set; }
        public List<IMonster> Player2Monsters { get; set; } 
        public Ghhhk Ghhhk { get; set; }
        public Houjix Houjix { get; set; }
        public Strider Strider { get; set; }
        public Molator Molator { get; set; }
        public Savrip Savrip { get; set; }
        public Klorslug Klorslug { get; set; }
        public Ngok Ngok { get; set; }
        public Monnok Monnok { get; set; }

        private readonly Random _random;
        //0 is opponent turn, 1, 2 are current player actions
        private int _actionNumber;
        //TODO:not sure if or how this will be used yet
        private bool _isHostPlayer = true;

        public GameState()
        {
            _random = new Random();

            //TODO:needs to be set by whatever function determines who starts
            _actionNumber = 0;

            GameGraph = new BoardGraph();
            InitializeMonsters();
            Player1Monsters = new List<IMonster>();
            Player2Monsters = new List<IMonster>();
        }

        private void InitializeMonsters()
        {
            Ghhhk = new Ghhhk();
            Houjix = new Houjix();
            Strider = new Strider();
            Molator = new Molator();
            Savrip = new Savrip();
            Klorslug = new Klorslug();
            Ngok = new Ngok();
            Monnok = new Monnok();
        }

        void Start()
        {
            if (_isHostPlayer)
            {
                AssignMonstersToPlayers();
                PlaceMonsters();
            }
        }

        void Update()
        {
            if (_actionNumber < 1)
            {
                return;
            }

            //TODO:Get user input to select from available monsters
            IMonster selectedMonster = Ghhhk;

            //TODO:Get available nodes from Library for move or attack, possibly returning an action type as well?
            Node actionNode = GameGraph.Nodes[0];

            //TODO:Get user input to select from available actions

            IMonster opponent = GetEnemyAtNode(actionNode);

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

        private IMonster GetEnemyAtNode(Node node)
        {
            List<IMonster> enemyMonsters = _isHostPlayer ? Player2Monsters : Player1Monsters;

            return enemyMonsters.FirstOrDefault(monster => monster.CurrentNode == node);
        }

        private void PlaceMonsters()
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

            AssignMonstersToNodes(Player1Monsters, player1StartingNodes);
            AssignMonstersToNodes(Player2Monsters, player2StartingNodes);
        }

        private void AssignMonstersToNodes(List<IMonster> monsters, List<Node> nodes)
        {
            int monsterIndex = 0;

            while (nodes.Any())
            {
                int nodeId = _random.Next(0, nodes.Count - 1);

                monsters[monsterIndex].CurrentNode = nodes[nodeId];

                nodes.RemoveAt(nodeId);

                monsterIndex++;
            }
        }

        private void AssignMonstersToPlayers()
        {

            List<IMonster> availableMonsters = new List<IMonster>
            {
                Ghhhk,
                Houjix,
                Strider,
                Molator,
                Savrip,
                Klorslug,
                Ngok,
                Monnok
            };

            while (availableMonsters.Any())
            {
                int monsterIndex = _random.Next(0, availableMonsters.Count - 1);
                   
                IMonster currentMonster = availableMonsters[monsterIndex];

                if(availableMonsters.Count % 2 == 0)
                {
                    Player1Monsters.Add(currentMonster);
                } else
                {
                    Player2Monsters.Add(currentMonster);
                }

                availableMonsters.RemoveAt(monsterIndex);

            }
        }
    }

}