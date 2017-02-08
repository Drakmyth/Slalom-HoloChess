using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class PassActionMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;
        public string HostMonsterState;
        public string GuestMonsterState;

        public PassActionMessage()
        {
            MessageTypeId = CustomMessageTypes.PassAction;
        }
    }
}
