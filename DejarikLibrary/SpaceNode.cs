using System.Collections.Generic;

namespace DejarikLibrary
{

    public class SpaceNode
    {
        public int Id { get; set; }
        public List<SpaceNode> AdjacentNodes { get; set; }
        public double xPosition { get; set; }
        public double yPosition { get; set; }

        public SpaceNode(int id)
        {
            Id = id;
            AdjacentNodes = new List<SpaceNode>();
        }
    }
}