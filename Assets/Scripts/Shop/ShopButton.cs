using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShopButton : MonoBehaviour
{
    [SerializeField]
    private ShopItemData _data;
    public event Action<ShopItemData, ShopButton> OnClick;

    private bool _active = true;

    private void OnMouseDown()
    {
        if (!_active)
            return;
        OnClick?.Invoke(_data, this);
    }

    public void SetActive(bool active)
    {
        _active = active;
    }
}
