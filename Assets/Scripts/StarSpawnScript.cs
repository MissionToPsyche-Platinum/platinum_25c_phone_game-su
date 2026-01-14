using UnityEngine;
using System;


public class StarSpawnScript : MonoBehaviour
{
    public GameObject[] starSprites;   //list of all stars that can be spawned
    public int[] starWeights; //chance of each star spawned (Ex: chance of starSprites[i] spawning is starWeights[i] / sum(starWeights))
    public float[] starSpeeds; //make closer stars move faster for parallax effect

    public float minSpawnInterval = 0.1f; // Minimum time between spawns
    public float maxSpawnInterval = 0.5f; // Maximum time between spawns
    public float moveSpeed;
    private float timer = 0f;
    private float nextSpawnTime;
    private int totalWeight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //throw an error if powerUps and powerUpSpawnWeights are different sizes
        if(starSprites.Length != starWeights.Length){
            throw new Exception("Number of power ups and power up spawn weights do not match.");
        }

        //calculate total of all weights provided
        totalWeight = 0;
        for(int i = 0; i < starWeights.Length; i++){
            totalWeight += starWeights[i];
        }

        for(int i = 0; i < 100; i++){

            Vector3 spawnPos = new Vector3(
                UnityEngine.Random.Range(-2.0f, 2.0f),  
                UnityEngine.Random.Range(-5.0f, 5.0f),                       
                1f
            );

            SpawnStar(spawnPos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= nextSpawnTime)
        {
            // Random position of spawn
            Vector3 spawnPos = new Vector3(
                UnityEngine.Random.Range(-2.0f, 2.0f),  
                5f,                       
                1f
            );

            SpawnStar(spawnPos);
            timer = 0f;
            
            nextSpawnTime = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        
    }
    
    void SpawnStar(Vector3 spawnPos)
    {
        // Random sprite
        GameObject selectedSprite = starSprites[starSprites.Length - 1];
        int randVal = UnityEngine.Random.Range(1, totalWeight); 
        int currSum = 0;

        for(int i = 0; i < 5; i++){
            currSum += starWeights[i];
            if(randVal <= currSum){             //checks if random value generated is between sum of first i-1 and first i weights
                selectedSprite = starSprites[i];
                moveSpeed = starSpeeds[i];
                break;
            }
        }
        
        // Spawn the star
        GameObject newStar = Instantiate(selectedSprite, spawnPos, Quaternion.identity);
        
        // Random speed
        DebrisMoveScript script = newStar.GetComponent<DebrisMoveScript>();
        if (script != null)
        {
            script.moveSpeed = moveSpeed;
        }
    }
}