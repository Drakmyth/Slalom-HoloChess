using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DejarikLibrary
{
	public class BoardGraph
	{

	    public List<SpaceNode> Nodes { get; set; }
	    public Dictionary<Tuple<int, int>, List<NodePath>> NodeMap { get; set; }  

	    public BoardGraph()
	    {
	        Nodes = GenerateNodes();

	        BuildGraph(Nodes);

	        NodeMap = BuildNodeMap(Nodes);

        }

	    private List<SpaceNode> GenerateNodes()
	    {
            List<SpaceNode> nodes = new List<SpaceNode>();

            for (int i = 0; i < 25; i++)
            {
                nodes.Insert(i, new SpaceNode(i));
                //assign x and y coordinate for each node
            }

	        return nodes;

	    }

	    private void BuildGraph(List<SpaceNode> nodes)
	    {
            //inner spaces
            for (int i = 1; i < 13; i++)
            {
                int innerNode = 0;
                int ccwNode = (i + 10) % 12 + 1;
                int cwNode = i % 12 + 1;
                int outerNode = i + 12;

                //add inner circle to center node
                nodes[0].AdjacentNodes.Add(nodes[i]);

                nodes[i].AdjacentNodes.Add(nodes[innerNode]);
                nodes[i].AdjacentNodes.Add(nodes[ccwNode]);
                nodes[i].AdjacentNodes.Add(nodes[cwNode]);
                nodes[i].AdjacentNodes.Add(nodes[outerNode]);

            }

            //outer circle spaces
            for (int i = 13; i < 25; i++)
            {
                int innerNode = i - 12;
                int ccwNode = (i + 10) % 12 + 13;
                int cwNode = i % 12 + 13;

                nodes[i].AdjacentNodes.Add(nodes[innerNode]);
                nodes[i].AdjacentNodes.Add(nodes[ccwNode]);
                nodes[i].AdjacentNodes.Add(nodes[cwNode]);

            }

        }

	    private Dictionary<Tuple<int, int>, List<NodePath>> BuildNodeMap(List<SpaceNode> nodes)
	    {
            Dictionary<Tuple<int, int>, List<NodePath>> nodeMap = new Dictionary<Tuple<int, int>, List<NodePath>>(); 

            foreach (SpaceNode node in nodes)
	        {
	            nodeMap = nodeMap.Concat(BuildMapForNode(nodes, node)).ToDictionary(x => x.Key, x => x.Value);
	        }

	        return nodeMap;
	    }

        private Dictionary<Tuple<int,int>,List<NodePath>> BuildMapForNode(List<SpaceNode> nodes, SpaceNode sourceNode)
	    {
            List<SpaceNode> unvisitedNodes = new List<SpaceNode>();
            Dictionary<SpaceNode, int> shortestDistanceToNode = new Dictionary<SpaceNode, int>();
            Dictionary<SpaceNode, SpaceNode> previousNodeAlongShortestPath = new Dictionary<SpaceNode, SpaceNode>();

            foreach (SpaceNode node in nodes)
            {
                shortestDistanceToNode.Add(node, int.MaxValue);
                previousNodeAlongShortestPath.Add(node, null);
                unvisitedNodes.Add(node);
            }

            shortestDistanceToNode[sourceNode] = 0;

            while (unvisitedNodes.Any())
            {
                SpaceNode currentNode = sourceNode;
                unvisitedNodes.Remove(currentNode);

                foreach (SpaceNode adjacentNode in currentNode.AdjacentNodes)
                {
                    shortestDistanceToNode(currentNode);
                }

            }
	    }
}

}
 