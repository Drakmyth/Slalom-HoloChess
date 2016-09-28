using System.Collections.Generic;

namespace DejarikLibrary
{
    public class Node
    {
        public int Id { get; set; }
        public List<Node> AdjacentNodes { get; set; }
        public float XPosition { get; set; }
        public float YPosition { get; set; }

        public Node(int id)
        {
            Id = id;
            AdjacentNodes = new List<Node>();
        }
    }
}