namespace Assets.Scripts.MessageModels
{
    public class MoveRequestMessage : GameStateMessage
    {
        public int SelectedMonsterTypeId { get; set; }
        public int DestinationNodeId { get; set; }

        public MoveRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.MoveRequest;
        }
    }
}
