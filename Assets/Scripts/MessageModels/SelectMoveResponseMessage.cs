using System.Collections.Generic;
using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class SelectMoveResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int DestinationNodeId { get; set; }
        public List<int> MovementPathIds { get; set; }

        public SelectMoveResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectActionResponse;
        }
    }
}
