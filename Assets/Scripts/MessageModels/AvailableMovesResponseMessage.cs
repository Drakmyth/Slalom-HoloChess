using Assets.Scripts.Monsters;
using DejarikLibrary;

namespace Assets.Scripts.MessageModels
{
    public class AvailableMovesResponseMessage : GameStateMessage
    {
        public Monster SelectedMonster;
        public Node[] AvailableMoves;
        public Node[] AvailableAttacks;
    }
}
