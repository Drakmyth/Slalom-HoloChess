using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class GameStateMessage : MessageBase
    {
        public int ActionId;
        public int SubActionId;
        public string Message;
        public short MessageTypeId;


        public GameStateMessage()
        {
            MessageTypeId = CustomMessageTypes.GameState;
        }


    }
}
