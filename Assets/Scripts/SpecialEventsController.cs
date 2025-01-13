using DG.Tweening;
using UnityEngine;

public class SpecialEventsController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;

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

    public void StartClickRush()
    {
        _particleSystem.Play();
        StartTimer();
    }

    public void StartCakeRush()
    {
        _particleSystem.Play();
        StartTimer();
    }

    private void StartTimer()
    {
        DOVirtual.DelayedCall(EVENT_LENGHT, () =>
        {
            _particleSystem.Stop();
        });
    }
}
