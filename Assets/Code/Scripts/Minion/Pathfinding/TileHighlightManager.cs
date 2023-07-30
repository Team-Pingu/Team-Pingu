using Code.Scripts.Player.Controller;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class TileHighlightManager : MonoBehaviour
    {
        private GridManager _gridManager;
        private Player.Controller.Player _player;
        private Dictionary<Vector2Int, Tile> _attackerTiles = new Dictionary<Vector2Int, Tile>();
        private Dictionary<Vector2Int, Tile> _defenderTiles = new Dictionary<Vector2Int, Tile>();
        private List<Tile> _pathStartTiles;
        private Pathfinder _pathfinder;


        private void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();
            _player = FindObjectOfType<Player.Controller.Player>();
            _pathfinder = FindObjectOfType<Pathfinder>();
        }

        private void Start()
        {
            if (_gridManager != null)
            {
                _attackerTiles = _gridManager.AttackerTiles;
                _defenderTiles = _gridManager.DefenderTiles;
            }

            _pathStartTiles = _pathfinder.GetAllPathsStartTiles();
        }

        public void MarkTiles()
        {
            if (_player.Role == PlayerRole.Attacker)
            {
                MarkAttackerPlacableTiles();
            }

            if (_player.Role == PlayerRole.Defender)
            {
                MarkDefenderPlacableTiles();
            }
        }

        public void MarkAttackerPlacableTiles()
        {
            //foreach (var tile in _attackerTiles.Values)
            //{
            //    // TODO: check if first tile of path
            //    tile.MarkTile();
            //    tile.SetIsSelectable(true);
            //}
            foreach (var tile in _pathStartTiles)
            {
                tile.MarkTile();
                tile.SetIsSelectable(true);
            }
        }

        public void MarkDefenderPlacableTiles()
        {
            foreach (var tile in _defenderTiles.Values)
            {
                if (!tile.isPlaceable) continue;
                tile.MarkTile();
                tile.SetIsSelectable(true);
            }
        }

        public void ResetMarkTiles()
        {
            foreach (var tile in _attackerTiles.Values)
            {
                tile.UnmarkTile();
                tile.SetIsSelectable(false);
                tile.DisableOutline();
            }
            foreach (var tile in _defenderTiles.Values)
            {
                tile.UnmarkTile();
                tile.SetIsSelectable(false);
                tile.DisableOutline();
            }
        }
    }
}