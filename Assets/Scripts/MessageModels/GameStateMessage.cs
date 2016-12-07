using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class GameStateMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;
        public string HostMonsterState;
        public string GuestMonsterState;

        public GameStateMessage()
        {
            MessageTypeId = CustomMessageTypes.GameState;
        }


    }
}
