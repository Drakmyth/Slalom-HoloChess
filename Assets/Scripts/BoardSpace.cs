using System.Collections.Generic;
using System.Linq;
using DejarikLibrary;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoardSpace : MonoBehaviour
    {
        public Node Node { get; set; }
        public GameObject SelectionIndicatorPrefab;
        private GameObject _selectionIndicatorInstance;
        private Color OriginalColor { get; set; }
        private Color SelectionYellow { get; set; }

        // Use this for initialization
        void Start()
        {
            SelectionYellow = new Color(1f, .89f, 0f, 1f);
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            OriginalColor = meshRenderer.material.color;
            Quaternion selectionIndicatorQuaternion =
                Quaternion.Euler(SelectionIndicatorPrefab.transform.rotation.eulerAngles.x,
                    SelectionIndicatorPrefab.transform.rotation.eulerAngles.y,
                    SelectionIndicatorPrefab.transform.rotation.eulerAngles.z);
            _selectionIndicatorInstance = Instantiate(SelectionIndicatorPrefab,
                new Vector3(transform.localPosition.x, SelectionIndicatorPrefab.transform.localPosition.y, transform.localPosition.z),
                selectionIndicatorQuaternion);
            _selectionIndicatorInstance.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnAvailableMonsters(IEnumerable<int> availableNodeIds)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            meshRenderer.material.color = availableNodeIds.Contains(Node.Id) ? SelectionYellow : OriginalColor;
        }

        void OnAvailableAttacks(IEnumerable<int> availableNodeIds)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (availableNodeIds.Contains(Node.Id))
            {
                meshRenderer.material.color = Color.red;
            }
            else if (meshRenderer.material.color == Color.red)
            {
                meshRenderer.material.color = OriginalColor;
            }
        }

        void OnAvailableMoves(IEnumerable<int> availableNodeIds)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (availableNodeIds.Contains(Node.Id))
            {
                meshRenderer.material.color = Color.green;
            }
            else if (meshRenderer.material.color == Color.green)
            {
                meshRenderer.material.color = OriginalColor;
            }
        }

        void OnMonsterSelected(int nodeId)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            _selectionIndicatorInstance.SetActive(nodeId == Node.Id);
        }

        void OnClearHighlighting()
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            meshRenderer.material.color = OriginalColor;

            _selectionIndicatorInstance.SetActive(false);
        }

        void OnClearHighlightingWithSelection(Node selectedNode)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer.material.color != SelectionYellow)
            {
                meshRenderer.material.color = OriginalColor;
            }

            if (selectedNode != null && selectedNode.Equals(Node))
            {
                _selectionIndicatorInstance.SetActive(true);
                meshRenderer.material.color = SelectionYellow;
            }
            else
            {
                _selectionIndicatorInstance.SetActive(false);
            }
        }

        void OnSelected(GameObject gameStateObject)
        {
            gameStateObject.SendMessage("OnSpaceSelected", Node.Id);
        }

        void OnPreviewEnter(GameObject gameStateObject)
        {
            gameStateObject.SendMessage("OnPreviewEnter", Node.Id);
        }

        void OnSpeechSelected(GameObject gameStateObject)
        {
            gameStateObject.SendMessage("OnSpaceSelected", Node.Id);
        }
    }
}