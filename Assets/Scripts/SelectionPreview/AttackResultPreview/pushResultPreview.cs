using System.Collections.Generic;
using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts.SelectionPreview.AttackResultPreview
{
    public class PushResultPreview : MonoBehaviour {

        private float InitialX;
        private float InitialY;
        private float InitialWidth;
        private float InitialHeight;

        // Use this for initialization
        void Start()
        {
            InitialX = GetComponent<RectTransform>().rect.x;
            InitialY = GetComponent<RectTransform>().rect.y;
            InitialWidth = GetComponent<RectTransform>().rect.width;
            InitialHeight = GetComponent<RectTransform>().rect.height;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnUpdateAttackResultPreview(IDictionary<AttackResult, decimal> attackResults)
        {
            float offsetX = ((float)attackResults[AttackResult.Kill]) * InitialX;

            GetComponent<RectTransform>().rect.Set(offsetX, InitialY, InitialWidth * (float)attackResults[AttackResult.CounterPush], InitialHeight);
        }
    }
}
