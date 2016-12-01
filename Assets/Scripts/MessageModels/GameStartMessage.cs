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

        public GameStartMessage(Monster[] hostMonsters, Monster[] guestMonsters)
        {
            HostMonsters = JsonUtility.ToJson(hostMonsters);
            GuestMonsters = JsonUtility.ToJson(guestMonsters);
            ActionId = 1;
            SubActionId = 1;
        }
    }
}
