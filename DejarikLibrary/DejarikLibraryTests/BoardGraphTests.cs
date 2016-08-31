using System;
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
            
            //TODO: ianb- make this test test what it says it will test -20160830
            Assert.IsNotNull(graph.NodeMap);

            for (int i = 13; i < 25; i++)
            {

                List<NodePath> actualNodePath = graph.NodeMap[new Tuple<int, int>(0, 1)];
                Assert.IsTrue(actualNodePath.Select(x => x.DestinationNode).Contains(graph.Nodes[i - 12]));
                Assert.AreEqual(1, actualNodePath.Single(x => x.DestinationNode.Id == i - 12).PathToDestination.Count());
                Assert.AreEqual(graph.Nodes[i - 12].Id, actualNodePath.Single(x => x.DestinationNode.Id == i - 12).PathToDestination[0].Id);


                actualNodePath = graph.NodeMap[new Tuple<int, int>(0, 2)];
                Assert.IsTrue(actualNodePath.Select(x => x.DestinationNode).Contains(graph.Nodes[i]));
                Assert.AreEqual(2, actualNodePath.Single(x => x.DestinationNode.Id == i).PathToDestination.Count());
                Assert.AreEqual(graph.Nodes[i].Id, actualNodePath.Single(x => x.DestinationNode.Id == i).PathToDestination[1].Id);
                Assert.AreEqual(graph.Nodes[i-12].Id, actualNodePath.Single(x => x.DestinationNode.Id == i).PathToDestination[0].Id);
            }

        }
    }
}
