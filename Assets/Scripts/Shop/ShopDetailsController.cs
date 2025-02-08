using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShopDetailsController : MonoBehaviour
{
    [SerializeField]
    private Transform _closedPoint;
    [SerializeField]
    private Transform _openedPoint;
    [SerializeField]
    private TMP_Text _detailsField;
    [SerializeField]
    private TMP_Text _costField;
    [SerializeField]
    private TapObject _buyButton;

    private Sequence _showTween;
    private bool _active;
    private bool _shown;

    public ShopItemData _currentItemData;

    public event Action<ShopItemData> BuyItem;

    private void Start()
    {
        transform.localPosition = _closedPoint.localPosition;
        _buyButton.OnClick += OnButtonTap;
    }

    private void OnButtonTap()
    {
        if (!_shown)
            return;
        BuyItem?.Invoke(_currentItemData);
    }

    public void SetActive()
    {
        transform.localPosition = _closedPoint.localPosition;
        _active = true;
    }

    public void UpdateView(ShopItemData data)
    {
        if (!_active)
            return;
        _currentItemData = data;
        _showTween?.Kill();
        _showTween = DOTween.Sequence();
        if (_shown)
        {
            _showTween.Append(CloseTween());
            _showTween.AppendCallback(() => SetData(data));
            _showTween.Append(OpenTween());
        }
        else
        {
            _shown = true;
            SetData(data);
            _showTween.Append(OpenTween());
        }
    }

    public void HideView()
    {
        _shown = false;
        _active = false;
        _showTween?.Kill();
        _showTween = DOTween.Sequence();
        _showTween.Append(CloseTween());
    }

    private void SetData(ShopItemData data)
    {
        _detailsField.text = data.Text;
        _costField.text = data.Cost.ToString() + "<sprite=0>";
    }

    private Tween OpenTween()
    {
        return transform.DOLocalMove(_openedPoint.localPosition, 0.5f).SetEase(Ease.InOutQuad);
    }

    private Tween CloseTween()
    {
        return transform.DOLocalMove(_closedPoint.localPosition, 0.3f).SetEase(Ease.InOutQuad);
    }
}
