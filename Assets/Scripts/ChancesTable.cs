using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChancesTable : MonoBehaviour
{
    [SerializeField]
    private TapObject _openButton;
    [SerializeField]
    private TMP_Text _givoutField;
    [SerializeField]
    private List<RarityText> _texts;

    private bool _active;
    private const int MAX_SIZE = 2;

    private void Start()
    {
        _openButton.OnClick += ChangeState;
    }

    private void ChangeState()
    {
        _active = !_active;
        if (_active)
        {
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad);
        }
        else
        {
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad);
        }
    }

    public void UpdateChances(float givoutChance, List<RarityChance> rarityChances)
    {
        _givoutField.text = $"Waifu {givoutChance}%";
        foreach (var rarityChance in rarityChances)
        {
            foreach (var text in _texts)
            {
                if (rarityChance.Rarity == text.Rarity)
                {
                    var chance = rarityChance.Chance;
                    if (chance < 0)
                        chance = 0;
                    text.TextField.text = $"{text.Name} {Math.Round(chance, MAX_SIZE)}%";
                }
            }
        }
    }

    [Serializable]
    public class RarityText
    {
        public Rarity Rarity;
        public string Name;
        public TMP_Text TextField;
    }
}

public struct RarityChance
{
    public Rarity Rarity;
    public float Chance;
}
