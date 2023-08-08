using Code.Scripts.Player.Controller;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public enum MarkTilesType
    {
        AllPathTilesExceptStartTiles,
        AllPathTiles,
        AllWorldTilesExceptPathTiles,
        PathStartTiles,
    }

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

        public void MarkTiles(MarkTilesType? markTilesType = null)
        {
            if (markTilesType == null)
            {
                if (_player.Role == PlayerRole.Defender)
                {
                    markTilesType = MarkTilesType.AllWorldTilesExceptPathTiles;
                } else
                {
                    markTilesType = MarkTilesType.PathStartTiles;
                }
            }

            if (markTilesType == MarkTilesType.PathStartTiles)
            {
                MarkPathStartTiles();
            } else if(markTilesType == MarkTilesType.AllPathTiles)
            {
                MarkAllPathTiles();
            } else if(markTilesType == MarkTilesType.AllPathTilesExceptStartTiles)
            {
                MarkAllPathTilesExceptStartTiles();
            }
            else if (markTilesType == MarkTilesType.AllWorldTilesExceptPathTiles)
            {
                MarkAllWorldTilesExceptPathTiles();
            }
        }

        public void MarkPathStartTiles()
        {
            foreach (var tile in _pathStartTiles)
            {
                tile.MarkTile();
                tile.SetIsSelectable(true);
            }
        }

        public void MarkAllPathTiles()
        {
            foreach (var tile in _attackerTiles.Values)
            {
                // TODO: check if first tile of path
                tile.MarkTile();
                tile.SetIsSelectable(true);
            }
        }

        public void MarkAllPathTilesExceptStartTiles()
        {
            foreach (var tile in _attackerTiles.Values)
            {
                bool isSameTile = false;
                foreach (var pathTile in _pathStartTiles)
                {
                    if (GameObject.ReferenceEquals(pathTile, tile))
                    {
                        isSameTile = true;
                        break;
                    }
                }
                if (isSameTile) continue;

                tile.MarkTile();
                tile.SetIsSelectable(true);
            }
        }

        public void MarkAllWorldTilesExceptPathTiles()
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