using System.Collections.Generic;

namespace Assets.Scripts.MessageModels
{
    public class SelectMoveResponseMessage : GameStateMessage
    {
        public int DestinationNodeId { get; set; }
        public List<int> MovementPathIds { get; set; }

        public SelectMoveResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectActionResponse;
        }
    }
}
