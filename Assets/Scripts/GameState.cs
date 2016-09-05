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

        public GameState()
        {
            _random = new Random();

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
            AssignMonstersToPlayers();
            PlaceMonsters();
        }

        void Update()
        {
            
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