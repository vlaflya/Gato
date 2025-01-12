using System;
using DG.Tweening;
using UnityEngine;

public class LootboxButton : MonoBehaviour
{
    [SerializeField]
    private TapObject _tapObject;

    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private ParticleSystem _readyParticles;

    [SerializeField]
    private AudioSource _tapSound;

    private bool _ready;
    public event Action Tapped;

    private void Start()
    {
        _tapObject.OnClick += OnTap;
    }

    public void SetActive()
    {
        _ready = true;
        _readyParticles.Play();
        _renderer.DOFade(1, 0.5f);
    }

    private void OnTap()
    {
        if (!_ready)
            return;
        _tapSound.Play();
        _ready = false;
        _readyParticles.Stop();
        _renderer.DOFade(0, 0.5f);
        Tapped?.Invoke();
    }
}
