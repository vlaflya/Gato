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
    private ScoreController _scoreController;

    [SerializeField]
    private TransparentWindow _transparentWindow;

    [SerializeField]
    private TutorialController _tutorialController;

    [SerializeField]
    private TapObject _soundButton;

    [SerializeField]
    private GameObject _soundCross;

    private bool _firstLaunch;
    private List<CatData> _data = new List<CatData>();
    private List<CatController> _cats = new List<CatController>();

    private const string CATS_SAVE_KEY = "CatsData";
    private const string FIRST_LAUNCH_SAVE_KEY = "FirstLaunch";
    private const string OVERLAY_SAVE_KEY = "Overlay";
    private const string SOUND_SAVE_KEY = "Sound";
    private const string SCORE_SAVE_KEY = "Score";

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
        LoadData();
        _soundButton.OnClick += ChangeSound;
        LoadSound();
        foreach (var catInfo in _data)
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _lootboxController.CalculateGiveout(_scoreController.TotalScore, true);
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
        _heartButton.Tapped -= GiveEventButtonClick;
        _lootboxController.AddIterration();
        _lootboxController.Reset();
        _eventsController.StartEvent(_cats);
    }

    private void OnLootBoxOpened(string catId, Rarity rarity)
    {
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
        for (int i = 0; i < _data.Count; i++)
        {
            if (data.UniqId.Equals(_data[i].UniqId))
            {
                if (_firstLaunch)
                {
                    _firstLaunch = false;
                    _tutorialController.ContinueTutorial();
                }
                _data[i] = data;
                SaveCats();
                return;
            }
        }
        _data.Add(data);
        _lootboxController.SetCatCount(_data.Count);
        SaveCats();
    }

    private void AddScore(int value)
    {
        _scoreController.AddScore(value);
        if (value == 0)
            return;
        var scoreData = new ScoreData
        {
            Score = _scoreController.TotalScore,
            X = _scoreController.transform.position.x,
            Y = _scoreController.transform.position.y,
            Scale = _scoreController.transform.localScale.y
        };
        _lootboxController.CalculateGiveout(_scoreController.TotalScore);
        PlayerPrefs.SetString(SCORE_SAVE_KEY, JsonConvert.SerializeObject(scoreData));
    }

    private void OnOverlayChanged(bool overlay)
    {
        PlayerPrefs.SetString(OVERLAY_SAVE_KEY, JsonConvert.SerializeObject(overlay));
        Debug.Log(overlay);
    }

    private void LoadData()
    {
        if (PlayerPrefs.HasKey(FIRST_LAUNCH_SAVE_KEY))
        {
            _firstLaunch = false;
        }
        else
        {
            _firstLaunch = true;
        }
        _data = new List<CatData>();
        var save = PlayerPrefs.GetString(CATS_SAVE_KEY);
        _data = JsonConvert.DeserializeObject<List<CatData>>(save);
        if (_data == null || _data.Count == 0)
        {
            _data = new List<CatData>();
        }
        _lootboxController.SetCatCount(_data.Count);
    }

    private void SaveCats()
    {
        PlayerPrefs.SetString(FIRST_LAUNCH_SAVE_KEY, "true");
        var json = JsonConvert.SerializeObject(_data);
        PlayerPrefs.SetString(CATS_SAVE_KEY, json);
    }
}
