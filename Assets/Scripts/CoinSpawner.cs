using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public float minSpawnInterval = 3f;  
    public float maxSpawnInterval = 10f;  
    public float minY = -5f;             
    public float maxY = 1f;              
    public float minX = -2.5f;             
    public float maxX = 2.5f;              

    private float timer = 0f;
    private float nextSpawnTime;
    private float spawnRateMultiplier = 1f;

    private void Awake()
    {
    }

    void Start()
    {
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        this.gameObject.SetActive(false);
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime / spawnRateMultiplier)
        {
            SpawnCoin();
            timer = 0f;
            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnCoin()
    {
        
        Vector3 spawnPos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        
        Instantiate(coinPrefab, spawnPos, Quaternion.identity);
    }

    public void SetSpawnRateMultiplier(float newSpawnRateMultiplier){
        spawnRateMultiplier = newSpawnRateMultiplier;
    }

    private void OnStartPlaying(object sender, EventArgs e)
    {
        this.gameObject.SetActive(true);
    }
}