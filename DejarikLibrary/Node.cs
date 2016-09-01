using System.Collections.Generic;

namespace DejarikLibrary
{

    public class Node
    {
        public int Id { get; set; }
        public List<Node> AdjacentNodes { get; set; }
        public double xPosition { get; set; }
        public double yPosition { get; set; }

        public Node(int id)
        {
            Id = id;
            AdjacentNodes = new List<Node>();
        }
    }
}