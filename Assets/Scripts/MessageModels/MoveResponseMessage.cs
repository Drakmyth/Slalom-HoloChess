using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class MoveResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int SelectedMonsterTypeId;
        public string NodePath;

        public MoveResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.MoveResponse;
        }
    }
}
