using System.Collections.Generic;
using Assets.Scripts.Monsters;
using UnityEngine;

namespace Assets.Scripts.MessageModels
{
    public class GameStartMessage: GameStateMessage
    {
        public string HostMonsters { get; set; } //TODO: Net Serialize
        public string GuestMonsters { get; set; } //TODO: Net Serialize

        public override short MessageTypeId
        {
            get { return CustomMessageTypes.GameStart; }
        }


        public GameStartMessage()
        {
            HostMonsters = "";
            GuestMonsters = "";
            ActionId = 1;
            SubActionId = 1;
        }

        public GameStartMessage(List<Monster> hostMonsters, List<Monster> guestMonsters)
        {
            // ridiculous that JsonUtility doesn't support arrays unless nested in an object
            MonsterListWrapper hostWrapper = new MonsterListWrapper
            {
                Monsters = hostMonsters
            };

            MonsterListWrapper guestWrapper = new MonsterListWrapper
            {
                Monsters = guestMonsters
            };

            HostMonsters = JsonUtility.ToJson(hostWrapper);
            GuestMonsters = JsonUtility.ToJson(guestWrapper);
            ActionId = 1;
            SubActionId = 1;
        }
    }
}
