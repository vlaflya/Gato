using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DraggableObject : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Camera _camera;
    private bool _isDragged;
    private Vector3 _dragOffset;
    private Vector3 _targetPosition;

    private void Start()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_isDragged)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _isDragged = false;
                return;
            }
            var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition) + _dragOffset;
            mousePosition.z = 0;
            _targetPosition = mousePosition;
        }
    }

    private void FixedUpdate()
    {
        if (_isDragged)
        {
            _rigidbody.MovePosition(_targetPosition);
        }
    }

    private void OnMouseDown()
    {
        _dragOffset = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
        _isDragged = true;
    }
}
