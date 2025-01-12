using System;
using UnityEngine;

public class CatsSpawner : MonoBehaviour
{
    public event Action<CatController> SpawnedCat;

    private const string PREFAB_PATH = "CatPrefab";

    private void Start()
    {
        var camera = Camera.main;
        var block = new GameObject();
        var edge = block.AddComponent<EdgeCollider2D>();
        var rigidBody = block.AddComponent<Rigidbody2D>();
        rigidBody.bodyType = RigidbodyType2D.Static;
        edge.SetPoints(new System.Collections.Generic.List<Vector2>
        {
            camera.ViewportToWorldPoint(Vector3.zero),
            camera.ViewportToWorldPoint(Vector3.up),
            camera.ViewportToWorldPoint(Vector3.one),
            camera.ViewportToWorldPoint(Vector3.right),
            camera.ViewportToWorldPoint(Vector3.zero),
        });
    }

    public void LoadCat(CatData data)
    {
        var cat = SpawnCat();
        cat.OnDataLoaded(data);
        SpawnedCat?.Invoke(cat);
    }

    public void CreateNewCat(string catId, Rarity rarity)
    {
        var cat = SpawnCat();
        var topRightPos = Camera.main.ViewportToWorldPoint(Vector3.one) - Vector3.one * 10;
        var bottomLeftPos = Camera.main.ViewportToWorldPoint(Vector3.zero) + Vector3.one * 10;
        cat.transform.position = Vector3.zero;
        SpawnedCat?.Invoke(cat);
        cat.Initialize(catId, rarity);
    }

    private CatController SpawnCat()
    {
        var catPrefab = Resources.Load<CatController>(PREFAB_PATH);
        var catInstance = Instantiate(catPrefab);
        return catInstance;
    }
}
