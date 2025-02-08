using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinEvent : MonoBehaviour, IEvent
{
    [SerializeField]
    private ShopController _shopController;
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
        _camera = Camera.main;
        _coins = new List<Coin>();
        _coinPrefab = Resources.Load<Coin>(_prefabName);
        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    public void StopEvent()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);
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
        coin.OnTap += () => _shopController.AddScore(_coinValue);
        _coins.Add(coin);
        var bottomLeft = _camera.ViewportToWorldPoint(Vector3.zero).x + 10;
        var x = Random.Range(bottomLeft, _camera.ViewportToWorldPoint(Vector3.right).x - 10);
        var y = Random.Range(bottomLeft, _camera.ViewportToWorldPoint(Vector3.up).y - 10);
        coin.transform.position = new Vector3(x, y, 0);
        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }
}
