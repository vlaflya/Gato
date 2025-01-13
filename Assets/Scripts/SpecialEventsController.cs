using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpecialEventsController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    private List<CatController> _cats;
    private const float EVENT_LENGHT = 10;

    private void Start()
    {
        var camera = Camera.main;
        var position = camera.ViewportToWorldPoint(new Vector3(0.5f, 1f));
        position.z = 0;
        _particleSystem.transform.position = position;
        var shape = _particleSystem.shape;
        shape.radius = camera.ViewportToWorldPoint(Vector3.right).x;
    }

    public void StartEvent(List<CatController> cats)
    {
        _cats = cats;
        StartClickRush();
    }

    private void StartClickRush()
    {
        _particleSystem.Play();
        StartTimer();
        foreach (var cat in _cats)
        {
            cat.StartGoldRush();
        }
    }

    private void StartCakeRush()
    {
        _particleSystem.Play();
        StartTimer();
    }

    private void StartTimer()
    {
        DOVirtual.DelayedCall(EVENT_LENGHT, () =>
        {
            _particleSystem.Stop();
            foreach (var cat in _cats)
            {
                cat.EndGoldRush();
            }
        });
    }
}
