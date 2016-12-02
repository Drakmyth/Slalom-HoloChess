namespace Assets.Scripts.MessageModels
{
    public class SelectMonsterResponseMessage : GameStateMessage
    {
        public int SelectedMonsterTypeId { get; set; }

        public SelectMonsterResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectMonsterResponse;
        }
    }
}
