using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts
{
	public class BoardGraph : MonoBehaviour
	{

	    public virtual List<SpaceNode> Nodes { get; set; }
        public virtual bool boole { get; set; }
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

}