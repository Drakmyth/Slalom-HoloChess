using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class AvailableMonstersResponseMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public int[] AvailableMonsterNodeIds;


        public AvailableMonstersResponseMessage()
        {
            MessageTypeId = CustomMessageTypes.AvailableMovesResponse;
        }

    }
}
