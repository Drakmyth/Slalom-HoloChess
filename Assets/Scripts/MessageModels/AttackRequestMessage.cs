using Assets.Scripts.Monsters;
using UnityEngine;

namespace Assets.Scripts.MessageModels
{
    public class AttackRequestMessage : GameMessage
    {
        public Monster AttackingMonster { get; set; }
        public Monster DefendingMonster { get; set; }
    }
}
