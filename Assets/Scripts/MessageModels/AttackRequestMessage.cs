using Assets.Scripts.Monsters;
using UnityEngine;

namespace Assets.Scripts.MessageModels
{
    public class AttackRequestMessage : GameStateMessage
    {
        public Monster AttackingMonster { get; set; }
        public Monster DefendingMonster { get; set; }
    }
}
