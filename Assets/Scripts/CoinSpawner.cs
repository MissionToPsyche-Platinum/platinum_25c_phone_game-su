using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour, IListenToStartGame
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

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    void Start()
    {
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
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

    public void OnStartGame()
    {
        this.gameObject.SetActive(true);
    }
}