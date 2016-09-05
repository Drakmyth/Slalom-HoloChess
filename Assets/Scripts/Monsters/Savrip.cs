using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.Monsters
{
    public class Savrip : MonoBehaviour, IMonster
    {
        public int AttackRating => 6;
        public int DefenseRating => 6;
        public int MovementRating => 2;

        public Node CurrentNode { get; set; }

    }
}
