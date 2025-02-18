using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChestEvent : MonoBehaviour, IEvent
{
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private GameObject _chest;
    [SerializeField]
    private TapObject _tapObject;
    [SerializeField]
    private ShopController _shopController;
    [SerializeField]
    private int _tapsNeeded;
    [SerializeField]
    private int _money;
    [SerializeField]
    private SpriteRenderer _chestIdle;
    [SerializeField]
    private List<Sprite> _tapSprites;
    [SerializeField]
    private Animator _openChest;
    [SerializeField]
    private ParticleSystem _particles;

    private int _taps;
    private int _spriteCount;
    private bool _active;
    private bool _opening;

    public void StartEvent(List<CatController> cats)
    {
        _taps = 0;
        _active = true;
        _opening = false;
        _text.DOFade(1, 0.5f);
        _chestIdle.gameObject.SetActive(true);
        _openChest.gameObject.SetActive(false);
        _chest.SetActive(true);
        _chestIdle.DOFade(1, 0.5f);
        _tapObject.OnClick += OnChestTap;
    }

    public void StopEvent()
    {
        if (!_opening)
            Hide();
    }

    private void OnChestTap()
    {
        if (_opening || !_active)
            return;
        _taps++;
        if (_taps > _tapsNeeded)
        {
            _opening = true;
            _text.DOFade(0, 0.5f);
            _chestIdle.gameObject.SetActive(false);
            _openChest.gameObject.SetActive(true);
            _openChest.SetTrigger("Open");
            var seq = DOTween.Sequence();
            seq.Append(DOVirtual.DelayedCall(0.5f, _particles.Play));
            seq.Append(DOVirtual.DelayedCall(2f, () =>
            {
                _shopController.AddMoney(_money);
                Hide();
            }));
        }
        else
        {
            _spriteCount++;
            if (_spriteCount >= _tapSprites.Count)
                _spriteCount = 0;
            _chestIdle.sprite = _tapSprites[_spriteCount];
        }
    }

    private void Hide()
    {
        if (!_active)
            return;
        _active = false;
        DOVirtual.DelayedCall(2, () => _chest.SetActive(false));
        if (_opening)
        {
            _openChest.SetTrigger("Hide");
        }
        else
        {
            _chestIdle.DOFade(0, 0.5f);
        }
    }
}
