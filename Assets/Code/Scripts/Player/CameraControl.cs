using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [SerializeField] private float _speedMove = 1.0f;
    [SerializeField] private float _speedZoom = 15.0f;
    [SerializeField] private float _smoothing = 5.0f;
    [SerializeField] private Vector2 _rangeMove = new Vector2(100, 100);
    [SerializeField] private Vector2 _rangeZoom = new Vector2(10, 25);

    private Vector3 _targetPosition;
    private Vector3 _inputMove;
    private float _inputZoom;
    private Camera _camera;
    private Quaternion _rotation = Quaternion.Euler(0f, 45f, 0f);

    private void Awake() {
        _camera = GetComponentInChildren<Camera>();
        _targetPosition = transform.position;
    }

    private void HandleInput() {
        //get WASD input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //get ScrollWeel input
        _inputZoom = Input.GetAxisRaw("Mouse ScrollWheel");


        Vector3 right = transform.right * x;
        Vector3 forward = transform.forward * z;

        _inputMove = (_rotation * (forward + right)).normalized;
    }

    private void Move() {
        Vector3 nextTargetPosition = _targetPosition + _inputMove * _speedMove;
        if (IsInMoveBound(nextTargetPosition)) _targetPosition = nextTargetPosition;
        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _smoothing);
    }

    private void Zoom() {
        _camera.orthographicSize -= Input.GetAxisRaw("Mouse ScrollWheel") * _speedZoom;
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, _rangeZoom.x, _rangeZoom.y);
    }

    private bool IsInMoveBound(Vector3 position) {
        return position.x > -_rangeMove.x &&
            position.x < _rangeMove.x &&
            position.z > -_rangeMove.y &&
            position.z < _rangeMove.y;
    }

    void Update() {
        HandleInput();
        Move();
        Zoom();
    }
}
