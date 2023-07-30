using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] public Vector2Int gridSize;
        
        [Tooltip("Unity World Grid Size - Should match UnityEditor snap settings")]
        [SerializeField] public int unityGridSize = 10;
        public int UnityGridSize { get { return unityGridSize; } }
        
        private Dictionary<Vector2Int, Node> _grid = new Dictionary<Vector2Int, Node>();
        public Dictionary<Vector2Int, Node> Grid { get { return _grid; } }

        private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
        
        private Dictionary<Vector2Int, Tile> _attackerTiles = new Dictionary<Vector2Int, Tile>();
        private Dictionary<Vector2Int, Tile> _defenderTiles = new Dictionary<Vector2Int, Tile>();
        
        public Dictionary<Vector2Int, Tile> Tiles { get { return _tiles; } }
        public Dictionary<Vector2Int, Tile> AttackerTiles { get { return _attackerTiles; } }
        public Dictionary<Vector2Int, Tile> DefenderTiles { get { return _defenderTiles; } }

        private void Awake()
        {
            GetTiles();
            CreateGrid();
        }

        public Node GetNode(Vector2Int coordinates)
        {
            if (_grid.ContainsKey(coordinates))
            {
                return _grid[coordinates];
            }

            return null;
        }

        public void BlockNode(Vector2Int coordinates)
        {
            if (_grid.ContainsKey(coordinates))
            {
                _grid[coordinates].isWalkable = false;
            }
        }

        public void ResetNodes()
        {
            foreach (KeyValuePair<Vector2Int, Node> entry in _grid)
            {
                entry.Value.connectedTo = null;
                entry.Value.isExplored = false;
                entry.Value.isPath = false;
            }
        }

        public Vector2Int GetCoordinatesFromPosition(Vector3 position)
        {
            Vector2Int coordinates = new Vector2Int();
            
            coordinates.x = Mathf.RoundToInt(position.x / unityGridSize);
            coordinates.y = Mathf.RoundToInt(position.z / unityGridSize);

            return coordinates;
        }

        public Vector3 GetPositionFromCoordinates(Vector2Int coordinates)
        {
            Vector3 position = new Vector3();

            position.x = coordinates.x * unityGridSize;
            position.z = coordinates.y * unityGridSize;

            return position;
        }

        private void GetTiles()
        {
            _tiles.Clear();
            _attackerTiles.Clear();
            _defenderTiles.Clear();

            GameObject pathParent = GameObject.FindGameObjectWithTag("Path");

            foreach (Transform child in pathParent.transform)
            {
                Tile tile = child.GetComponent<Tile>();

                if (tile != null)
                {
                    Vector3 position = tile.transform.position;
                    _tiles.Add(GetCoordinatesFromPosition(position), tile);
                    _attackerTiles.Add(GetCoordinatesFromPosition(position), tile);
                }
            }
            
            GameObject worldTilesParent = GameObject.FindGameObjectWithTag("WorldTiles");

            foreach (Transform child in worldTilesParent.transform)
            {
                Tile tile = child.GetComponent<Tile>();

                if (tile != null)
                {
                    Vector3 position = tile.transform.position;
                    _tiles.Add(GetCoordinatesFromPosition(position), tile);
                    _defenderTiles.Add(GetCoordinatesFromPosition(position), tile);
                }
            }
        }

        public void UpdateTiles()
        {
            GetTiles();
        }

        public Tile GetTile(Vector2Int tileCoordinates)
        {
            return _tiles[tileCoordinates];
        }
        
        private void CreateGrid()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector2Int coordinates = new Vector2Int(x, y);

                    if (_tiles.ContainsKey(coordinates) && _tiles[coordinates].isWalkable) // todo: maybe _attackerTiles.ContainsKey(coordinates) but should not be neccessary
                    {
                        _grid.Add(coordinates, new Node(coordinates, true));
                    }
                    else
                    {
                        _grid.Add(coordinates, new Node(coordinates, false));
                    }
                }
            }   
        }
    }
}