using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Houjix : MonoBehaviour, IMonster
    {
        public int AttackRating => 4;
        public int DefenseRating => 4;
        public int MovementRating => 1;

        public Node CurrentNode { get; set; }

    }
}
