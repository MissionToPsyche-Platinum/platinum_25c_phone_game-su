using UnityEngine;

public class PowerUpSpawnScript : MonoBehaviour
{
    public GameObject powerUp1;
    public GameObject powerUp2;
    public GameObject powerUp3;
    public GameObject powerUp4;
    public GameObject powerUp5;

    public float minSpawnInterval = 0.3f; // Minimum time between spawns
    public float maxSpawnInterval = 1.0f; // Maximum time between spawns
    private float timer = 0f;
    private float nextSpawnTime;

    private int powerUpWeight1 = 1;
    private int powerUpWeight2 = 1;
    private int powerUpWeight3 = 1;
    private int powerUpWeight4 = 1;
    private int powerUpWeight5 = 1;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= nextSpawnTime)
        {
            SpawnPowerUp();
            timer = 0f;
            
            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        
    }

    GameObject GetPowerUp(int i){
        if(i == 0) return powerUp1;
        if(i == 1) return powerUp2;
        if(i == 2) return powerUp3;
        if(i == 3) return powerUp4;
        if(i == 4) return powerUp5;
        return powerUp1;
    }

    int GetPowerUpWeight(int i){
        if(i == 0) return powerUpWeight1;
        if(i == 1) return powerUpWeight2;
        if(i == 2) return powerUpWeight3;
        if(i == 3) return powerUpWeight4;
        if(i == 4) return powerUpWeight5;
        return powerUpWeight1;
    }
    
    void SpawnPowerUp()
    {
        // Random position of spawn
        Vector3 spawnPos = new Vector3(
            Random.Range(-2.0f, 2.0f),  
            10f,                       
            0f
        );

        // Random power up
        GameObject selectedPowerUp = GetPowerUp(0);
        int randVal = Random.Range(1, powerUpWeight1 + powerUpWeight2 + powerUpWeight3 + powerUpWeight4 + powerUpWeight5);
        int currSum = 0;
        for(int i = 0; i < 5; i++){
            currSum += GetPowerUpWeight(i);
            if(randVal <= currSum){
                selectedPowerUp = GetPowerUp(i);
                break;
            }
        }
        
        // Spawn the debris
        GameObject newPowerUp = Instantiate(selectedPowerUp, spawnPos, Quaternion.identity);
        
        // Random speed
        DebrisMoveScript script = newPowerUp.GetComponent<DebrisMoveScript>();
        if (script != null)
        {
            script.moveSpeed = Random.Range(2.0f, 8.0f);
        }
    }
}