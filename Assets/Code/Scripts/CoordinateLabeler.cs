using Code.Scripts.Pathfinding;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshPro))]
    public class CoordinateLabeler : MonoBehaviour
    {
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color blockedColor = Color.grey;
        [SerializeField] private Color exploredColor = Color.yellow;
        [SerializeField] private Color pathColor = Color.red;
        
        private TextMeshPro _label;
        private Vector2Int _coordinates = new Vector2Int();
        private GridManager _gridManager;
        
        void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();
            _label = GetComponent<TextMeshPro>();
            _label.enabled = false;
            
            DisplayCoordinates();
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                DisplayCoordinates();
                UpdateObjectName();
            }

            SetLabelColor();
            ToggleLabels();
        }

        private void ToggleLabels()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _label.enabled = !_label.IsActive();
            }
        }

        private void SetLabelColor()
        {
            if (_gridManager == null) return;

            Node node = _gridManager.GetNode(_coordinates);

            if (node == null) return;

            if (!node.isWalkable) {
                _label.color = blockedColor;
            } else if (node.isPath) {
                _label.color = pathColor;
            } else if (node.isExplored) {
                _label.color = exploredColor;
            } else {
                _label.color = defaultColor;
            }
        }

        private void DisplayCoordinates()
        {
            if (_gridManager == null) return;
            
            Vector3 parentPosition = transform.parent.position;
            _coordinates.x = Mathf.RoundToInt(parentPosition.x / _gridManager.UnityGridSize);
            _coordinates.y = Mathf.RoundToInt(parentPosition.z / _gridManager.UnityGridSize);
            
            _label.text = _coordinates.x + "," + _coordinates.y;
        }

        private void UpdateObjectName()
        {
            transform.parent.name = _coordinates.ToString();
        }
    }
}