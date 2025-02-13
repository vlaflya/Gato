using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectionItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _backSprite;
    [SerializeField]
    private SpriteRenderer _waifuSprite;
    [SerializeField]
    private SpriteRenderer _rarityBack;
    [SerializeField]
    private List<RaritySettings> _raritySettings = new List<RaritySettings>();
    [SerializeField]
    private TMP_Text _nameField;
    [SerializeField]
    private float _tweenTime;

    private Tween _fadeTween;
    private const string SILUETE_PATH = "waifusSprites/";

    public void Initialize(CatCollectionData info, bool active)
    {
        _fadeTween?.Kill();
        var seq = DOTween.Sequence();
        seq.Append(_backSprite.DOFade(0, _tweenTime));
        seq.Join(_waifuSprite.DOFade(0, _tweenTime));
        seq.Join(_rarityBack.DOFade(0, _tweenTime));
        seq.Join(_nameField.DOFade(0, _tweenTime));
        seq.AppendCallback(() =>
        {
            _nameField.text = info.Name;
            _waifuSprite.sprite = Resources.Load<Sprite>($"{SILUETE_PATH}{info.Id}");
            _waifuSprite.color = active ? Color.white : Color.black;
            foreach (var settings in _raritySettings)
            {
                if (settings.Rarity == info.Rarity)
                {
                    _rarityBack.color = settings.BackColor;
                    break;
                }
            }
        });
        seq.Append(_backSprite.DOFade(1, _tweenTime));
        seq.Join(_waifuSprite.DOFade(1, _tweenTime));
        seq.Join(_rarityBack.DOFade(1, _tweenTime));
        seq.Join(_nameField.DOFade(1, _tweenTime));
        _fadeTween = seq;
    }

    public void Hide()
    {
        _fadeTween?.Kill();
        var seq = DOTween.Sequence();
        seq.Append(_backSprite.DOFade(0, _tweenTime));
        seq.Join(_waifuSprite.DOFade(0, _tweenTime));
        seq.Join(_rarityBack.DOFade(0, _tweenTime));
        seq.Join(_nameField.DOFade(0, _tweenTime));
    }

    [Serializable]
    public class RaritySettings
    {
        public Rarity Rarity;
        public Color BackColor;
    }
}

public struct CatCollectionData
{
    public Rarity Rarity;
    public string Name;
    public string Id;
}
