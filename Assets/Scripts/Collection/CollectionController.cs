using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CollectionController : MonoBehaviour, IWindow
{
    [SerializeField]
    private TapObject _stateButton;
    [SerializeField]
    private TapObject _exitButton;
    [SerializeField]
    private TapObject _forwardButton;
    [SerializeField]
    private TapObject _backButton;
    [SerializeField]
    private TMP_Text _collectionNameField;
    [SerializeField]
    private SpriteRenderer _backSprite;
    [SerializeField]
    private LootboxData _data;
    [SerializeField]
    private List<CollectionSettings> _settings;
    [SerializeField]
    private CollectionItem _itemPrefab;
    [SerializeField]
    private Transform _gridStart;
    [SerializeField]
    private Vector2Int _gridSize;
    [SerializeField]
    private Vector2 _gridOffset;

    private bool _canChange = true;
    private Tween _textTween;
    private Tween _changeCollectionTween;
    private float _gridStartPosition;
    private List<CollectionItem> _items = new List<CollectionItem>();
    private Dictionary<Collection, List<CatCollectionData>> _collectionData = new Dictionary<Collection, List<CatCollectionData>>();
    private List<string> _catIds = new List<string>();
    private bool _active;
    private int _collectionId = 0;
    public event Action<IWindow> RequestOpen;

    private void Start()
    {
        _stateButton.OnClick += ChangeState;
        _exitButton.OnClick += () => Close();
        transform.transform.localScale = Vector3.zero;
        _gridStartPosition = _gridStart.localPosition.x;
        InitializeItems();
        _forwardButton.OnClick += () => ChangeCollection(1);
        _backButton.OnClick += () => ChangeCollection(-1);
        foreach (var rarity in _data.CatsData)
        {
            foreach (var data in rarity.CatLootboxInfos)
            {
                var collectionInfo = new CatCollectionData { Name = data.Name, Rarity = rarity.Rarity, Id = data.Id };
                if (_collectionData.ContainsKey(data.Collection))
                {
                    _collectionData[data.Collection].Add(collectionInfo);
                }
                else
                {
                    _collectionData.Add(data.Collection, new List<CatCollectionData> { collectionInfo });
                }
            }
        }
    }

    private void ChangeCollection(int direction)
    {
        if (!_canChange)
            return;
        _canChange = false;
        var delaqSeq = DOTween.Sequence();
        delaqSeq.AppendInterval(0.5f);
        delaqSeq.AppendCallback(() => _canChange = true);
        _collectionId += direction;
        var maxValue = Enum.GetValues(typeof(Collection)).Cast<int>().Max();
        if (_collectionId > maxValue)
            _collectionId = 0;
        if (_collectionId < 0)
            _collectionId = maxValue;
        var gridOffsetPosition = _gridStart.localPosition;
        gridOffsetPosition.x = _gridStartPosition + 0.05f * direction;
        _changeCollectionTween?.Kill();
        var changeSeq = DOTween.Sequence();
        changeSeq.Append(_gridStart.DOLocalMoveX(_gridStartPosition - 0.05f * direction, 0.4f));
        changeSeq.AppendCallback(() => _gridStart.localPosition = gridOffsetPosition);
        changeSeq.Append(_gridStart.DOLocalMoveX(_gridStartPosition, 0.4f));
        _changeCollectionTween = changeSeq;
        UpdateItems();
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

    private void InitializeItems()
    {
        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                var item = Instantiate(_itemPrefab, _gridStart);
                item.transform.localPosition = new Vector3(x * _gridOffset.x, y * _gridOffset.y);
                _items.Add(item);
            }
        }
        Debug.Log(_items.Count);
    }

    private void UpdateItems()
    {
        var currentCollection = (Collection)_collectionId;
        var itemsData = _collectionData[currentCollection];

        foreach (var setting in _settings)
        {
            if (currentCollection == setting.Collection)
            {
                _textTween?.Kill();
                var seq = DOTween.Sequence();
                seq.Append(_collectionNameField.DOFade(0, 0.2f));
                seq.AppendCallback(() =>
                {
                    _collectionNameField.color = setting.NameColor;
                    _collectionNameField.text = setting.Name;
                    _backSprite.DOColor(setting.BackColor, 0.5f);
                });
                seq.Append(_collectionNameField.DOFade(1, 0.2f));
                _textTween = seq;
                break;
            }
        }
        for (int i = 0; i < _gridSize.x * _gridSize.y; i++)
        {
            if (i >= itemsData.Count)
            {
                _items[i].Hide();
                continue;
            }
            _items[i].gameObject.SetActive(true);
            _items[i].Initialize(itemsData[i], _catIds.Contains(itemsData[i].Id));
        }
    }

    public void SetCats(List<CatData> cats)
    {
        _catIds = new List<string>();
        foreach (var cat in cats)
        {
            _catIds.Add(cat.CatId);
        }
    }

    public bool Close()
    {
        _active = false;
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad);
        return true;
    }

    public void Open()
    {
        _active = true;
        UpdateItems();
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad);
    }

    [Serializable]
    public class CollectionSettings
    {
        public Collection Collection;
        public string Name;
        public Color NameColor;
        public Color BackColor;
    }
}
