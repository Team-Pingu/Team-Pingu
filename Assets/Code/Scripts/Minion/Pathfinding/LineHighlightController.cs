using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class LineHighlightController
    {
        private LineRenderer _lineRenderer;
        private Color _pathColor;
        private Vector3[] _points;

        // private void Awake()
        // {
        //     //_lineRenderer = GetComponent<LineRenderer>();
        //     _lineRenderer = Instantiate(new LineRenderer());
        // }

        public LineHighlightController(LineRenderer lineRenderer)
        {
            _lineRenderer = lineRenderer;
        }

        public void SetUpLine(Vector3[] points)
        {
            _lineRenderer.positionCount = points.Length;
            _points = points;

            for (int i = 0; i < _points.Length; i++)
            {
                _lineRenderer.SetPosition(i, _points[i]);
            }
        }

        public void ResetLine()
        {
            _lineRenderer.positionCount = 0;
            _points = Array.Empty<Vector3>();
        }
    }
}
