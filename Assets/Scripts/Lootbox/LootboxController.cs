using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LootboxController : MonoBehaviour
{
    [SerializeField]
    private Transform _boxTransform;

    [SerializeField]
    private LootBoxProgressMeter _meter;

    [SerializeField]
    private IterationStars _iterationStars;

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
    private ChancesTable _table;

    [SerializeField]
    private List<RaritySettings> _raritySettings;

    [SerializeField]
    private LootboxData _data;

    [SerializeField]
    private AudioSource _startSound;

    [SerializeField]
    private AudioSource _sucessSound;

    private bool _active;
    private int _lastScore;
    private int _catCount;
    private int _currentIteration;
    // private int _currentUnluck = 0;
    private Rarity _currentRarity = Rarity.ERare;

    public event Action<string, Rarity> GaveCat;
    public event Action GiveLootbox;
    public event Action GiveEvent;

    private const string SILUETE_PATH = "waifus/";
    private const int MAX_ITERATIONS = 25;
    // private const float UNLUCK_GIVEOUT_MULT = 0.005f;
    private const int BASE_SCORE_CHECKPOINT = 250;
    private const int SCORE_INCREMENT = 350;


    public void Initialize(int startScore)
    {
        _lastScore = startScore;
        _boxTransform.transform.localScale = Vector3.zero;
        _nameBox.localScale = Vector3.up;
    }

    public void SetCatCount(int catCount)
    {
        _catCount = catCount;
        _currentIteration = 1;
        UpdateChancesTable();
    }

    public void SetFull()
    {
        _meter.SetFull();
    }

    public void SetEmpty()
    {
        _meter.Empty();
    }

    public void AddIterration()
    {
        _iterationStars.SetIteration(_currentIteration - 1);
        _currentIteration++;
        UpdateChancesTable();
    }

    public void Reset()
    {
        _meter.Empty();
    }

    public void GiveCat()
    {
        _meter.Empty();
        _iterationStars.Clear();
        RaritySettings currentSettings = null;
        foreach (var settings in _raritySettings)
        {
            if (settings.Rarity == _currentRarity)
            {
                currentSettings = settings;
            }
        }
        var name = GetRandomCat(currentSettings);
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
                DOVirtual.DelayedCall(1f, () =>
                {
                    _stars.Show(currentSettings.Rarity);
                });
                DOVirtual.DelayedCall(5f, () =>
                {
                    _stars.Hide();
                });
                currentSettings.Particles.Play();
                currentSettings.BackSprite.DOFade(1, 0.8f);
                DOVirtual.DelayedCall(5.5f, () =>
                {
                    currentSettings.BackSprite.DOFade(0, 0.3f);
                    _active = false;
                    _boxTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutQuad);
                    _nameBox.DOScale(Vector3.up, 0.2f).SetEase(Ease.InOutQuad);
                });
            });
        });
    }

    public void CalculateGiveout(int score, bool test = false)
    {
        if (!test)
        {
            var targetScore = BASE_SCORE_CHECKPOINT + (_currentIteration - 1) * SCORE_INCREMENT;
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
            var giveoutChance = ChancesHelper.CalculateGiveoutChance(_currentIteration, _catCount);
            if (chance > giveoutChance)
            {
                GiveEvent?.Invoke();
                _lastScore = score;
                if (_currentIteration > MAX_ITERATIONS)
                    _currentIteration = MAX_ITERATIONS;
                return;
            }
        }
        _lastScore = score;
        _active = true;
        GiveLootbox?.Invoke();
        float rarityChance = UnityEngine.Random.Range(0f, 100f);
        if (rarityChance < ChancesHelper.CalculateBChance(_currentIteration))
        {
            _currentRarity = Rarity.BRare;
            return;
        }
        if (rarityChance < ChancesHelper.CalculateCChance(_currentIteration))
        {
            _currentRarity = Rarity.CRare;
            return;
        }
        if (rarityChance < ChancesHelper.CalculateDChance(_currentIteration))
        {
            _currentRarity = Rarity.DRare;
            return;
        }
        _currentRarity = Rarity.ERare;
    }

    private string GetRandomCat(RaritySettings settings)
    {
        foreach (var catData in _data.CatsData)
        {
            if (catData.Rarity == _currentRarity)
            {
                var r = UnityEngine.Random.Range(0, catData.Count + 1);
                string name = settings.BaseName + r.ToString();
                return name;
            }
        }
        return null;
    }

    private void UpdateChancesTable()
    {
        _table.UpdateChances(ChancesHelper.CalculateGiveoutChance(_currentIteration, _catCount), new List<RarityChance>
        {
            new RarityChance{
                Rarity = Rarity.ERare,
                Chance = ChancesHelper.CalculateEChance(_currentIteration)
            },
            new RarityChance{
                Rarity = Rarity.DRare,
                Chance = ChancesHelper.CalculateDChance(_currentIteration) - ChancesHelper.CalculateCChance(_currentIteration) - ChancesHelper. CalculateBChance(_currentIteration)
            },
            new RarityChance{
                Rarity = Rarity.CRare,
                Chance = ChancesHelper.CalculateCChance(_currentIteration) - ChancesHelper.CalculateBChance(_currentIteration)
            },
            new RarityChance{
                Rarity = Rarity.BRare,
                Chance = ChancesHelper.CalculateBChance(_currentIteration)
            },
        });
    }


}

[Serializable]
public class RaritySettings
{
    public Rarity Rarity;
    public string BaseName;
    public SpriteRenderer BackSprite;
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
