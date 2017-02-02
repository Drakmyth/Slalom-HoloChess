using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{

    public class SpeechManager : MonoBehaviour
    {
        private KeywordRecognizer _keywordRecognizer = null;
        private readonly Dictionary<string, System.Action> _keywords = new Dictionary<string, System.Action>();

        // Use this for initialization
        void Start()
        {
            _keywords.Add("Menu", () =>
            {
                GameManager.Instance.ResetGameManager();
                SceneManager.LoadScene("lobby");
            });

            _keywords.Add("Select", () =>
            {
                var focusObject = GazeGestureManager.Instance.FocusedObject;
                if (focusObject != null)
                {
                    // Call the OnSelected method on just the focused object.
                    focusObject.SendMessage("OnSelected", ClientGameState.Instance);
                }
            });

            _keywords.Add("Grab Table", () =>
            {
                TapToPlace.Instance.UpdatePlacingStatus(true);
            });

            _keywords.Add("Move Table", () =>
            {
                TapToPlace.Instance.UpdatePlacingStatus();
            });

            _keywords.Add("Place Table", () =>
            {
                TapToPlace.Instance.UpdatePlacingStatus(false);
            });

            _keywords.Add("Zoom", () =>
            {              
                TableZoom.Instance.Zoom();
            });

            // Tell the KeywordRecognizer about our _keywords.
            _keywordRecognizer = new KeywordRecognizer(_keywords.Keys.ToArray());

            // Register a callback for the KeywordRecognizer and start recognizing!
            _keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            _keywordRecognizer.Start();
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action keywordAction;
            if (_keywords.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }
        }
    }
}