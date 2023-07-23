using System.Collections.Generic;
using System;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class HighlightPath : MonoBehaviour
    {
        [SerializeField] private Color[] pathColors;

        private GameObject _lineRenderersParent;
        private List<LineHighlightController> _lineHighlightController = new List<LineHighlightController>();
        private List<Vector3[]>  _points = new List<Vector3[]>();
        private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
        private int _pathCount = 0;

        private void Start()
        {
            //_lineHighlightController = FindObjectOfType<LineHighlightController>();

            _lineRenderersParent = new GameObject("LineRenderers");
            
            for (int i = 0; i < _pathCount; i++)
            {
                _lineHighlightController.Add(new LineHighlightController(CreateLineRenderer()));
                _points.Add(Array.Empty<Vector3>());
            }
        }

        public void SetPathCount(int pathCount)
        {
            _pathCount = pathCount;
        }

        public void SetPoints(int pathIndex, Vector3[] points)
        {
            _points[pathIndex] = points;
        }

        public void HighlightThePath(int pathIndex)
        {
            _lineHighlightController[pathIndex].SetUpLine(_points[pathIndex]);
        }

        public void ResetPoints(int pathIndex)
        {
            _points[pathIndex] = Array.Empty<Vector3>();
            _lineHighlightController[pathIndex].ResetLine();
        }

        private LineRenderer CreateLineRenderer()
        {
            var gObjectName = "LinePath" + _lineRenderers.Count;
            var gObject = new GameObject(gObjectName);
            
            // moving it to our parent
            gObject.transform.parent = _lineRenderersParent.transform;
            
            var lineRenderer = gObject.AddComponent<LineRenderer>();
            
            var index = _lineRenderers.Count;
            var color = index >= 0 && index < pathColors.Length ? pathColors[index] : Color.white;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            
            _lineRenderers.Add(lineRenderer);
            return lineRenderer;
        }
    }
}