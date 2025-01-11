using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;

    private Camera _camera;
    private Vector3 _dragOffset;
    private bool _isDragged;
    public int TotalScore => _totalScore;
    private int _totalScore = 0;
    private bool _isHowered;
    private Vector3 _textStartPosition;
    private Tween _bounceTween;

    private const int MAX_SCORE = 999999999;
    private const float MIN_SCALE = 0.5f;

    public void Initialize(ScoreData data)
    {
        AddScore(data.Score);
        transform.localScale = Vector3.one * data.Scale;
        transform.position = new Vector3(data.X, data.Y, 0);
    }

    public void AddScore(int value)
    {
        _totalScore += value;
        if (_totalScore > MAX_SCORE)
            _totalScore = 0;
        _bounceTween?.Kill();
        _bounceTween = _text.transform.DOLocalJump(_textStartPosition, 0.03f, 1, 0.2f);
        _text.text = _totalScore.ToString();
    }

    private void Start()
    {
        _textStartPosition = _text.transform.localPosition;
        _camera = Camera.main;
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
            transform.position = mousePosition;
        }
        else
        {
            if (_isHowered)
            {
                var newScale = transform.localScale + Vector3.one * 0.1f * Input.mouseScrollDelta.y;
                if (newScale.y > MIN_SCALE)
                {
                    transform.localScale = newScale;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        _dragOffset = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
        _isDragged = true;
    }

    private void OnMouseOver()
    {
        _isHowered = true;
    }

    private void OnMouseExit()
    {
        _isHowered = false;
    }
}
