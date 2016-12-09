using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class SelectMonsterResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int SelectedMonsterTypeId;

        public SelectMonsterResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectMonsterResponse;
        }
    }
}
