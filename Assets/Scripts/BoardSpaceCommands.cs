using UnityEngine;

namespace Assets.Scripts
{
    public class BoardSpaceCommands : MonoBehaviour {
        // Called by GazeGestureManager when the user performs a Select gesture
        void OnSelect()
        {
            var meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.cyan;
        }
    }
}