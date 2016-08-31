using System.Collections.Generic;

namespace DejarikLibrary
{
    public class NodePath
    {
        public SpaceNode DestinationNode { get; set; }
        public List<SpaceNode> PathToDestination { get; set; }
    }
}