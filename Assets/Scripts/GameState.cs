using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts
{

    public class GameState: MonoBehaviour
    {
        public BoardGraph GameGraph { get; set; }
        public List<IMonsterModel> Player1Monsters { get; set; }
        public List<IMonsterModel> Player2Monsters { get; set; } 
        public GhhhkModel Ghhhk { get; set; }
        //remaining monster models

        public GameState()
        {
            GameGraph = new BoardGraph();
            Ghhhk = new GhhhkModel();
            Player1Monsters = new List<IMonsterModel>();
            Player2Monsters = new List<IMonsterModel>();
        }

        void Start()
        {
            AssignMonsters();
            PlaceMonsters();
        }

        void Update()
        {
            
        }

        private void PlaceMonsters()
        {
            
        }

        private void AssignMonsters()
        {
            List<IMonsterModel> availableMonsters = new List<IMonsterModel>
            {
                Ghhhk
            };
            //TODO: randomly assign
            Player1Monsters.Add(Ghhhk);
        }
    }

}