using System;
using UnityEngine;

namespace Code.Scripts.Pathfinding
{
    public class HighlightPath : MonoBehaviour
    {
        [SerializeField] private LineHighlightController _lineHighlightController;
        private Vector3[] _points;

        private void Start()
        {
            _lineHighlightController = FindObjectOfType<LineHighlightController>();
        }

        public void SetPoints(Vector3[] points)
        {
            _points = points;
        }

        public void HighlightThePath()
        {
            _lineHighlightController.SetUpLine(_points);
        }

        public void ResetPoints()
        {
            _points = Array.Empty<Vector3>();
            _lineHighlightController.ResetLine();
        }
    }
}