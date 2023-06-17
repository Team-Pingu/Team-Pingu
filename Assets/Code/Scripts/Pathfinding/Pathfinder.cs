using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        private enum PathfindingAlgorithm
        {
            BreadthFirstSearch,
            DepthFirstSearch
        }

        [SerializeField] private PathfindingAlgorithm pathfindingAlgorithm = PathfindingAlgorithm.DepthFirstSearch;
        
        [SerializeField] private Vector2Int startCoordinates;
        public Vector2Int StartCoordinates { get { return startCoordinates; } }
        
        [SerializeField] private Vector2Int destinationCoordinates;
        public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }

        private Node _startNode;
        private Node _destinationNode;
        private Node _currentSearchNode;

        private Queue<Node> _frontierBfs = new Queue<Node>();
        private Stack<Node> _frontierDfs = new Stack<Node>();
        private Dictionary<Vector2Int, Node> _reached = new Dictionary<Vector2Int, Node>();

        private Vector2Int[] _directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        private GridManager _gridManager;
        private Dictionary<Vector2Int, Node> _grid = new Dictionary<Vector2Int, Node>();
        
        private List<List<Node>> _possiblePaths = new List<List<Node>>();

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

        public List<List<Node>> GetNewPath()
        {
            _gridManager.ResetNodes();
            
            _possiblePaths.Clear();

            switch (pathfindingAlgorithm)
            {
                case PathfindingAlgorithm.BreadthFirstSearch:
                    BreadthFirstSearch();
                    break;
                case PathfindingAlgorithm.DepthFirstSearch:
                default:
                    DepthFirstSearch();
                    break;
            }

            _possiblePaths.Sort((path1, path2) => path1.Count.CompareTo(path2.Count));

            // PrintPaths();
            
            return _possiblePaths;
        }

        /*
         * Not deleting this function for debug reasons.
         * 
         * @DEBUG
         */
        private void PrintPaths()
        {
            foreach (var path in _possiblePaths)
            {
                Debug.Log("Path:");
                String pathString = "";
                foreach (var node in path)
                {
                    pathString += node.coordinates + " ";
                }
                Debug.Log(pathString);
                Debug.Log("----------------------");
            }
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
                    // Bei BFS:
                    _frontierBfs.Enqueue(neighbour);
                    // Bei DFS:
                    _frontierDfs.Push(neighbour);
                }
            }
        }

        private void BreadthFirstSearch()
        {
            _startNode.isWalkable = true;
            _destinationNode.isWalkable = true;

            _frontierBfs.Clear();
            _reached.Clear();

            bool isRunning = true;

            _frontierBfs.Enqueue(_startNode);
            _reached.Add(startCoordinates, _startNode);

            while (_frontierBfs.Count > 0 && isRunning)
            {
                _currentSearchNode = _frontierBfs.Dequeue();
                _currentSearchNode.isExplored = true;

                ExploreNeighbours();

                if (_currentSearchNode.coordinates == destinationCoordinates)
                {
                    isRunning = false;
                }
            }
            
            List<Node> path = BuildPath();
            _possiblePaths.Add(path);
        }

        private void DepthFirstSearch()
        {
            _startNode.isWalkable = true;
            _destinationNode.isWalkable = true;

            _frontierDfs.Clear();
            _reached.Clear();

            // bool isRunning = true;
            //
            // _frontierDfs.Push(_startNode);
            // _reached.Add(startCoordinates, _startNode);
            //
            // while (_frontierDfs.Count > 0 && isRunning)
            // {
            //     _currentSearchNode = _frontierDfs.Pop();
            //     _currentSearchNode.isExplored = true;
            //
            //     ExploreNeighbours();
            //
            //     if (_currentSearchNode.coordinates == destinationCoordinates)
            //     {
            //         isRunning = false;
            //     }
            // }
            //
            // List<Node> path = BuildPath();
            // _possiblePaths.Add(path);
            
            ExplorePath(_startNode, new List<Node>());
        }
        
        private void ExplorePath(Node currentNode, List<Node> currentPath)
        {
            _reached.Add(currentNode.coordinates, currentNode);
            currentPath.Add(currentNode);
            currentNode.isExplored = true;

            if (currentNode.coordinates == destinationCoordinates)
            {
                // Hinzufügen des aktuellen Pfads zu _possiblePaths, da das Ziel erreicht wurde
                _possiblePaths.Add(new List<Node>(currentPath));
            }
            else
            {
                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (!_reached.ContainsKey(neighbour.coordinates) && neighbour.isWalkable)
                    {
                        neighbour.connectedTo = currentNode;
                        ExplorePath(neighbour, currentPath);
                    }
                }
            }

            // Entfernen des aktuellen Knotens und Zurückgehen zum vorherigen Knoten
            currentPath.Remove(currentNode);
            _reached.Remove(currentNode.coordinates);
        }

        private List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            foreach (Vector2Int direction in _directions)
            {
                Vector2Int neighbourCoords = node.coordinates + direction;

                if (_grid.ContainsKey(neighbourCoords))
                {
                    neighbours.Add(_grid[neighbourCoords]);
                }
            }

            return neighbours;
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