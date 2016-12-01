using Assets.Scripts.Monsters;

namespace Assets.Scripts.MessageModels
{
    public class GameStartMessage: GameStateMessage
    {
        public Monster[] HostMonsters { get; set; }
        public Monster[] GuestMonsters { get; set; }

        public GameStartMessage()
        {
            HostMonsters = new Monster[0];
            GuestMonsters = new Monster[0];
            MessageTypeId = CustomMessageTypes.GameState;
            ActionId = 1;
            SubActionId = 1;
        }

        public GameStartMessage(Monster[] hostMonsters, Monster[] guestMonsters)
        {            
            HostMonsters = hostMonsters;
            GuestMonsters = guestMonsters;
            MessageTypeId = CustomMessageTypes.GameState;
            ActionId = 1;
            SubActionId = 1;
        }
    }
}
