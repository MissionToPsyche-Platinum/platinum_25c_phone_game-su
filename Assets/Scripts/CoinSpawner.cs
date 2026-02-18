using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] coinPrefabs;
    [SerializeField] private int[] coinValues;
    [SerializeField] private int[] coinSpawnWeights;
    private int totalWeight;

    public float minSpawnInterval = 3f;  
    public float maxSpawnInterval = 10f;  
    public float minY = -5f;             
    public float maxY = 1f;              
    public float minX = -2.5f;             
    public float maxX = 2.5f;              

    private float timer = 0f;
    private float nextSpawnTime;
    private float spawnRateMultiplier = 1f;

    private bool spawnerActive = true;

    CheckpointSpawnScript checkpointSpawnScript;

    private void Awake()
    {
        
    }

    void Start()
    {
        if(coinPrefabs.Length != coinSpawnWeights.Length){
            throw new Exception("Number of power ups and power up spawn weights do not match.");
        }

        totalWeight = 0;
        for(int i = 0; i < coinSpawnWeights.Length; i++){
            totalWeight += coinSpawnWeights[i];
        }

        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        this.gameObject.SetActive(false);

        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed += OnCheckpointPassed;

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying -= OnStopPlaying;
        checkpointSpawnScript.OnCheckpointReached -= OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed -= OnCheckpointPassed;
    }


    void Update()
    {
        if(spawnerActive){
            timer += Time.deltaTime;

            if (timer >= nextSpawnTime / spawnRateMultiplier)
            {
                SpawnCoin();
                timer = 0f;
                nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    void SpawnCoin()
    {
        
        Vector3 spawnPos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        GameObject selectedCoin = coinPrefabs[coinPrefabs.Length - 1];
        int randVal = UnityEngine.Random.Range(1, totalWeight); 
        int currSum = 0;

        for(int i = 0; i < 5; i++){
            currSum += coinSpawnWeights[i];
            if(randVal <= currSum){       
                selectedCoin = coinPrefabs[i];
                break;
            }
        }
        
        GameObject newCoin = Instantiate(selectedCoin, spawnPos, Quaternion.identity);
    }

    public void SetSpawnRateMultiplier(float newSpawnRateMultiplier){
        spawnRateMultiplier = newSpawnRateMultiplier;
    }

    private void OnStartPlaying(object sender, EventArgs e)
    {
        this.gameObject.SetActive(true);
    }
    
    private void OnStopPlaying(object sender, EventArgs e)
    {
        this.gameObject.SetActive(false);
    }

    private void OnCheckpointReached(object sender, EventArgs e)
    {
        spawnerActive = false;
    }

    private void OnCheckpointPassed(object sender, EventArgs e)
    {
        spawnerActive = true;
    }
}