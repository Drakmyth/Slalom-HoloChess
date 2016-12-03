namespace Assets.Scripts.MessageModels
{
    public class AttackRequestMessage : GameStateMessage
    {
        public int AttackingMonsterTypeId { get; set; }
        public int DefendingMonsterTypeId { get; set; }
        public float XCoordinate { get; set; }
        public float YCoordinate { get; set; }
        public float ZCoordinate { get; set; }

        public AttackRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.AttackRequest;
        }
    }
}

