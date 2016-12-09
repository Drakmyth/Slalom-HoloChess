using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class AttackPushResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int[] AvailablePushDestinationIds;
        public int AttackingMonsterTypeId;
        public int DefendingMonsterTypeId;
        public int AttackResultId;

        public AttackPushResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.AttackPushResponse;
        }
    }
}
