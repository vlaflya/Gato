using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;

public class AppController : MonoBehaviour
{
    [SerializeField]
    private TapObject _exitButton;

    [SerializeField]
    private TapObject _clearButton;

    [SerializeField]
    private LootboxButton _heartButton;

    [SerializeField]
    private CatsSpawner _spawner;

    [SerializeField]
    private SpecialEventsController _eventsController;

    [SerializeField]
    private LootboxController _lootboxController;

    [SerializeField]
    private IterationStars _iterationStars;

    [SerializeField]
    private ScoreController _scoreController;

    [SerializeField]
    private ShopController _shopController;

    [SerializeField]
    private ChancesTable _chancesTable;

    [SerializeField]
    private TransparentWindow _transparentWindow;

    [SerializeField]
    private TutorialController _tutorialController;

    [SerializeField]
    private TapObject _soundButton;

    [SerializeField]
    private GameObject _soundCross;

    private bool _firstLaunch;
    private IWindow _currentWindow;
    private int _currentIteration = 1;
    private const int MAX_ITERATIONS = 25;
    private List<CatData> _catData = new List<CatData>();
    private List<CatController> _cats = new List<CatController>();
    private List<ItemData> _itemData = new List<ItemData>();
    private List<IItem> _items = new List<IItem>();

    private const string CATS_SAVE_KEY = "CatsData";
    private const string MONEY_SAVE_KEY = "Money";
    private const string FIRST_LAUNCH_SAVE_KEY = "FirstLaunch";
    private const string OVERLAY_SAVE_KEY = "Overlay";
    private const string SOUND_SAVE_KEY = "Sound";
    private const string SCORE_SAVE_KEY = "Score";
    private const string ITEM_SAVE_KEY = "ItemData";

