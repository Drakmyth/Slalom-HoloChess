using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Molator : MonoBehaviour, IMonster
    {
        public int AttackRating => 8;
        public int DefenseRating => 2;
        public int MovementRating => 2;

        public Node CurrentNode { get; set; }

    }
}
