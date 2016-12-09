﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{

    public class SpeechManager : MonoBehaviour
    {
        KeywordRecognizer keywordRecognizer = null;
        Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

        // Use this for initialization
        void Start()
        {
            keywords.Add("Menu", () => //menu instead?
            {
                SceneManager.LoadScene("lobby");
            });

            keywords.Add("Select", () =>
            {
                var focusObject = GazeGestureManager.Instance.FocusedObject;
                if (focusObject != null)
                {
                    // Call the OnDrop method on just the focused object.
                    var currentObject = GameObject.Find("GameState");
                    focusObject.SendMessage("OnSelected", currentObject);
                }
            });

            keywords.Add("Zoom", () => //possess?
            {
				Camera.main.GetComponent<CameraZoom>().ZoomIn();
            });


            // Tell the KeywordRecognizer about our keywords.
            keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

            // Register a callback for the KeywordRecognizer and start recognizing!
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action keywordAction;
            if (keywords.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }
        }
    }
}