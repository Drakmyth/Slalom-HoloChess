namespace Assets.Scripts.MessageModels
{
    public class AttackRequestMessage : GameStateMessage
    {
        public int AttackingMonsterTypeId { get; set; }
        public int DefendingMonsterTypeId { get; set; }

        public AttackRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.AttackRequest;
        }
    }
}
