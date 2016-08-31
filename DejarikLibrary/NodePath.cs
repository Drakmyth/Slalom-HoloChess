using System.Collections.Generic;

namespace DejarikLibrary
{
    public class NodePath
    {
        public SpaceNode DestinationNode { get; set; }
        public List<SpaceNode> PathToDestination { get; set; }

        public NodePath()
        {
            PathToDestination = new List<SpaceNode>();
        }

        public NodePath(SpaceNode destinationNode)
        {
            DestinationNode = destinationNode;
            PathToDestination = new List<SpaceNode> { destinationNode};
        }

        public NodePath(List<SpaceNode> nodePath, SpaceNode destinationNode)
        {
            DestinationNode = destinationNode;
            PathToDestination = nodePath;
        }
    }
}