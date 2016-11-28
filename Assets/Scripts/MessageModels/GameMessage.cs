using UnityEngine;

namespace Assets.Scripts.MessageModels
{
    public class GameMessage : MonoBehaviour
    {
        public int ActionId { get; set; }
        public int SubActionId { get; set; }
        public string Message { get; set; }
    }
}
