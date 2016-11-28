using System.Collections.Generic;
using Assets.Scripts.Monsters;
using DejarikLibrary;

namespace Assets.Scripts.MessageModels
{
    public class AvailableMovesResponseMessage : GameMessage
    {
        public Monster SelectedMonster;
        public IEnumerable<Node> AvailableMoves;
        public IEnumerable<Node> AvailableAttacks;
    }
}
