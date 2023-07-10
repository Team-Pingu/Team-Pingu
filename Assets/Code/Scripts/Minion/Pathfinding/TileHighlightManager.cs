using System;
using System.Collections.Generic;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class TileHighlightManager : MonoBehaviour
    {
        private GridManager _gridManager;
        private Dictionary<Vector2Int, Tile> _attackerTiles = new Dictionary<Vector2Int, Tile>();
        private Dictionary<Vector2Int, Tile> _defenderTiles = new Dictionary<Vector2Int, Tile>();

        private void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();
        }

        private void Start()
        {
            if (_gridManager != null)
            {
                _attackerTiles = _gridManager.AttackerTiles;
                _defenderTiles = _gridManager.DefenderTiles;
            }
        }

        public void HighlightUsableTiles(PlayerController.Role role = PlayerController.Role.Defender)
        {
            Dictionary<Vector2Int, Tile> highlightTiles = role switch
            {
                PlayerController.Role.Attacker => _attackerTiles,
                PlayerController.Role.Defender => _defenderTiles,
                _ => new Dictionary<Vector2Int, Tile>()
            };

            if (highlightTiles.Count == 0) return;
            
            // do the highlighting here
            foreach (KeyValuePair<Vector2Int, Tile> tile in highlightTiles)
            {
                // highlight the value (Tile)
            }
        }
    }
}