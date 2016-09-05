using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Ghhhk : MonoBehaviour, IMonster
    {
        public int AttackRating => 4;
        public int DefenseRating => 3;
        public int MovementRating => 2;

        public Node CurrentNode { get; set; }

    }
}
