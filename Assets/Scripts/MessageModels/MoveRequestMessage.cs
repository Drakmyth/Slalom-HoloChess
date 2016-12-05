using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class MoveRequestMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int SelectedMonsterTypeId;
        public int DestinationNodeId;

        public MoveRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.MoveRequest;
        }
    }
}
