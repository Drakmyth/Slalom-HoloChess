namespace Assets.Scripts.MessageModels
{
    public class SelectAttackResponseMessage : GameStateMessage
    {
        public int AttackNodeId { get; set; }

        public SelectAttackResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectActionResponse;
        }
    }
}
