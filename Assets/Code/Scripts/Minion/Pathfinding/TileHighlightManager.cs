using System;
using System.Collections.Generic;
using Code.Scripts.Player.Controller;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class TileHighlightManager : MonoBehaviour
    {
        private GridManager _gridManager;
        private Dictionary<Vector2Int, Tile> _attackerTiles = new Dictionary<Vector2Int, Tile>();
        private Dictionary<Vector2Int, Tile> _defenderTiles = new Dictionary<Vector2Int, Tile>();
        public bool highlightTiles = false;
        public bool tilesHighlighted = false;

        [SerializeField] private Color outlineColor = Color.yellow;
        [SerializeField][Range(0f, 10f)] private float outlineWidth = 10f;
        [SerializeField] private Outline.Mode outlineMode = Outline.Mode.OutlineAll;

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

                CreateOutlineOnTiles();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                highlightTiles = !highlightTiles;

                if (!highlightTiles)
                {
                    ResetTilesHighlight();
                    tilesHighlighted = false;
                }
            }

            if (highlightTiles && !tilesHighlighted)
            {
                HighlightUsableTiles();
                tilesHighlighted = true;
            }
        }

        private void CreateOutlineOnTiles(PlayerRole role = PlayerRole.Defender)
        {
            Dictionary<Vector2Int, Tile> tilesToHighlight = role switch
            {
                PlayerRole.Attacker => _attackerTiles,
                PlayerRole.Defender => _defenderTiles,
                _ => new Dictionary<Vector2Int, Tile>()
            };

            if (tilesToHighlight.Count == 0) return;
            
            foreach (KeyValuePair<Vector2Int, Tile> tilePair in tilesToHighlight)
            {
                // create outline on tile
                Transform tile = tilePair.Value.transform;
                
                if (tile.CompareTag("Selectable"))
                {
                    Outline outline = tile.gameObject.AddComponent<Outline>();
                    outline.enabled = false;
                    outline.OutlineColor = outlineColor;
                    outline.OutlineWidth = outlineWidth;
                    outline.OutlineMode = outlineMode;
                }
            }
        }

        public void HighlightUsableTiles(PlayerRole role = PlayerRole.Defender)
        {
            Dictionary<Vector2Int, Tile> tilesToHighlight = role switch
            {
                PlayerRole.Attacker => _attackerTiles,
                PlayerRole.Defender => _defenderTiles,
                _ => new Dictionary<Vector2Int, Tile>()
            };

            if (tilesToHighlight.Count == 0) return;
            
            foreach (KeyValuePair<Vector2Int, Tile> tilePair in tilesToHighlight)
            {
                // highlight the value (Tile)
                Transform tile = tilePair.Value.transform;
                
                if (tile.CompareTag("Selectable"))
                {
                    Outline outline = tile.gameObject.GetComponent<Outline>();
                    outline.enabled = true;
                    outline.OutlineColor = outlineColor;
                    outline.OutlineWidth = outlineWidth;
                    outline.OutlineMode = outlineMode;
                }
            }
        }

        public void ResetTilesHighlight(PlayerRole role = PlayerRole.Defender)
        {
            Dictionary<Vector2Int, Tile> tilesToReset = role switch
            {
                PlayerRole.Attacker => _attackerTiles,
                PlayerRole.Defender => _defenderTiles,
                _ => new Dictionary<Vector2Int, Tile>()
            };

            if (tilesToReset.Count == 0) return;
            
            
            foreach (KeyValuePair<Vector2Int, Tile> tilePair in tilesToReset)
            {
                Transform tile = tilePair.Value.transform;
                
                if (tile.CompareTag("Selectable"))
                {
                    tile.gameObject.GetComponent<Outline>().enabled = false;
                }
            }
        }
    }
}