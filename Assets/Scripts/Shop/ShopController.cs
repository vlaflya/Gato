using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopController : MonoBehaviour, IWindow
{
    [SerializeField]
    private TapObject _openButton;
    [SerializeField]
    private TapObject _closeButton;
    [SerializeField]
    private TMP_Text _moneyField;
    [SerializeField]
    private ParticleSystem _moneyParticles;
    [SerializeField]
    private List<ShopButton> _detailsButtons;
    [SerializeField]
    private ShopDetailsController _detailsScreen;
    [SerializeField]
    private Transform _itemSpawnPoint;

    private ShopButton _currentButton;
    private bool _active;
    private int _money;
    private Tween _stateTween;
    public event Action<int> MoneyUpdated;
    public event Action<IItem> ItemSpawned;
    public event Action<IWindow> RequestOpen;

    private const string ITEM_PATH = "Items/";
    private const int SCORE_TO_MONEY = 1;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        _moneyField.text = _money.ToString() + "<sprite=0>";
        _openButton.OnClick += ChangeState;
        _closeButton.OnClick += () => Close();
        _detailsScreen.BuyItem += BuyItem;
        foreach (var button in _detailsButtons)
        {
            button.OnClick += UpdateDetails;
        }
    }

    public void Initialize(int savedMoney)
    {
        _money = savedMoney;
    }

    public void SpawnItemsFromSave(List<ItemData> itemData)
    {
        foreach (var data in itemData)
        {
            var item = SpawnItem(data.Id);
            item.Initialize(data);
        }
    }

    public void AddMoney(int money)
    {
        _money += money;
        _moneyField.text = _money.ToString() + "<sprite=0>";
        MoneyUpdated?.Invoke(_money);
    }

    private void UpdateDetails(ShopItemData data, ShopButton shopButton)
    {
        if (!_active)
            return;
        _currentButton?.SetActive(true);
        _currentButton = shopButton;
        _currentButton.SetActive(false);
        _detailsScreen.UpdateView(data);
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

    private void BuyItem(ShopItemData data)
    {
        if (data.Cost > _money)
            return;
        _money -= data.Cost;
        _moneyField.text = _money.ToString() + "<sprite=0>";
        MoneyUpdated?.Invoke(_money);
        var item = SpawnItem(data.Id);
        item.Spawn(data.Id);
    }

    private IItem SpawnItem(string itemId)
    {
        var itemInstance = Instantiate(Resources.Load(ITEM_PATH + itemId), _itemSpawnPoint.position, Quaternion.identity);
        var item = itemInstance.GetComponent<IItem>();
        ItemSpawned?.Invoke(item);
        return item;
    }

    public bool Close()
    {
        if (!_active)
            return true;
        _active = false;
        _currentButton?.SetActive(true);
        _detailsScreen.HideView();
        _stateTween?.Kill();
        _stateTween = transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad);
        return true;
    }

    public void Open()
    {
        _active = true;
        _detailsScreen.SetActive();
        _stateTween?.Kill();
        _stateTween = transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad);
    }
}
