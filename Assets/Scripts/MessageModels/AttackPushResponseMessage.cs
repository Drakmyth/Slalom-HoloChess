using System.Collections.Generic;
using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class AttackPushResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public List<int> AvailablePushDestinationIds { get; set; }
        public int AttackingMonsterTypeId;
        public int DefendingMonsterTypeId;
        public int AttackResultId;

        public AttackPushResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.AttackPushResponse;
        }
    }
}
