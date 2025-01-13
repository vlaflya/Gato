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
    private int _currentUnluck = 0;
    private Rarity _currentRarity = Rarity.Normal;

    public event Action<string, Rarity> GaveCat;
    public event Action GiveLootbox;
    public event Action GiveEvent;

    private const string SILUETE_PATH = "waifusSiluets/";
    private const int MAX_ITERATIONS = 13;
    private const float ITERATIONS_GIVEOUT_MULT = 10;
    private const float ITERATIONS_RARE_MULT = 3;
    private const float ITERATIONS_SRARE_MULT = 0.8f;
    private const float ITERATIONS_SSRARE_MULT = 0.05f;
    private const float UNLUCK_GIVEOUT_MULT = 0.005f;
    private const int BASE_SCORE_CHECKPOINT = 250;
    private const int SCORE_INCREMENT = 350;
    private const float BASE_GIVEOUT_CHANCE = 95;
    private const float CAT_GIVEOUT_CHANCE_SUBSTRACT = 5;
    private const float MIN_GIVEOUT_CHANCE = 30;
    private const float RARE_CHANCE = 20;
    private const float SRARE_CHANCE = -1f;
    private const float SSRARE_CHANCE = -0.15f;

    public void Initialize(int startScore)
    {
        _lastScore = startScore;
        _currentIteration = 1;
        _boxTransform.transform.localScale = Vector3.zero;
    }

    public void SetCatCount(int catCount)
    {
        _catCount += catCount;
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
    }

    public void Reset()
    {
        _active = false;
        _meter.Empty();
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

    public void GiveCat()
    {
        _meter.Empty();
        _currentIteration = 1;
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
                return;
            if (score - _lastScore < targetScore)
            {
                return;
            }
            float chance = UnityEngine.Random.Range(0f, 100f);
            var giveoutChance = BASE_GIVEOUT_CHANCE - CAT_GIVEOUT_CHANCE_SUBSTRACT * _catCount;
            giveoutChance = Mathf.Max(giveoutChance, MIN_GIVEOUT_CHANCE);
            if (chance + _currentIteration * ITERATIONS_GIVEOUT_MULT > giveoutChance)
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
        if (rarityChance < SSRARE_CHANCE + _currentIteration * ITERATIONS_SSRARE_MULT)
        {
            _currentRarity = Rarity.SSRare;
            _currentUnluck = 0;
            return;
        }
        if (rarityChance < SRARE_CHANCE + _currentIteration * ITERATIONS_SRARE_MULT + _currentUnluck * UNLUCK_GIVEOUT_MULT)
        {
            _currentRarity = Rarity.SRare;
            _currentUnluck = 0;
            return;
        }
        if (rarityChance < RARE_CHANCE + _currentIteration * ITERATIONS_RARE_MULT)
        {
            _currentRarity = Rarity.Rare;
            return;
        }
        _currentRarity = Rarity.Normal;
        _currentUnluck++;
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
    Normal,
    Rare,
    SRare,
    SSRare
}
