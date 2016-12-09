using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class SelectAttackResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int AttackNodeId;

        public SelectAttackResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectAttackActionResponse;
        }
    }
}
