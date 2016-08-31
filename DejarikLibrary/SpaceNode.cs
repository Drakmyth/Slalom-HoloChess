using System.Collections.Generic;

namespace DejarikLibrary
{

    public class SpaceNode
    {
        public int Id { get; set; }
        public List<SpaceNode> AdjacentNodes { get; set; }
//        public Transform Position { get; set; }

        public SpaceNode(int id)
        {
            Id = id;
            AdjacentNodes = new List<SpaceNode>();
        }
    }
}