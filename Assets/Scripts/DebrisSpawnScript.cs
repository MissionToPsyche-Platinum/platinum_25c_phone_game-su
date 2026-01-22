using UnityEngine;

public class DebrisSpawnScript : MonoBehaviour, IListenToStartGame
{
    public GameObject debris;
    public float minSpawnInterval = 0.3f; // Minimum time between spawns
    public float maxSpawnInterval = 1.0f; // Maximum time between spawns
    private float timer = 0f;
    private float nextSpawnTime;
    
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
            SpawnDebris();
            timer = 0f;
            
            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        
    }
    
    void SpawnDebris()
    {
        // Random position of spawn
        Vector3 spawnPos = new Vector3(
            Random.Range(-2.0f, 2.0f),  
            10f,                       
            0f
        );
        
        // Spawn the debris
        GameObject newDebris = Instantiate(debris, spawnPos, Quaternion.identity);
        
        // Random size
        float randomScale = Random.Range(0.3f, 2.0f);
        newDebris.transform.localScale = Vector3.one * randomScale;
        
        // Random speed
        DebrisMoveScript script = newDebris.GetComponent<DebrisMoveScript>();
        if (script != null)
        {
            script.moveSpeed = Random.Range(2.0f, 8.0f);
        }
    }

    public void OnStartGame()
    {
        this.gameObject.SetActive(true);
    }
}