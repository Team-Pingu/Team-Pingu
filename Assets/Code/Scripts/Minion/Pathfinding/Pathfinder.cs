using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        [SerializeField] private Vector2Int startCoordinates;
        public Vector2Int StartCoordinates { get { return startCoordinates; } }
        
        [SerializeField] private Vector2Int destinationCoordinates;
        public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }

        private Node _startNode;
        private Node _destinationNode;
        private Node _currentSearchNode;

        private Queue<Node> _frontier = new Queue<Node>();
        private Dictionary<Vector2Int, Node> _reached = new Dictionary<Vector2Int, Node>();

        private Vector2Int[] _directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        private GridManager _gridManager;
        private Dictionary<Vector2Int, Node> _grid = new Dictionary<Vector2Int, Node>();

        private void Awake()
        {
            _gridManager = FindObjectOfType<GridManager>();

            if (_gridManager != null)
            {
                _grid = _gridManager.Grid;
                _startNode = _grid[startCoordinates];
                _destinationNode = _grid[destinationCoordinates];
            }
        }

        private void Start()
        {
            GetNewPath();
        }

        public List<Node> GetNewPath()
        {
            _gridManager.ResetNodes();
            BreadthFirstSearch();
            return BuildPath();
        }

        private void ExploreNeighbours()
        {
            List<Node> neighbours = new List<Node>();

            foreach (Vector2Int direction in _directions)
            {
                Vector2Int neighbourCoords = _currentSearchNode.coordinates + direction;

                if (_grid.ContainsKey(neighbourCoords))
                {
                    neighbours.Add(_grid[neighbourCoords]);
                }
            }

            foreach (Node neighbour in neighbours)
            {
                if (!_reached.ContainsKey(neighbour.coordinates) && neighbour.isWalkable)
                {
                    neighbour.connectedTo = _currentSearchNode;
                    _reached.Add(neighbour.coordinates, neighbour);
                    _frontier.Enqueue(neighbour);
                }
            }
        }

        private void BreadthFirstSearch()
        {
            _startNode.isWalkable = true;
            _destinationNode.isWalkable = true;
            
            _frontier.Clear();
            _reached.Clear();
            
            bool isRunning = true;
            
            _frontier.Enqueue(_startNode);
            _reached.Add(startCoordinates, _startNode);

            while (_frontier.Count > 0 && isRunning)
            {
                _currentSearchNode = _frontier.Dequeue();
                _currentSearchNode.isExplored = true;
                
                ExploreNeighbours();

                if (_currentSearchNode.coordinates == destinationCoordinates)
                {
                    isRunning = false;
                }
            }
        }

        private List<Node> BuildPath()
        {
            List<Node> path = new List<Node>();
            Node currentNode = _destinationNode;
            
            path.Add(currentNode);
            currentNode.isPath = true;

            while (currentNode.connectedTo != null)
            {
                currentNode = currentNode.connectedTo;
                path.Add(currentNode);
                currentNode.isPath = true;
            }
            
            path.Reverse();
            
            return path;
        }
    }
}