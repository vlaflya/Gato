using DG.Tweening;
using UnityEngine;

public class LootBoxProgressMeter : MonoBehaviour
{
    [SerializeField]
    private Transform _filler;
    [SerializeField]
    private Transform _fillerStart;
    [SerializeField]
    private Transform _fillerEnd;

    private bool _full;
    private bool _draining;

    public void SetFull()
    {
        _full = true;
        _filler.localPosition = _fillerEnd.localPosition;
    }

    public void Fill(float percent)
    {
        if (_draining || _full)
            return;
        if (percent > 0.95f)
            _full = true;
        _filler.localPosition = Vector3.Lerp(_fillerStart.localPosition, _fillerEnd.localPosition, percent);
    }

    public void Empty()
    {
        _filler.DOLocalMove(_fillerStart.localPosition, 0.5f).OnComplete(() =>
        {
            _draining = false;
            _full = false;
        });
    }
}
