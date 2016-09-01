using System.Collections.Generic;

namespace DejarikLibrary
{
    public class NodePath
    {
        public Node DestinationNode { get; set; }
        public List<Node> PathToDestination { get; set; }

        public NodePath()
        {
            PathToDestination = new List<Node>();
        }

        public NodePath(Node destinationNode)
        {
            DestinationNode = destinationNode;
            PathToDestination = new List<Node> { destinationNode};
        }

        public NodePath(List<Node> nodePath, Node destinationNode)
        {
            DestinationNode = destinationNode;
            PathToDestination = nodePath;
        }
    }
}