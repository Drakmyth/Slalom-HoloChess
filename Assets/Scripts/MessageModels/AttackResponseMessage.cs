﻿namespace Assets.Scripts.MessageModels
{
    public class AttackResponseMessage : GameStateMessage
    {
        public int AttackingMonsterTypeId { get; set; }
        public int DefendingMonsterTypeId { get; set; }
        public int AttackResultId { get; set; }

        public override short MessageTypeId
        {
            get { return CustomMessageTypes.AttackResponse; }
        }
    }
}
