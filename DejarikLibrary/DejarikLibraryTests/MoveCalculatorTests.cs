using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DejarikLibrary;

namespace DejarikLibraryTests
{
    [TestClass]
    public class MoveCalculatorTests
    {
        [TestMethod]
        public void FindMoves_ReturnsExpectedNodesForMovementThree()
        {
            BoardGraph boardGraph = new BoardGraph();
            MoveCalculator moveCalculator = new MoveCalculator();

            List<Node> occupiedNodes = new List<Node>();
            occupiedNodes.Add(boardGraph.Nodes[13]);
            occupiedNodes.Add(boardGraph.Nodes[14]);
            occupiedNodes.Add(boardGraph.Nodes[17]);
            occupiedNodes.Add(boardGraph.Nodes[18]);
            occupiedNodes.Add(boardGraph.Nodes[19]);
            occupiedNodes.Add(boardGraph.Nodes[20]);
            occupiedNodes.Add(boardGraph.Nodes[23]);
            occupiedNodes.Add(boardGraph.Nodes[24]);

            IEnumerable<NodePath> actual;

            actual = moveCalculator.FindMoves(boardGraph.Nodes[13], 3, occupiedNodes);
            actual = moveCalculator.FindMoves(boardGraph.Nodes[14], 3, occupiedNodes);
            actual = moveCalculator.FindMoves(boardGraph.Nodes[17], 3, occupiedNodes);
            actual = moveCalculator.FindMoves(boardGraph.Nodes[18], 3, occupiedNodes);
            actual = moveCalculator.FindMoves(boardGraph.Nodes[19], 3, occupiedNodes);
            actual = moveCalculator.FindMoves(boardGraph.Nodes[20], 3, occupiedNodes);
            actual = moveCalculator.FindMoves(boardGraph.Nodes[23], 3, occupiedNodes);
            actual = moveCalculator.FindMoves(boardGraph.Nodes[24], 3, occupiedNodes);

        }
    }
}
