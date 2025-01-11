using DG.Tweening;
using UnityEngine;

public class CatView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private ParticleSystem _bounceParticles;

    public bool Flipped => _flipped;
    private bool _flipped;
    private Camera _camera;
    private Vector3 _startScale;
    private Tween _bounceTween;
    private const string SPRITE_PATH = "waifusSprites/";

    public void Initialize(string id)
    {
        _camera = Camera.main;
        _startScale = transform.localScale;
        _renderer.sprite = Resources.Load<Sprite>(SPRITE_PATH + id);
    }

    public void Flip()
    {
        _flipped = !_flipped;
        var scale = _renderer.transform.localScale;
        _startScale.x *= -1;
        scale.x *= -1;
        _renderer.transform.localScale = scale;
    }

    public void Bounce()
    {
        transform.localScale = _startScale;
        _bounceTween?.Kill();
        _bounceTween = transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        _bounceParticles.transform.position = pos;
        _bounceParticles.Play();
    }

    public void Twitch()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DORotate(Vector3.forward * 5, 0.1f));
        sequence.Append(transform.DORotate(Vector3.forward * -5, 0.1f));
        sequence.Append(transform.DORotate(Vector3.forward * 0, 0.1f));
    }
}
