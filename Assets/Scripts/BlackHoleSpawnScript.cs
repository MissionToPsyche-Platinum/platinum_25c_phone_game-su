using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackHoleSpawnScript : MonoBehaviour
{
    public GameObject blackHole;
    public float minSpawnInterval = 0.3f; // Minimum time between spawns
    public float maxSpawnInterval = 1.0f; // Maximum time between spawns
    private float timer = 0f;
    private float nextSpawnTime;
    private float size;

    public event EventHandler<EventArgs> OnBlackHoleSpawned;
    private bool spawnerActive;
    private bool hazardUnlocked;

    CheckpointSpawnScript checkpointSpawnScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.SetActive(false);
        hazardUnlocked = false;
        spawnerActive = true;

        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed += OnCheckpointPassed;

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        checkpointSpawnScript.OnCheckpointReached -= OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed -= OnCheckpointPassed;
    }


    // Update is called once per frame
    void Update()
    {

        //unlocks after the Moon
        if(!hazardUnlocked){
            if(GameStateManager.Instance.GetGameStage() >= 4){
                hazardUnlocked = true;
            }
        }

        if(hazardUnlocked && spawnerActive)
        {
            timer += Time.deltaTime;
            
            if (timer >= nextSpawnTime)
            {
                SpawnBlackHole();
                timer = 0f;
                
                nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
        
    }
    
    void SpawnBlackHole()
    {
        // Random position of spawn
        Vector3 spawnPos = new Vector3(
            Random.Range(-2.0f, 2.0f),  
            Random.Range(-7f, 7f),                       
            0f
        );
        
        // Spawn the debris
        GameObject newBlackHole = Instantiate(blackHole, spawnPos, Quaternion.identity);
        OnBlackHoleSpawned?.Invoke(this, EventArgs.Empty);
        
        newBlackHole.transform.localScale = Vector3.one * 0.001f;
        
        BlackHoleController script = newBlackHole.GetComponent<BlackHoleController>();
        if (script != null)
        {
            script.size = Random.Range(0.2f, 0.5f);
            script.initialSize = 0.001f;
        }
    }

    private void OnStartPlaying(object sender, EventArgs e)
    {
        this.gameObject.SetActive(true);
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