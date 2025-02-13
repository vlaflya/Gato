using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LootboxController : MonoBehaviour, IWindow
{
    [SerializeField]
    private Transform _boxTransform;

    [SerializeField]
    private LootBoxProgressMeter _meter;

    [SerializeField]
    private SpriteRenderer _siluete;

    [SerializeField]
    private ParticleSystem _backParticles;

    [SerializeField]
    private ParticleSystem _unboxParticles;

    [SerializeField]
    private LootboxStars _stars;

    [SerializeField]
    private Transform _nameBox;

    [SerializeField]
    private TMP_Text _nameField;

    [SerializeField]
    private ChancesTable _table;

    [SerializeField]
    private SpriteRenderer _rarityBack;

    [SerializeField]
    private List<RaritySettings> _raritySettings;

    [SerializeField]
    private LootboxData _data;

    [SerializeField]
    private AudioSource _startSound;

    [SerializeField]
    private AudioSource _sucessSound;

    private bool _active;
    private bool _opening;
    private int _lastScore;
    private int _catCount;
    private Rarity _currentRarity = Rarity.ERare;

    public event Action<string, Rarity> GaveCat;
    public event Action GiveLootbox;
    public event Action GiveEvent;
    public event Action<IWindow> RequestOpen;

    private const string SILUETE_PATH = "waifusSprites/";
    private const int BASE_SCORE_CHECKPOINT = 250;
    private const int SCORE_INCREMENT = 350;


    public void Initialize(int startScore)
    {
        _lastScore = startScore;
        _boxTransform.transform.localScale = Vector3.zero;
        _nameBox.localScale = Vector3.up;
    }

    public void SetCatCount(int catCount, int iterations)
    {
        _catCount = catCount;
        UpdateChancesTable(iterations);
    }

    public void SetFull()
    {
        _meter.SetFull();
    }

    public void SetEmpty()
    {
        _meter.Empty();
    }

    public void RestartIteration(int currentIteration, int score)
    {
        _lastScore = score;
        _meter.Empty();
        UpdateChancesTable(currentIteration);
    }

    public void GiveCat()
    {
        _opening = true;
        RequestOpen?.Invoke(this);
        _meter.Empty();
        RaritySettings currentSettings = null;
        foreach (var settings in _raritySettings)
        {
            if (settings.Rarity == _currentRarity)
            {
                currentSettings = settings;
            }
        }
        var name = GetRandomCat();
        _siluete.sprite = Resources.Load<Sprite>(SILUETE_PATH + name);
        _siluete.gameObject.SetActive(true);
        _siluete.transform.localScale = Vector3.zero;
        _boxTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            _startSound.Play();
            _backParticles.Play();
            DOVirtual.DelayedCall(2.5f, () => _sucessSound.Play());
            _siluete.transform.DOScale(Vector3.one * 3f, 3).OnComplete(() =>
            {
                _unboxParticles.Play();
                _siluete.gameObject.SetActive(false);
                _backParticles.Stop();
                GaveCat?.Invoke(name, _currentRarity);
                _nameBox.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutQuad);
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    var stamp = currentSettings.Stamp;
                    stamp.alpha = 0;
                    stamp.transform.localScale = Vector3.one * 4;
                    stamp.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.InOutQuad);
                    stamp.DOFade(1, 0.6f);
                });
                DOVirtual.DelayedCall(1f, () =>
                {
                    _stars.Show(currentSettings.Rarity);
                });
                DOVirtual.DelayedCall(5f, () =>
                {
                    _stars.Hide();
                });
                currentSettings.Particles.Play();
                _rarityBack.color = currentSettings.BackSpriteColor;
                _rarityBack.DOFade(1, 0.8f);
                DOVirtual.DelayedCall(5.5f, () =>
                {
                    currentSettings.Stamp.transform.localScale = Vector3.zero;
                    _rarityBack.DOFade(0, 0.3f);
                    _opening = false;
                    _active = false;
                    _boxTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad);
                    _nameBox.DOScale(Vector3.up, 0.2f).SetEase(Ease.InOutQuad);
                });
            });
        });
    }

    public void CalculateGiveout(int score, int currentIterration, bool test = false)
    {
        if (!test)
        {
            var targetScore = BASE_SCORE_CHECKPOINT + (currentIterration - 1) * SCORE_INCREMENT;
            _meter.Fill((float)(score - _lastScore) / targetScore);
            if (_active)
            {
                _lastScore = score;
                return;
            }
            if (score - _lastScore < targetScore)
            {
                return;
            }
            float chance = UnityEngine.Random.Range(0f, 100f);
            var giveoutChance = ChancesHelper.CalculateGiveoutChance(currentIterration, _catCount);
            if (chance > giveoutChance)
            {
                GiveEvent?.Invoke();
                _lastScore = score;
                return;
            }
        }
        _lastScore = score;
        _active = true;
        GiveLootbox?.Invoke();
        float rarityChance = UnityEngine.Random.Range(0f, 100f);
        if (rarityChance < ChancesHelper.CalculateBChance(currentIterration))
        {
            _currentRarity = Rarity.BRare;
            return;
        }
        if (rarityChance < ChancesHelper.CalculateCChance(currentIterration))
        {
            _currentRarity = Rarity.CRare;
            return;
        }
        if (rarityChance < ChancesHelper.CalculateDChance(currentIterration))
        {
            _currentRarity = Rarity.DRare;
            return;
        }
        _currentRarity = Rarity.ERare;
    }

    private string GetRandomCat()
    {
        foreach (var catData in _data.CatsData)
        {
            if (catData.Rarity == _currentRarity)
            {
                var r = UnityEngine.Random.Range(0, catData.CatLootboxInfos.Count);
                string id = catData.CatLootboxInfos[r].Id;
                _nameField.text = catData.CatLootboxInfos[r].Name;
                return id;
            }
        }
        return null;
    }

    public void UpdateChancesTable(int currentIteration)
    {
        var givoutChance = ChancesHelper.CalculateGiveoutChance(currentIteration, _catCount);
        var s = ChancesHelper.CalculateSChance(currentIteration);
        var a = ChancesHelper.CalculateAChance(currentIteration) - s;
        var b = ChancesHelper.CalculateBChance(currentIteration) - a - s;
        var c = ChancesHelper.CalculateCChance(currentIteration) - b - a - s;
        var d = ChancesHelper.CalculateDChance(currentIteration) - c - b - a - s;
        var e = 100 - d - c - b - a - s;
        _table.UpdateChances(new List<RarityChance>
        {
            new RarityChance{
                Rarity = Rarity.ERare,
                Chance = e * givoutChance / 100
            },
            new RarityChance{
                Rarity = Rarity.DRare,
                Chance = d * givoutChance / 100
            },
            new RarityChance{
                Rarity = Rarity.CRare,
                Chance = c * givoutChance / 100
            },
            new RarityChance{
                Rarity = Rarity.BRare,
                Chance = b * givoutChance / 100
            },
            new RarityChance{
                Rarity = Rarity.ARare,
                Chance = a * givoutChance / 100
            },
            new RarityChance{
                Rarity = Rarity.SRare,
                Chance = s * givoutChance / 100
            }
        });
    }

    public void Open()
    {

    }

    public bool Close()
    {
        return !_opening;
    }
}

[Serializable]
public class RaritySettings
{
    public Rarity Rarity;
    public Color BackSpriteColor;
    public TMP_Text Stamp;
    public ParticleSystem Particles;
}

public enum Rarity
{
    ERare,
    DRare,
    CRare,
    BRare,
    ARare,
    SRare
}
