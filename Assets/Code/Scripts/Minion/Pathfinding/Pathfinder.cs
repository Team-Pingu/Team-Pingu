using System;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private CoordinatePair[] _coordinatePairs;
        
        [SerializeField] private PathfindingAlgorithm pathfindingAlgorithm = PathfindingAlgorithm.DepthFirstSearch;

        private Node _startNode;
        private Node _destinationNode;
        private Node _currentSearchNode;
        
        private Vector2Int _startCoordinate;
        private Vector2Int _destinationCoordinate;

        private Queue<Node> _frontierBfs = new Queue<Node>();
        private Stack<Node> _frontierDfs = new Stack<Node>();
        private Dictionary<Vector2Int, Node> _reached = new Dictionary<Vector2Int, Node>();

        private Vector2Int[] _directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        private GridManager _gridManager;
        private Dictionary<Vector2Int, Node> _grid = new Dictionary<Vector2Int, Node>();
        
        private List<List<Node>> _possiblePaths = new List<List<Node>>();
        
        [SerializeField] public bool highlightPath = false;
        private HighlightPath _highlight;
        private List<bool> _isPathHighlighted = new List<bool>();

        private void Awake()
        {
            if (_coordinatePairs.Length == 0)
            {
                throw new Exception("Add at least one CoordinatePair to the Pathfinding Script!");
            }
            
            _gridManager = FindObjectOfType<GridManager>();

            if (_gridManager != null)
            {
                _grid = _gridManager.Grid;
                // _startNode = _grid[_coordinatePairs[0].startCoordinate];
                // _destinationNode = _grid[_coordinatePairs[0].destinationCoordinate];
            }

            _highlight = FindObjectOfType<HighlightPath>();
        }

        private void Start()
        {
            GetNewPath();

            _highlight.SetPathCount(_possiblePaths.Count);
            
            for (var i = 0; i < _possiblePaths.Count; i++)
            {
                _isPathHighlighted.Add(false);
            }
            
            // Debug.Log(_isPathHighlighted.Count);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                highlightPath = !highlightPath;
            }

            if (_highlight && highlightPath)
            {
                for (var index = 0; index < _isPathHighlighted.Count; index++)
                {
                    // Debug.Log(index);
                    if (!_isPathHighlighted[index])
                    {
                        var path = _possiblePaths[index];
                        Vector3[] points = GetPointsOfPath(index, path);

                        _highlight.SetPoints(index ,points);
                        _highlight.HighlightThePath(index);
                        _isPathHighlighted[index] = true;
                    }
                    
                }
            }

            if (!highlightPath)
            {
                for (var index = 0; index < _possiblePaths.Count; index++)
                {
                    _highlight.ResetPoints(index);
                    _isPathHighlighted[index] = false;
                }
            }
        }

        private Vector3[] GetPointsOfPath(int pathIndex, List<Node> path)
        {
            Vector3[] points = new Vector3[path.Count];

            for (var i = 0; i < path.Count; i++)
            {
                Vector2Int coords = path[i].coordinates;
                Vector3 point = _gridManager.GetTile(coords).transform.position;
                
                // shifting the position a little bit above the tile, otherwise we get parts of lines disappearing in tiles
                point.y += 0.5f;
                
                // shifting x and z according to the number of the path to place them next to each other instead of laying them on each other
                // TODO: remove if we only show one path at once -> would be easier because we don't need to shift the lines
                // not needed in final map, because there is only certain paths
                // point.x += pathIndex * 0.2f;
                // point.z -= pathIndex * 0.2f;
                
                points[i] = point;
            }

            return points;
        }

        public List<List<Node>> GetNewPath()
        {
            _gridManager.ResetNodes();
            
            _possiblePaths.Clear();

            foreach (CoordinatePair coordinatePair in _coordinatePairs)
            {
                _startNode = _grid[coordinatePair.startCoordinate];
                _startCoordinate = coordinatePair.startCoordinate;
                _destinationNode = _grid[coordinatePair.destinationCoordinate];
                _destinationCoordinate = coordinatePair.destinationCoordinate;
                
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
        
        private void PrintPath(List<Node> path)
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
            _reached.Add(_startCoordinate, _startNode);

            while (_frontierBfs.Count > 0 && isRunning)
            {
                _currentSearchNode = _frontierBfs.Dequeue();
                _currentSearchNode.isExplored = true;

                ExploreNeighbours();

                if (_currentSearchNode.coordinates == _destinationCoordinate)
                {
                    isRunning = false;
                }
            }
            
            List<Node> path = BuildPath();
            _possiblePaths.Add(new List<Node>(path));
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

            if (currentNode.coordinates == _destinationCoordinate)
            {
                // PrintPath(currentPath);
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