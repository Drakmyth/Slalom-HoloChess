namespace Assets.Scripts.MessageModels
{
    public class SelectActionRequestMessage : GameStateMessage
    {
        public int SelectedNodeId { get; set; }

        public SelectActionRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectActionRequest;
        }
    }
}
