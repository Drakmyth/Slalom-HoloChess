using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class ConnectRequestMessage : MessageBase
    {

        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public string ClientName;
        public bool IsHost;

        public ConnectRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.ConnectRequest;
        }
    }
}
