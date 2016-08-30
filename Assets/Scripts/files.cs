using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts
{
/*
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

	public class BoardGraph : MonoBehaviour
	{

	    public List<SpaceNode> Nodes { get; set; }
	    private Transform _position;

	    public BoardGraph()
	    {
	        Nodes = new List<SpaceNode>();

	        for (int i = 0; i < 25; i++)
	        {
                Nodes.Insert(i, new SpaceNode(i));
                //assign x and y coordinate for each node
            }

            //inner spaces
	        for (int i = 1; i < 13; i++)
	        {
	            int innerNode = 0;
                int ccwNode = (i + 10) % 12 + 1;
	            int cwNode = i % 12 + 1;
	            int outerNode = i + 12;

                //add inner circle to center node
	            Nodes[0].AdjacentNodes.Add(Nodes[i]);

                Nodes[i].AdjacentNodes.Add(Nodes[innerNode]);
                Nodes[i].AdjacentNodes.Add(Nodes[ccwNode]);
                Nodes[i].AdjacentNodes.Add(Nodes[cwNode]);
                Nodes[i].AdjacentNodes.Add(Nodes[outerNode]);

            }

            //outer circle spaces
            for (int i = 13; i < 25; i++)
            {
                int innerNode = i - 12;
                int ccwNode = (i + 10) % 12 + 13;
                int cwNode = i % 12 + 13;

                Nodes[i].AdjacentNodes.Add(Nodes[innerNode]);
                Nodes[i].AdjacentNodes.Add(Nodes[ccwNode]);
                Nodes[i].AdjacentNodes.Add(Nodes[cwNode]);

            }

        }

	    void Start()
	    {
            //may need to be in update
	        _position = transform;
	    }
    }

    public class SpaceNode
    {
        public int Id { get; set; }
        public List<SpaceNode> AdjacentNodes { get; set; }
        public IMonsterModel OccupiedBy { get; set; }
        public Transform Position { get; set; }

        public SpaceNode(int id)
        {
            Id = id;
            AdjacentNodes = new List<SpaceNode>();
        }
    }

    public interface IMonsterModel
    {
        GameObject UnityObject { get; set; }
        SpaceNode CurrentNode { get; set; }
    }
    */
    public class GhhhkModel : IMonsterModel
    {
        public const int Attack = 4;
        public const int Defense = 3;
        public const int Movement = 2;

        public GameObject UnityObject { get; set; }
        public SpaceNode CurrentNode { get; set; }

        public GhhhkModel()
        {
            UnityObject = new GameObject("ghhhk");
        }
    }

}