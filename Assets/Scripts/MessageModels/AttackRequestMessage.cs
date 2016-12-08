using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class AttackRequestMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int AttackingMonsterTypeId;
        public int DefendingMonsterTypeId;

        public AttackRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.AttackRequest;
        }
    }
}

