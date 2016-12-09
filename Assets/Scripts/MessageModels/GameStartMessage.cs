using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class GameStartMessage: MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;

        public string HostMonsters;
        public string GuestMonsters;

        public GameStartMessage()
        {
            MessageTypeId = CustomMessageTypes.GameStart;
        }
    }
}
