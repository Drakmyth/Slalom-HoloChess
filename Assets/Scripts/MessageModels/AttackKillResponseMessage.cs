using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class AttackKillResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int AttackingMonsterTypeId;
        public int DefendingMonsterTypeId;
        public int AttackResultId;

        public AttackKillResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.AttackKillResponse;
        }
    }
}
