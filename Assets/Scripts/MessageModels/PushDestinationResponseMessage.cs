using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class PushDestinationResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int PushedMonsterTypeId;
        public int[] PathToDestinationNodeIds;
        public int DestinationNodeId;

        public PushDestinationResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.PushDestinationResponse;
        }
    }
}
