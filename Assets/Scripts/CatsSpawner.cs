using System;
using UnityEngine;

public class CatsSpawner : MonoBehaviour
{
    public event Action<CatController> SpawnedCat;

    private const string PREFAB_PATH = "CatPrefab";

    public void LoadCat(CatData data)
    {
        var cat = SpawnCat();
        cat.OnDataLoaded(data);
        SpawnedCat?.Invoke(cat);
    }

    public void CreateNewCat(string catId)
    {
        var cat = SpawnCat();
        var topRightPos = Camera.main.ViewportToWorldPoint(Vector3.one) - Vector3.one * 10;
        var bottomLeftPos = Camera.main.ViewportToWorldPoint(Vector3.zero) + Vector3.one * 10;
        cat.transform.position = Vector3.zero;
        SpawnedCat?.Invoke(cat);
        cat.Initialize(catId);
    }

    private CatController SpawnCat()
    {
        var catPrefab = Resources.Load<CatController>(PREFAB_PATH);
        var catInstance = Instantiate(catPrefab);
        return catInstance;
    }
}
