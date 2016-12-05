using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class SelectMoveResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int DestinationNodeId;
        public int[] MovementPathIds;

        public SelectMoveResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectMoveActionResponse;
        }
    }
}
