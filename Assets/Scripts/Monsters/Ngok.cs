using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Ngok : MonoBehaviour, IMonster
    {
        public int AttackRating => 3;
        public int DefenseRating => 8;
        public int MovementRating => 1;

        public Node CurrentNode { get; set; }

    }
}
