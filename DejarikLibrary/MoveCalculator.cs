using System.Collections.Generic;
using System.Linq;

namespace DejarikLibrary
{
	public class MoveCalculator
	{
	    public IEnumerable<NodePath> FindMoves(Node start, int distance, IEnumerable<Node> occupiedNodes = null)
	    {
	        if (occupiedNodes == null)
	        {
	            occupiedNodes = new List<Node>();
	        }

	        BoardGraph boardGraph = new BoardGraph(occupiedNodes.ToList());

	        NodeMapKey nodeMapKey = new NodeMapKey(start.Id, distance);

	        List<NodePath> nodePaths = new List<NodePath>();

	        if (boardGraph.NodeMap.ContainsKey(nodeMapKey))
	        {
	            nodePaths = boardGraph.NodeMap[new NodeMapKey(start.Id, distance)];
	        }

	        return nodePaths;

        }


	    public IEnumerable<Node> FindAttackMoves(Node start, IEnumerable<Node> enemyOccupiedNodes)
		{
	        IEnumerable<Node> occupied = enemyOccupiedNodes.ToList();

	        return start.AdjacentNodes.Where(adjacentNode => occupied.Contains(adjacentNode)).ToList();
		}

	}
}