    private void Awake()
    {
        _exitButton.OnClick += Application.Quit;
        _clearButton.OnClick += () =>
        {
            PlayerPrefs.DeleteAll();
            Application.Quit();
        };
        _transparentWindow.Initialize();
        _transparentWindow.OvelayChanged += OnOverlayChanged;
        _lootboxController.GaveCat += OnLootBoxOpened;
        _lootboxController.GiveLootbox += ReadyLootbox;
        _lootboxController.GiveEvent += ReadyEvent;
        _spawner.SpawnedCat += OnSpawnedCat;
        _soundButton.OnClick += ChangeSound;
        _shopController.MoneyUpdated += SaveMoney;
        _shopController.ItemSpawned += OnItemSpawned;
        LoadCatData();
        LoadMoney();
        LoadSound();
        LoadItems();
        InitializeWindows();
        foreach (var catInfo in _catData)
        {
            _spawner.LoadCat(catInfo);
        }
        LoadScore();
        LoadOverlay();
        if (_firstLaunch)
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                _tutorialController.StartTutorial();
                ReadyLootbox();
            });
        }
    }

    private void InitializeWindows()
    {
        _lootboxController.RequestOpen += ChangeWindow;
        _shopController.RequestOpen += ChangeWindow;
        _chancesTable.RequestOpen += ChangeWindow;
    }

    private void ChangeWindow(IWindow window)
    {
        if (_currentWindow == null)
        {
            _currentWindow = window;
            window.Open();
        }
        else
        {
            if (_currentWindow.Close())
            {
                _currentWindow = window;
                window.Open();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _lootboxController.CalculateGiveout(_scoreController.TotalScore, _currentIteration, true);
        }
    }

    private void ReadyLootbox()
    {
        _heartButton.SetActive();
        _heartButton.Tapped += GiveLootboxButtonClick;
    }

    private void ReadyEvent()
    {
        _heartButton.SetActive();
        _heartButton.Tapped += GiveEventButtonClick;
    }

    private void GiveLootboxButtonClick()
    {
        _heartButton.Tapped -= GiveLootboxButtonClick;
        if (_firstLaunch)
            _tutorialController.ContinueTutorial();
        _lootboxController.GiveCat();
    }

    private void GiveEventButtonClick()
    {
        _currentIteration++;
        if (_currentIteration > MAX_ITERATIONS)
            _currentIteration = MAX_ITERATIONS;
        _iterationStars.SetIteration(_currentIteration - 1);
        _heartButton.Tapped -= GiveEventButtonClick;
        _lootboxController.UpdateChancesTable(_currentIteration);
        _lootboxController.SetEmpty();
        _eventsController.StartRandomEvent(_cats);
    }

    private void OnLootBoxOpened(string catId, Rarity rarity)
    {
        _currentIteration = 1;
        _iterationStars.Clear();
        if (_firstLaunch)
            _tutorialController.ContinueTutorial();
        _spawner.CreateNewCat(catId, rarity);
    }

    private void ChangeSound()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
        _soundCross.SetActive(AudioListener.volume == 0);
        PlayerPrefs.SetFloat(SOUND_SAVE_KEY, AudioListener.volume);
    }

    private void LoadSound()
    {
        if (PlayerPrefs.HasKey(SOUND_SAVE_KEY))
        {
            AudioListener.volume = PlayerPrefs.GetFloat(SOUND_SAVE_KEY);
            _soundCross.SetActive(AudioListener.volume == 0);
        }
        else
        {
            AudioListener.volume = 1;
            _soundCross.SetActive(false);
        }
    }

    private void LoadScore()
    {
        if (PlayerPrefs.HasKey(SCORE_SAVE_KEY))
        {
            var scoreData = JsonConvert.DeserializeObject<ScoreData>(PlayerPrefs.GetString(SCORE_SAVE_KEY));
            _scoreController.Initialize(scoreData);
            _lootboxController.Initialize(scoreData.Score);
            _lootboxController.SetEmpty();
        }
        else
        {
            _lootboxController.Initialize(0);
            _lootboxController.SetFull();
            AddScore(0);
        }
    }

    private void LoadMoney()
    {
        if (PlayerPrefs.HasKey(MONEY_SAVE_KEY))
        {
            var money = PlayerPrefs.GetInt(MONEY_SAVE_KEY);
            _shopController.Initialize(money);
        }
        else
        {
            _shopController.Initialize(0);
        }
    }

    private void LoadOverlay()
    {
        if (PlayerPrefs.HasKey(OVERLAY_SAVE_KEY))
        {
            var overlay = JsonConvert.DeserializeObject<bool>(PlayerPrefs.GetString(OVERLAY_SAVE_KEY));
            _transparentWindow.SetOverlay(overlay);
        }
        else
        {
            _transparentWindow.SetOverlay(true);
        }
    }

    private void OnSpawnedCat(CatController catController)
    {
        _cats.Add(catController);
        catController.GeneratedScore += AddScore;
        catController.UpdatedData += OnCatUpdated;
    }

    private void OnCatUpdated(CatData data)
    {
        for (int i = 0; i < _catData.Count; i++)
        {
            if (data.UniqId.Equals(_catData[i].UniqId))
            {
                if (_firstLaunch)
                {
                    _firstLaunch = false;
                    _tutorialController.ContinueTutorial();
                }
                _catData[i] = data;
                SaveCats();
                return;
            }
        }
        _catData.Add(data);
        _lootboxController.SetCatCount(_catData.Count, _currentIteration);
        SaveCats();
    }

    private void OnItemSpawned(IItem item)
    {
        _items.Add(item);
        item.DataChanged += OnItemUpdated;
    }

    private void OnItemUpdated(ItemData data)
    {
        if (data.ItemType == ItemType.Timed)
            return;
        for (int i = 0; i < _itemData.Count; i++)
        {
            if (data.UniqId.Equals(_itemData[i].UniqId))
            {
                _itemData[i] = data;
                SaveItems();
                return;
            }
        }
        _itemData.Add(data);
        SaveItems();
    }

    private void LoadItems()
    {
        if (PlayerPrefs.HasKey(ITEM_SAVE_KEY))
        {
            _itemData = JsonConvert.DeserializeObject<List<ItemData>>(PlayerPrefs.GetString(ITEM_SAVE_KEY));
            _shopController.SpawnItemsFromSave(_itemData);
        }
        else
        {
            _itemData = new List<ItemData>();
        }
    }

    private void AddScore(int value)
    {
        _scoreController.AddScore(value);
        var scoreData = new ScoreData
        {
            Score = _scoreController.TotalScore,
            X = _scoreController.transform.position.x,
            Y = _scoreController.transform.position.y,
            Scale = _scoreController.transform.localScale.y
        };
        _shopController.AddScore(1);
        _lootboxController.CalculateGiveout(_scoreController.TotalScore, _currentIteration);
        PlayerPrefs.SetString(SCORE_SAVE_KEY, JsonConvert.SerializeObject(scoreData));
    }

    private void OnOverlayChanged(bool overlay)
    {
        PlayerPrefs.SetString(OVERLAY_SAVE_KEY, JsonConvert.SerializeObject(overlay));
        Debug.Log(overlay);
    }

    private void LoadCatData()
    {
        if (PlayerPrefs.HasKey(FIRST_LAUNCH_SAVE_KEY))
        {
            _firstLaunch = false;
        }
        else
        {
            _firstLaunch = true;
        }
        _catData = new List<CatData>();
        var save = PlayerPrefs.GetString(CATS_SAVE_KEY);
        _catData = JsonConvert.DeserializeObject<List<CatData>>(save);
        if (_catData == null || _catData.Count == 0)
        {
            _catData = new List<CatData>();
        }
        _lootboxController.SetCatCount(_catData.Count, _currentIteration);
    }

    private void SaveCats()
    {
        PlayerPrefs.SetString(FIRST_LAUNCH_SAVE_KEY, "true");
        var json = JsonConvert.SerializeObject(_catData);
        PlayerPrefs.SetString(CATS_SAVE_KEY, json);
    }

    private void SaveItems()
    {
        var json = JsonConvert.SerializeObject(_itemData);
        PlayerPrefs.SetString(ITEM_SAVE_KEY, json);
    }

    private void SaveMoney(int money)
    {
        PlayerPrefs.SetInt(MONEY_SAVE_KEY, money);
    }
}
