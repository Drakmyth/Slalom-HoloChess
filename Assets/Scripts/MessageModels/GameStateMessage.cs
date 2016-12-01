using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public abstract class GameStateMessage : MessageBase
    {
        public int ActionId { get; set; }
        public int SubActionId { get; set; }
        public string Message { get; set; }
        public short MessageTypeId = CustomMessageTypes.GameState;

    }
}
