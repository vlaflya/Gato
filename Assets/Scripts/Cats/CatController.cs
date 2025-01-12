using System;
using System.Collections;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections.Generic;

public class CatController : MonoBehaviour
{
    [SerializeField]
    private CatView _view;
    [SerializeField]
    private float _passiveDelay;
    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private AudioSource _tapSound;
    [SerializeField]
    private List<RaritySettings> _raritySettings;

    private Rarity _rarity;
    private RaritySettings _currentSettings;
    private Camera _camera;
    private Coroutine _passiveScoreGenerationCoroutine;
    private Coroutine _tapCoroutine;
    private bool _isDragged;
    private bool _isHowered;
    private Vector3 _dragOffset;
    private Vector3 _targetPosition;
    private Vector3 _targetScale;
    private string _id;
    private string _uniqId;
    private bool _canTap;
    private bool _canMove;
    private const float MIN_SCALE = 1f;
    private const float MAX_SCALE = 20f;
    private const float TAP_TIME = 0.1f;

    public event Action<CatData> UpdatedData;
    public event Action<int> GeneratedScore;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        Observable.EveryUpdate().Where(_ => transform).Select(_ => transform.position).Subscribe(position =>
        {
            if (Input.mousePositionDelta.magnitude > 0)
                OnChanged();
        });
        DOVirtual.DelayedCall(4f, () =>
        {
            _canMove = true;
        });
        _canTap = true;
        _camera = Camera.main;
        _passiveScoreGenerationCoroutine = StartCoroutine(GenerateScore());
        _targetScale = transform.localScale;
    }

    public void Initialize(string id, Rarity rarity)
    {
        _rarity = rarity;
        _currentSettings = GetCurrentSettings();
        _id = id;
        _uniqId = DateTime.Now.ToString();
        _view.Initialize(id, _currentSettings.TapParticles);
        _view.Bounce();
        _view.Twitch();
        OnChanged();
    }

    public void OnDataLoaded(CatData data)
    {
        _canMove = true;
        _rarity = data.Rarity;
        _currentSettings = GetCurrentSettings();
        _id = data.CatId;
        _uniqId = data.UniqId;
        _view.Initialize(data.CatId, _currentSettings.TapParticles);
        transform.position = new Vector3(data.X, data.Y, 0);
        transform.localScale = Vector3.one * data.Scale;
        if (data.Flipped)
            _view.Flip();
    }

    private RaritySettings GetCurrentSettings()
    {
        foreach (var settings in _raritySettings)
        {
            if(settings.Rarity == _rarity){
                return settings;
            }
        }
        return null;
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
        else
        {
            if (_isHowered)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    var newScale = transform.localScale + Vector3.one * 0.3f * Input.mouseScrollDelta.y;
                    if (newScale.y > MIN_SCALE && newScale.y < MAX_SCALE)
                    {
                        _targetScale = newScale;
                        OnChanged();
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isDragged)
        {
            _rigidbody.MovePosition(_targetPosition);
        }
        else
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }
        if (_isHowered)
        {
            transform.localScale = _targetScale;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _view.Bounce();
            _dragOffset = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
            if (_canMove)
                _isDragged = true;
            if (_canTap)
            {
                _canTap = false;
                _tapCoroutine = StartCoroutine(TapDelay());
                GeneratedScore?.Invoke(_currentSettings.ScoreValue);
                if (!_tapSound.isPlaying)
                {
                    _tapSound.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    _tapSound.Play();
                }
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
            Flipped = _view.Flipped,
            Rarity = _currentSettings.Rarity
        });
    }

    private void OnMouseExit()
    {
        _isHowered = false;
    }

    private IEnumerator GenerateScore()
    {
        yield return new WaitForSecondsRealtime(_passiveDelay);
        GeneratedScore?.Invoke(_currentSettings.ScoreValue);
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

    [Serializable]
    public class RaritySettings
    {
        public Rarity Rarity;
        public int ScoreValue;
        public ParticleSystem TapParticles;
    }
}
