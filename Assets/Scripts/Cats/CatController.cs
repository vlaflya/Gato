using System;
using System.Collections;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField]
    private CatView _view;
    [SerializeField]
    private int _scoreValue;
    [SerializeField]
    private float _passiveDelay;

    private Camera _camera;
    private Coroutine _passiveScoreGenerationCoroutine;
    private Coroutine _tapCoroutine;
    private bool _isDragged;
    private bool _isHowered;
    private Vector3 _dragOffset;
    private string _id;
    private string _uniqId;
    private bool _canTap;
    private const float MIN_SCALE = 0.5f;
    private const float TAP_TIME = 0.1f;

    public event Action<CatData> UpdatedData;
    public event Action<int> GeneratedScore;

    private void Start()
    {
        _canTap = true;
        _camera = Camera.main;
        _passiveScoreGenerationCoroutine = StartCoroutine(GenerateScore());
    }

    public void Initialize(string id)
    {
        _id = id;
        _uniqId = DateTime.Now.ToString();
        _view.Initialize(id);
        _view.Bounce();
        _view.Twitch();
        OnChanged();
    }

    public void OnDataLoaded(CatData data)
    {
        _id = data.CatId;
        _uniqId = data.UniqId;
        _view.Initialize(data.CatId);
        transform.position = new Vector3(data.X, data.Y, 0);
        transform.localScale = Vector3.one * data.Scale;
        if (data.Flipped)
            _view.Flip();
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
            if (Input.mousePositionDelta.magnitude > 0)
                OnChanged();
        }
        else
        {
            if (_isHowered)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    var newScale = transform.localScale + Vector3.one * 0.1f * Input.mouseScrollDelta.y;
                    if (newScale.y > MIN_SCALE)
                    {
                        transform.localScale = newScale;
                        OnChanged();
                    }
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _view.Bounce();
            _dragOffset = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
            _isDragged = true;
            if (_canTap)
            {
                _canTap = false;
                _tapCoroutine = StartCoroutine(TapDelay());
                GeneratedScore?.Invoke(_scoreValue);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            _view.Flip();
            OnChanged();

        }
        _isHowered = true;
    }

    private void OnChanged()
    {
        UpdatedData.Invoke(new CatData
        {
            CatId = _id,
            UniqId = _uniqId,
            X = transform.position.x,
            Y = transform.position.y,
            Scale = transform.localScale.y,
            Flipped = _view.Flipped
        });
    }

    private void OnMouseExit()
    {
        _isHowered = false;
    }

    private IEnumerator GenerateScore()
    {
        yield return new WaitForSecondsRealtime(_passiveDelay);
        GeneratedScore?.Invoke(_scoreValue);
        _passiveScoreGenerationCoroutine = StartCoroutine(GenerateScore());
    }

    private IEnumerator TapDelay()
    {
        yield return new WaitForSecondsRealtime(TAP_TIME);
        _canTap = true;
    }

    private void OnDestroy()
    {
        if (_passiveScoreGenerationCoroutine != null)
            StopCoroutine(_passiveScoreGenerationCoroutine);
        if (_tapCoroutine != null)
            StopCoroutine(_tapCoroutine);
    }
}
