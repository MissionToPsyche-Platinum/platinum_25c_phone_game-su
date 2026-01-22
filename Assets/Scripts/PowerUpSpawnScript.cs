using UnityEngine;
using System;


public class PowerUpSpawnScript : MonoBehaviour, IListenToStartGame
{
    public GameObject[] powerUps;   //list of all power ups that can be spawned
    public int[] powerUpSpawnWeights; //chance of each power up spawned (Ex: chance of powerUps[i] spawning is powerUpSpawnWeights[i] / sum(powerUpSpawnWeights))

    public float minSpawnInterval = 10f; // Minimum time between spawns
    public float maxSpawnInterval = 15f; // Maximum time between spawns
    private float timer = 0f;
    private float nextSpawnTime;
    private int totalWeight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //throw an error if powerUps and powerUpSpawnWeights are different sizes
        if(powerUps.Length != powerUpSpawnWeights.Length){
            throw new Exception("Number of power ups and power up spawn weights do not match.");
        }

        //calculate total of all weights provided
        totalWeight = 0;
        for(int i = 0; i < powerUpSpawnWeights.Length; i++){
            totalWeight += powerUpSpawnWeights[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= nextSpawnTime)
        {
            SpawnPowerUp();
            timer = 0f;
            
            nextSpawnTime = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        
    }
    
    void SpawnPowerUp()
    {
        // Random position of spawn
        Vector3 spawnPos = new Vector3(
            UnityEngine.Random.Range(-2.0f, 2.0f),  
            10f,                       
            0f
        );

        // Random power up
        GameObject selectedPowerUp = powerUps[powerUps.Length - 1];
        int randVal = UnityEngine.Random.Range(1, totalWeight); 
        int currSum = 0;

        for(int i = 0; i < 5; i++){
            currSum += powerUpSpawnWeights[i];
            if(randVal <= currSum){             //checks if random value generated is between sum of first i-1 and first i weights
                selectedPowerUp = powerUps[i];
                break;
            }
        }
        
        // Spawn the power up
        GameObject newPowerUp = Instantiate(selectedPowerUp, spawnPos, Quaternion.identity);

        // Scale power up
        newPowerUp.transform.localScale = Vector3.one * 0.1f;
        
        // Random speed
        DebrisMoveScript script = newPowerUp.GetComponent<DebrisMoveScript>();
        if (script != null)
        {
            script.moveSpeed = UnityEngine.Random.Range(2.0f, 8.0f);
        }
    }

    public void OnStartGame()
    {
        this.gameObject.SetActive(true);
    }
}