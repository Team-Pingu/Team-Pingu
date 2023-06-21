using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    class LineHighlightController : MonoBehaviour
    {
        [SerializeField] private Color[] pathColors;
        
        private LineRenderer _lineRenderer;
        private Vector3[] _points;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void SetUpLine(Vector3[] points)
        {
            _lineRenderer.positionCount = points.Length;
            _points = points;

            for (int i = 0; i < _points.Length; i++)
            {
                _lineRenderer.SetPosition(i, _points[i]);
            }

            int index = 0;
            _lineRenderer.startColor = (index >= 0 && index < pathColors.Length) ? pathColors[index] : Color.white;
            _lineRenderer.endColor = (index >= 0 && index < pathColors.Length) ? pathColors[index] : Color.white;
        }

        public void ResetLine()
        {
            _lineRenderer.positionCount = 0;
            _points = Array.Empty<Vector3>();
        }
    }
}
