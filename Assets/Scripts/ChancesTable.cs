using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChancesTable : MonoBehaviour, IWindow
{
    [SerializeField]
    private TapObject _openButton;
    [SerializeField]
    private TapObject _closeButton;
    [SerializeField]
    private List<RarityText> _texts;

    private Tween _stateTween;
    private bool _active;
    private const int MAX_SIZE = 2;

    public event Action<IWindow> RequestOpen;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        _openButton.OnClick += ChangeState;
        _closeButton.OnClick += () => Close();
    }

    private void ChangeState()
    {
        if (_active)
        {
            Close();
        }
        else
        {
            RequestOpen?.Invoke(this);
        }
    }

    public void UpdateChances(List<RarityChance> rarityChances)
    {
        foreach (var rarityChance in rarityChances)
        {
            foreach (var text in _texts)
            {
                if (rarityChance.Rarity == text.Rarity)
                {
                    var chance = rarityChance.Chance;
                    if (chance < 0)
                        chance = 0;
                    text.TextField.text = $"{Math.Round(chance, MAX_SIZE)}%";
                }
            }
        }
    }

    public void Open()
    {
        _active = true;
        _stateTween = transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad);
    }

    public bool Close()
    {
        if (!_active)
            return true;
        _active = false;
        _stateTween = transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad);
        return true;
    }

    [Serializable]
    public class RarityText
    {
        public Rarity Rarity;
        public TMP_Text TextField;
    }
}

public struct RarityChance
{
    public Rarity Rarity;
    public float Chance;
}
