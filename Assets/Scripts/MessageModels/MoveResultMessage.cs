using Assets.Scripts.Monsters;
using DejarikLibrary;

namespace Assets.Scripts.MessageModels
{
    public class MoveResultMessage : GameStateMessage
    {
        public Monster SelectedMonster { get; set; }
        public NodePath NodePath { get; set; }
    }
}
