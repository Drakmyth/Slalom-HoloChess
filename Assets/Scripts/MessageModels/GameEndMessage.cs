using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class GameEndMessage: MessageBase
    {
        public short MessageTypeId;
        public bool IsHostWinner;
        public bool IsGuestWinner;

        public GameEndMessage()
        {
            MessageTypeId = CustomMessageTypes.GameEnd;
        }
    }
}
