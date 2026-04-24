using UnityEngine;
using System;
using UnityEditor;


public class PowerUpSpawnScript : MonoBehaviour
{
    public GameObject[] powerUps;   //list of all power ups that can be spawned
    public int[] powerUpSpawnWeights; //chance of each power up spawned (Ex: chance of powerUps[i] spawning is powerUpSpawnWeights[i] / sum(powerUpSpawnWeights))

    public float minSpawnInterval = 10f; // Minimum time between spawns
    public float maxSpawnInterval = 15f; // Maximum time between spawns
    public float minSpeed;
    public float maxSpeed;
    private float timer = 0f;
    private float nextSpawnTime;
    private int totalWeight;

    private bool spawnerActive = true;

    CheckpointSpawnScript checkpointSpawnScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.SetActive(false);
        //throw an error if powerUps and powerUpSpawnWeights are different sizes
        if(powerUps.Length != powerUpSpawnWeights.Length){
            throw new Exception("Number of power ups and power up spawn weights do not match.");
        }

        //calculate total of all weights provided
        totalWeight = 0;
        for(int i = 0; i < powerUpSpawnWeights.Length; i++){
            totalWeight += powerUpSpawnWeights[i];
        }

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

    // Update is called once per frame
    void Update()
    {
        if(spawnerActive){
            timer += Time.deltaTime;
            
            if (timer >= nextSpawnTime)
            {
                SpawnPowerUp();
                timer = 0f;
                
                nextSpawnTime = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
        
    }
    
    void SpawnPowerUp()
    {
        // Random position of spawn
        Vector3 spawnPos = new Vector3(
            UnityEngine.Random.Range(-2.0f, 2.0f),  
            UnityEngine.Random.Range(0f, 4f),                       
            0f
        );

        // Random power up
        GameObject selectedPowerUp = powerUps[powerUps.Length - 1];
        float[] effectiveWeights = new float[powerUpSpawnWeights.Length];
        float effectiveTotalWeight = 0f;
        for (int i = 0; i < powerUpSpawnWeights.Length; i++)
        {
            effectiveWeights[i] = powerUpSpawnWeights[i] * PowerUpUpgradeManager.Instance.GetSpawnMultiplier(i);
            effectiveTotalWeight += effectiveWeights[i];
        }

        float randVal = UnityEngine.Random.Range(0f, effectiveTotalWeight);
        float currSum = 0f;
        for (int i = 0; i < powerUpSpawnWeights.Length; i++)
        {
            currSum += effectiveWeights[i];
            if (randVal <= currSum)
            {
                selectedPowerUp = powerUps[i];
                break;
            }
        }
        
        // Spawn the power up
        GameObject newPowerUp = Instantiate(selectedPowerUp, spawnPos, Quaternion.identity);
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