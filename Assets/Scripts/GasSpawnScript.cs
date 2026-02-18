using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GasSpawnScript : MonoBehaviour
{
    public GameObject gas;
    public float minSpawnInterval = 0.3f; // Minimum time between spawns
    public float maxSpawnInterval = 1.0f; // Maximum time between spawns
    private float timer = 0f;
    private float nextSpawnTime;

    public event EventHandler<EventArgs> OnGasSpawned;
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

        //unlocks after the Moon
        if(!hazardUnlocked){
            if(GameStateManager.Instance.GetGameStageInt() >= 3){
                hazardUnlocked = true;
            }
        }

        if(hazardUnlocked && spawnerActive)
        {
            timer += Time.deltaTime;
            
            if (timer >= nextSpawnTime)
            {
                SpawnGas();
                timer = 0f;
                
                nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
        
    }
    
    void SpawnGas()
    {
        // Random position of spawn
        Vector3 spawnPos = new Vector3(
            Random.Range(-2.0f, 2.0f),  
            10f,                       
            0f
        );
        
        // Spawn the debris
        GameObject newGas = Instantiate(gas, spawnPos, Quaternion.identity);
        OnGasSpawned?.Invoke(this, EventArgs.Empty);
        
        // Random size
        float randomScale = Random.Range(0.2f, 0.5f);
        newGas.transform.localScale = Vector3.one * randomScale;
        
        // Random speed
        DebrisMoveScript script = newGas.GetComponent<DebrisMoveScript>();
        if (script != null)
        {
            script.moveSpeed = Random.Range(2.0f, 8.0f);
        }
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