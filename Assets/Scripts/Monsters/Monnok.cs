using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Monnok : MonoBehaviour, IMonster
    {
        public int AttackRating => 6;
        public int DefenseRating => 5;
        public int MovementRating => 3;

        public Node CurrentNode { get; set; }

    }
}
