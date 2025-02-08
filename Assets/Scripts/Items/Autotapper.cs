using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;
using UniRx;

public class Autotapper : MonoBehaviour, IItem
{
    [SerializeField]
    private float _time;
    [SerializeField]
    private Transform _visuals;
    [SerializeField]
    private Transform _tapPoint;
    [SerializeField]
    private float _tapDelay;
    [SerializeField]
    private ParticleSystem _particles;
    private ItemData _itemData;
    private Coroutine _tapCoroutine;

    public event Action<ItemData> DataChanged;

    public void Initialize(ItemData data)
    {
        _itemData = data;
        transform.position = new Vector3(data.X, data.Y);
        Observable.EveryUpdate().Where(_ => transform).Select(_ => transform.position).Subscribe(position =>
        {
            if (Input.mousePositionDelta.magnitude > 0)
                OnChanged();
        });
    }

    public void Spawn(string id)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f);
        _itemData = new ItemData()
        {
            UniqId = DateTime.Now.ToString(),
            Id = id,
            X = transform.position.x,
            Y = transform.position.y
        };
        DataChanged?.Invoke(_itemData);
        Observable.EveryUpdate().Where(_ => transform).Select(_ => transform.position).Subscribe(position =>
        {
            if (Input.mousePositionDelta.magnitude > 0 && Input.GetMouseButton(0))
                OnChanged();
        });
    }

    private void Start()
    {
        _tapCoroutine = StartCoroutine(TapCoroutine());
    }

    private void OnChanged()
    {
        _itemData = new ItemData()
        {
            UniqId = _itemData.UniqId,
            Id = _itemData.Id,
            X = transform.position.x,
            Y = transform.position.y
        };
        DataChanged?.Invoke(_itemData);
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSecondsRealtime(_time);
        if (_tapCoroutine != null)
        {
            StopCoroutine(_tapCoroutine);
        }
        Destroy(gameObject);
    }

    private IEnumerator TapCoroutine()
    {
        yield return new WaitForSecondsRealtime(_tapDelay);
        var colliders = Physics2D.OverlapCircleAll(_tapPoint.position, 0.1f);
        if (colliders.Length > 0)
        {
            var foundCat = false;
            foreach (var collider in colliders)
            {
                var cat = collider.GetComponentInParent<CatController>();
                if (cat != null)
                {
                    foundCat = true;
                    var seq = DOTween.Sequence();
                    seq.Append(_visuals.DOScale(Vector3.one * 0.7f, _tapDelay / 3)).SetEase(Ease.InOutQuad);
                    seq.AppendCallback(() =>
                    {
                        cat.Tap();
                        _particles.Play();
                        _tapCoroutine = StartCoroutine(TapCoroutine());
                    });
                    seq.Append(_visuals.DOScale(Vector3.one, _tapDelay / 4).SetEase(Ease.InOutQuad));
                    break;
                }
            }
            if (!foundCat)
            {
                _tapCoroutine = StartCoroutine(TapCoroutine());
            }
        }
        else
        {
            _tapCoroutine = StartCoroutine(TapCoroutine());
        }
    }
}
