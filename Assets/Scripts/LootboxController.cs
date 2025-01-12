using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class LootboxController : MonoBehaviour
{
    [SerializeField]
    private Transform _boxTransform;

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
    private int _currentIteration;
    private Rarity _currentRarity = Rarity.Normal;

    public event Action<string> GaveCat;
    public event Action ReadyToGiveCat;

    private const string SILUETE_PATH = "waifusSiluets/";
    private const int MAX_ITERATIONS = 6;
    private const float ITERATIONS_GIVEOUT_MULT = 2;
    private const float ITERATIONS_RARE_MULT = 3;
    private const float ITERATIONS_ULTRA_RARE_MULT = 2;
    private const int SCORE_CHECKPOINT = 500;
    private const float GIVEOUT_CHANCE = 40;
    private const float RARE_CHANCE = 40;
    private const float SRARE_CHANCE = 8;
    private const float SSRARE_CHANCE = 3;

    public void Initialize(int startScore)
    {
        _lastScore = startScore;
        _currentIteration = 1;
        _boxTransform.transform.localScale = Vector3.zero;
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
                GaveCat?.Invoke(name);
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

    public void CalculateGiveout(int score)
    {
        if (_active)
            return;
        if (score < SCORE_CHECKPOINT * _currentIteration + _lastScore)
            return;
        float giveoutChance = UnityEngine.Random.Range(0f, 100f);
        if (giveoutChance + _currentIteration * ITERATIONS_GIVEOUT_MULT > GIVEOUT_CHANCE)
        {
            _currentIteration++;
            if (_currentIteration > MAX_ITERATIONS)
                _currentIteration = MAX_ITERATIONS;
            return;
        }
        _lastScore = score;
        _currentIteration = 1;
        _active = true;
        ReadyToGiveCat?.Invoke();
        float rarityChance = UnityEngine.Random.Range(0f, 100f);
        if (rarityChance < SSRARE_CHANCE + _currentIteration)
        {
            _currentRarity = Rarity.SSRare;
            return;
        }
        if (rarityChance < SRARE_CHANCE + _currentIteration * ITERATIONS_ULTRA_RARE_MULT)
        {
            _currentRarity = Rarity.SRare;
            return;
        }
        if (rarityChance < RARE_CHANCE + _currentIteration * ITERATIONS_RARE_MULT)
        {
            _currentRarity = Rarity.Rare;
            return;
        }
        _currentRarity = Rarity.Normal;
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
