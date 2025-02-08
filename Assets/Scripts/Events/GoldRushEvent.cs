using System.Collections.Generic;
using UnityEngine;

public class GoldRushEvent : MonoBehaviour, IEvent
{
    [SerializeField]
    private ParticleSystem _particles;

    [SerializeField]
    private float _eventTime;

    private List<CatController> _cats;

    private void Start()
    {
        var camera = Camera.main;
        var position = camera.ViewportToWorldPoint(new Vector3(0.5f, 1f));
        position.z = 0;
        _particles.transform.position = position;
        var shape = _particles.shape;
        shape.radius = camera.ViewportToWorldPoint(Vector3.right).x;
    }

    public void StartEvent(List<CatController> cats)
    {
        _cats = cats;
        _particles.Play();
        foreach (var cat in _cats)
        {
            cat.StartGoldRush();
        }
    }

    public void StopEvent()
    {
        _particles.Stop();
        foreach (var cat in _cats)
        {
            cat.EndGoldRush();
        }
    }
}
