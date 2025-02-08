using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IterationStars : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _stars;

    public void SetIteration(int iteration)
    {
        for (int i = 0; i < _stars.Count; i++)
        {
            if (i < iteration)
                _stars[i].DOScale(Vector3.one * 0.4f, 0.5f);
            else
                _stars[i].DOScale(Vector3.zero, 0.5f);
        }
    }

    public void Clear()
    {
        foreach (var star in _stars)
        {
            star.DOScale(Vector3.zero, 0.5f);
        }
    }
}
