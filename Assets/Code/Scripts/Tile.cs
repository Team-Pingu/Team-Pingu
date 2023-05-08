using System;
using Code.Scripts.Pathfinding;
using UnityEngine;

namespace Code.Scripts
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] public GameObject buildingPrefab;
        [SerializeField] public bool isPlaceable;
        [SerializeField] public bool isWalkable;

        private GridManager _gridManager;
        private Vector2Int _coordinates = new Vector2Int();

        private void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();
        }

        private void Start()
        {
            if (_gridManager != null)
            {
                _coordinates = _gridManager.GetCoordinatesFromPosition(transform.position);
                
                if (!isPlaceable && !isWalkable)
                {
                    _gridManager.BlockNode(_coordinates);
                }
            }
        }

        private void OnMouseDown()
        {
            if (isPlaceable)
            {
                Instantiate(buildingPrefab, transform.position, Quaternion.identity);
                isPlaceable = false;
                _gridManager.BlockNode(_coordinates);
            }
        }
    }
}
