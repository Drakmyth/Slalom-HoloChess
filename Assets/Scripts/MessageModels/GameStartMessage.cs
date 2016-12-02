namespace Assets.Scripts.MessageModels
{
    public class GameStartMessage: GameStateMessage
    {
        public string HostMonsters;
        public string GuestMonsters;

        public GameStartMessage()
        {
            MessageTypeId = CustomMessageTypes.GameStart;
        }
    }
}
