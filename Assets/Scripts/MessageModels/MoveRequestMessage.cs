using Assets.Scripts.Monsters;
using DejarikLibrary;

namespace Assets.Scripts.MessageModels
{
    public class MoveRequestMessage : GameMessage
    {
        public Monster SelectedMonster { get; set; }
        public Node DestinationNode { get; set; }
    }
}
