using System;

namespace DejarikLibrary
{
    public class NodeMapKey : IEquatable<NodeMapKey>
    {
        public int NodeId;
        public int Movement;

        public NodeMapKey(int nodeId, int movement)
        {
            NodeId = nodeId;
            Movement = movement;
        }

        public override bool Equals(object x)
        {
            return Equals(x as NodeMapKey);
        }

        public bool Equals(NodeMapKey x)
        {
            return x != null && x.NodeId == NodeId && x.Movement == Movement;
        }

        public override int GetHashCode()
        {
            return NodeId.GetHashCode() + Movement.GetHashCode();
        }

        public override string ToString()
        {
            return NodeId + ":" + Movement;
        }

    }
}
