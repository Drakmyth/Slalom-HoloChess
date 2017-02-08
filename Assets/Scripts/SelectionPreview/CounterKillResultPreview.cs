using System.Collections.Generic;
using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.SelectionPreview.AttackResultPreview
{
    public class CounterKillResultPreview : MonoBehaviour {

        private float _initialY;
        private float _initialWidth;
        private float _initialHeight;

        public static CounterKillResultPreview Instance;

        // Use this for initialization
        void Start()
        {
            Instance = this;

            _initialY = GetComponent<RectTransform>().anchoredPosition.y;
            _initialWidth = GetComponent<RectTransform>().sizeDelta.x;
            _initialHeight = GetComponent<RectTransform>().sizeDelta.y;


            GetComponent<RectTransform>().sizeDelta = new Vector2(0, _initialHeight);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, _initialY);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnUpdate(IDictionary<AttackResult, decimal> attackResults)
        {
            float offsetX = (float)attackResults[AttackResult.Kill] * _initialWidth + (float)attackResults[AttackResult.Push] * _initialWidth + (float)attackResults[AttackResult.CounterPush] * _initialWidth;

            GetComponent<RectTransform>().sizeDelta = new Vector2(_initialWidth * 4 * (float)attackResults[AttackResult.CounterKill], _initialHeight);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetX, _initialY);
        }

        void OnClear()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, _initialHeight);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, _initialY);
        }
    }
}
