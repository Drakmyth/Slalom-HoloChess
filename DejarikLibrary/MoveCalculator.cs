using System;
using System.Collections.Generic;
using System.Linq;

namespace DejarikLibrary
{
	public class MoveCalculator
	{
		public List<Node> FindMoves(Node start, int distance)
		{
			Dictionary<Node, int> distanceMap = new Dictionary<Node, int> { { start, 0 } };
			Stack<Node> nodesToVisit = new Stack<Node>();
			nodesToVisit.Push(start);

			do
			{
				Node currentNode = nodesToVisit.Pop();

				foreach (Node adjacentNode in currentNode.AdjacentNodes)
				{
					int newDistance = distanceMap[currentNode] + 1;
					if (distanceMap.ContainsKey(adjacentNode))
					{
						int oldDistance = distanceMap[adjacentNode];
						distanceMap[adjacentNode] = Math.Min(oldDistance, newDistance);
						continue;
					}

					distanceMap[adjacentNode] = newDistance;
					nodesToVisit.Push(adjacentNode);
				}
			}
			while (nodesToVisit.Any());

			List<Node> validMoves = new List<Node>();
			foreach (Node node in distanceMap.Keys)
			{
				int nodeDistance = distanceMap[node];

				if (nodeDistance == distance)
				{
					validMoves.Add(node);
				}
			}

			return validMoves;
		}
	}
}