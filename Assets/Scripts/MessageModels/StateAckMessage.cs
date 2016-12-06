using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class StateAckMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public bool IsHost;


        public StateAckMessage()
        {
            MessageTypeId = CustomMessageTypes.StateAck;
        }
    }
}
