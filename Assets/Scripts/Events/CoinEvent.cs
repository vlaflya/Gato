using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class CoinEvent : MonoBehaviour, IEvent
{
    [SerializeField]
    private ShopController _shopController;
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private string _prefabName;
    [SerializeField]
    private float _spawnDelay;
    [SerializeField]
    private int _coinValue;

    private Coroutine _spawnCoroutine;
    private Camera _camera;
    private Coin _coinPrefab;
    private List<Coin> _coins;

    public void StartEvent(List<CatController> cats)
    {
        _text.DOFade(1, 0.5f);
        _camera = Camera.main;
        _coins = new List<Coin>();
        _coinPrefab = Resources.Load<Coin>(_prefabName);
        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    public void StopEvent()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
        _text.DOFade(0, 0.5f);
        var length = _coins.Count;
        for (int i = 0; i < length; i++)
        {
            var coin = _coins[0];
            _coins.Remove(coin);
            if (coin != null)
                coin.Delete();
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSecondsRealtime(_spawnDelay);
        var coin = Instantiate(_coinPrefab);
        coin.OnTap += () => _shopController.AddMoney(_coinValue);
        _coins.Add(coin);
        var bottomLeft = _camera.ViewportToWorldPoint(Vector3.zero).x + 0.5f;
        var x = Random.Range(bottomLeft, _camera.ViewportToWorldPoint(Vector3.right).x - 0.5f);
        var y = Random.Range(bottomLeft, _camera.ViewportToWorldPoint(Vector3.up).y - 0.5f);
        coin.transform.position = new Vector3(x, y, 0);
        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }
}
