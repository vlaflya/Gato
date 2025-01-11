using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LootboxStars : MonoBehaviour
{
    [SerializeField]
    private List<RarityStars> _stars;

    private RarityStars _currentStars;

    public void Show(Rarity rarity)
    {
        foreach (var stars in _stars)
        {
            if (stars.Rarity == rarity)
            {
                _currentStars = stars;
            }
        }
        var seq = DOTween.Sequence();
        for (int i = 0; i < _currentStars.StarsAnimators.Count; i++)
        {
            var star = _currentStars.StarsAnimators[i];
            seq.Append(star.transform.DOScale(Vector3.one, 0.5f));
            seq.Join(star.transform.DOLocalRotate(Vector3.forward * 360, 0.5f, RotateMode.FastBeyond360));
            seq.AppendInterval(0.05f);
        }
        seq.OnComplete(() =>
        {
            foreach (var star in _currentStars.StarsAnimators)
            {
                star.SetTrigger("Start");
            }
        });
    }

    public void Hide()
    {
        for (int i = 0; i < _currentStars.StarsAnimators.Count; i++)
        {
            var star = _currentStars.StarsAnimators[i];
            star.transform.DOScale(Vector3.zero, 0.5f);
        }
    }
}

[Serializable]
public class RarityStars
{
    public Rarity Rarity;
    public List<Animator> StarsAnimators;
}
