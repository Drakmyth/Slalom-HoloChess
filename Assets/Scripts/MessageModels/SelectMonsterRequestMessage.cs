namespace Assets.Scripts.MessageModels
{
    public class SelectMonsterRequestMessage : GameStateMessage
    {
        public int SelectedMonsterTypeId { get; set; }

        public SelectMonsterRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectMonsterRequest;
        }
    }
}
