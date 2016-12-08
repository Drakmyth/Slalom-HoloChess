using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class PushDestinationRequestMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;
        
        public int SelectedNodeId;

        public PushDestinationRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.PushDestinationRequest;
        }
    }
}
