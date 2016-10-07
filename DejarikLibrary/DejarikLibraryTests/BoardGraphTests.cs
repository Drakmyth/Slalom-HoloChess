using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DejarikLibrary;

namespace DejarikLibraryTests
{
    [TestClass]
    public class BoardGraphTests
    {
        [TestMethod]
        public void BoardGraph_ShortestPathsFromCenterAreAccurate()
        {
            BoardGraph graph = new BoardGraph();
            
            Assert.IsNotNull(graph.NodeMap);

            for (int i = 13; i < 25; i++)
            {

                List<NodePath> actualNodePath = graph.NodeMap[new NodeMapKey(0, 1)];
                Assert.IsTrue(actualNodePath.Select(x => x.DestinationNode).Contains(graph.Nodes[i - 12]));
                Assert.AreEqual(1, actualNodePath.Single(x => x.DestinationNode.Id == i - 12).PathToDestination.Count());
                Assert.AreEqual(graph.Nodes[i - 12].Id, actualNodePath.Single(x => x.DestinationNode.Id == i - 12).PathToDestination[0].Id);


                actualNodePath = graph.NodeMap[new NodeMapKey(0, 2)];
                Assert.IsTrue(actualNodePath.Select(x => x.DestinationNode).Contains(graph.Nodes[i]));
                Assert.AreEqual(2, actualNodePath.Single(x => x.DestinationNode.Id == i).PathToDestination.Count());
                Assert.AreEqual(graph.Nodes[i].Id, actualNodePath.Single(x => x.DestinationNode.Id == i).PathToDestination[1].Id);
                Assert.AreEqual(graph.Nodes[i-12].Id, actualNodePath.Single(x => x.DestinationNode.Id == i).PathToDestination[0].Id);
            }

        }

        [TestMethod]
        public void BoardGraph_NoShortestPathsGreaterThanFour()
        {
            BoardGraph graph = new BoardGraph();

            List<NodeMapKey> actual = graph.NodeMap.Keys.Where(k => k.Movement > 4).ToList();

            Assert.AreEqual(actual.Count, 0);

        }

        [TestMethod]
        public void BoardGraph_EachNodeHasExpectedNumberOfNodesAtVariousDistances()
        {
            const int innerOneExpected = 4;
            const int innerTwoExpected = 11;
            const int innerThreeExpected = 9;
            const int innerFourExpected = 0;
            const int outerOneExpected = 3;
            const int outerTwoExpected = 5;
            const int outerThreeExpected = 11;
            const int outerFourExpected = 5;
            const int centerOneExpected = 12;
            const int centerTwoExpected = 12;
            const int centerThreeExpected = 0;
            const int centerFourExpected = 0;


            BoardGraph graph = new BoardGraph();

            List<NodeMapKey> centerOneActual = graph.NodeMap.Keys.Where(k => k.NodeId == 0 && k.Movement == 1).ToList();
            List<NodeMapKey> centerTwoActual = graph.NodeMap.Keys.Where(k => k.NodeId == 0 && k.Movement == 2).ToList();
            List<NodeMapKey> centerThreeActual = graph.NodeMap.Keys.Where(k => k.NodeId == 0 && k.Movement == 3).ToList();
            List<NodeMapKey> centerFourActual = graph.NodeMap.Keys.Where(k => k.NodeId == 0 && k.Movement == 4).ToList();

            for (int i = 13; i < 25; i++)
            {
                List<NodeMapKey> innerOneActual = graph.NodeMap.Keys.Where(k => k.NodeId == i - 12 && k.Movement == 1).ToList();
                List<NodeMapKey> innerTwoActual = graph.NodeMap.Keys.Where(k => k.NodeId == i - 12 && k.Movement == 2).ToList();
                List<NodeMapKey> innerThreeActual = graph.NodeMap.Keys.Where(k => k.NodeId == i - 12 && k.Movement == 3).ToList();
                List<NodeMapKey> innerFourActual = graph.NodeMap.Keys.Where(k => k.NodeId == i - 12 && k.Movement == 4).ToList();


                List<NodeMapKey> outerOneActual = graph.NodeMap.Keys.Where(k => k.NodeId == i && k.Movement == 1).ToList();
                List<NodeMapKey> outerTwoActual = graph.NodeMap.Keys.Where(k => k.NodeId == i && k.Movement == 2).ToList();
                List<NodeMapKey> outerThreeActual = graph.NodeMap.Keys.Where(k => k.NodeId == i && k.Movement == 3).ToList();
                List<NodeMapKey> outerFourActual = graph.NodeMap.Keys.Where(k => k.NodeId == i && k.Movement == 4).ToList();

                Assert.AreEqual(innerOneActual.Count, innerOneExpected);
                Assert.AreEqual(innerTwoActual.Count, innerTwoExpected);
                Assert.AreEqual(innerThreeActual.Count, innerThreeExpected);
                Assert.AreEqual(innerFourActual.Count, innerFourExpected);

                Assert.AreEqual(outerOneActual.Count, outerOneExpected);
                Assert.AreEqual(outerTwoActual.Count, outerTwoExpected);
                Assert.AreEqual(outerThreeActual.Count, outerThreeExpected);
                Assert.AreEqual(outerFourActual.Count, outerFourExpected);

            }

            Assert.AreEqual(centerOneActual.Count, centerOneExpected);
            Assert.AreEqual(centerTwoActual.Count, centerTwoExpected);
            Assert.AreEqual(centerThreeActual.Count, centerThreeExpected);
            Assert.AreEqual(centerFourActual.Count, centerFourExpected);

        }

        [TestMethod]
        public void BoardGraph_NodeMapExcludesSpecifiedNodesInCalculation()
        {
            List<Node> excludedNodes = new List<Node> {new Node(0)};
            BoardGraph graph = new BoardGraph(excludedNodes);

            List<NodePath> actual = graph.NodeMap[new NodeMapKey(14, 3)];
            //17,23,12,4
            Assert.AreEqual(actual.Count, 4);
            Assert.IsTrue(actual.Select(n => n.DestinationNode.Id).Contains(17));
            Assert.IsTrue(actual.Select(n => n.DestinationNode.Id).Contains(23));
            Assert.IsTrue(actual.Select(n => n.DestinationNode.Id).Contains(12));
            Assert.IsTrue(actual.Select(n => n.DestinationNode.Id).Contains(4));

        }

        [TestMethod]
        public void BoardGraph_NodeSurroundedByOccupiedNodesHasNoMovements()
        {
            List<Node> excludedNodes = new List<Node> { new Node(13), new Node(0), new Node(2), new Node(12) };

            BoardGraph graph = new BoardGraph(excludedNodes);

            List<NodeMapKey> actualKeys = graph.NodeMap.Keys.Where(k => k.NodeId == 1).ToList();

            Assert.AreEqual(actualKeys.Count, 1);
            Assert.AreEqual(actualKeys[0].NodeId, 1);

        }
    }
}
