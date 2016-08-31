using System;
using System.Collections.Generic;
using System.Linq;

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


        // Well, it certainly ain't pretty, but it's past my bed time - ianb 20160831
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
                unvisitedNodes.Sort((x, y) => shortestDistanceToNode[x] - shortestDistanceToNode[y]);
                SpaceNode currentNode = unvisitedNodes[0];
                unvisitedNodes.Remove(currentNode);

                foreach(SpaceNode adjacentNode in currentNode.AdjacentNodes)
                {
                    int currentPathDistance = shortestDistanceToNode[currentNode] + 1;
                    if (currentPathDistance < shortestDistanceToNode[adjacentNode])
                    {
                        shortestDistanceToNode[adjacentNode] = currentPathDistance;
                        previousNodeAlongShortestPath[adjacentNode] = currentNode;
                    }
                }
            }


            Dictionary<Tuple<int, int>, List<NodePath>> shortestPathMap = new Dictionary<Tuple<int, int>, List<NodePath>>();

            foreach (SpaceNode node in nodes)
            {
                int distance = 0;
                NodePath shortestPath = new NodePath();
                shortestPath.DestinationNode = node;
                SpaceNode currentPathNode = node;
                while (previousNodeAlongShortestPath[currentPathNode] != null)
                {
                    distance++;
                    shortestPath.PathToDestination.Insert(0, currentPathNode);
                    currentPathNode = previousNodeAlongShortestPath[currentPathNode];
                }

                Tuple<int, int> currentTupleKey = new Tuple<int, int>(sourceNode.Id, distance);

                if (!shortestPathMap.ContainsKey(currentTupleKey))
                {
                    shortestPathMap.Add(currentTupleKey, new List<NodePath>());
                }

                shortestPathMap[currentTupleKey].Add(shortestPath);

            }

            return shortestPathMap;
        }

    }

}
 