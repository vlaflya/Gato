using System;
using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    private bool _active = true;
    private bool _deleting = false;
    public event Action OnTap;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f);
    }

    private void OnMouseDown()
    {
        if (!_active)
            return;
        _active = false;
        _particleSystem.Play();
        OnTap?.Invoke();
        transform.DOPunchScale(Vector3.one, 0.3f).OnComplete(() =>
        {
            Delete();
        });
    }

    public void Delete()
    {
        if (_deleting)
            return;
        _deleting = true;
        _active = false;
        transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
