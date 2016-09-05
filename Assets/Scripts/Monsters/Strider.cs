using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Strider : MonoBehaviour, IMonster
    {
        public int AttackRating => 2;
        public int DefenseRating => 7;
        public int MovementRating => 3;

        public Node CurrentNode { get; set; }

    }
}
