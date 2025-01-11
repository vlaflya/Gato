using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TapObject : MonoBehaviour
{
    private Vector3 _startScale;
    private Tween _bounceTween;
    public event Action OnClick;

    private void Start()
    {
        _startScale = transform.localScale;
    }

    private void OnMouseDown()
    {
        transform.localScale = _startScale;
        _bounceTween?.Kill();
        _bounceTween = transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        OnClick?.Invoke();
    }
}
