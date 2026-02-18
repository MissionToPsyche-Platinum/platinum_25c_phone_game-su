using System;
using UnityEngine;

public class CheckpointSpawnScript : MonoBehaviour
{

    [SerializeField] private GameObject scoreIncrement;
    [SerializeField] private GameObject debrisSpawner;
    [SerializeField] private GameObject coinSpawner;
    [SerializeField] private GameObject powerUpSpawner;

    public GameObject[] checkpoints;
    public int[] checkpointScores;
    private GameObject[] checkpointInstances;

    private const int numCheckpoints = 4;
    private float checkpointZ = 0.9f;

    private int nextCheckpoint = 0;
    float stopTimer = 7f;

    private ScoreIncrement scoreSystem;
    private DebrisSpawnScript debrisSpawnSystem;
    private CoinSpawner coinSpawnSystem;
    private PowerUpSpawnScript powerUpSpawnSystem;

    private bool stopped = false;

    public event EventHandler<EventArgs> OnCheckpointReached;
    public event EventHandler<EventArgs> OnCheckpointPassed;

    private void Awake()
    {
        checkpointInstances = new GameObject[numCheckpoints];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreSystem = scoreIncrement.GetComponent<ScoreIncrement>();
        debrisSpawnSystem = debrisSpawner.GetComponent<DebrisSpawnScript>();
        coinSpawnSystem = coinSpawner.GetComponent<CoinSpawner>();
        powerUpSpawnSystem = powerUpSpawner.GetComponent<PowerUpSpawnScript>();

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }

    public void spawnCheckpoint(int index){
        Vector3 spawnPos = new Vector3(0.0f, 0.00f, checkpointZ);
        if(index != 0){
            spawnPos.Set(0.0f, 8.0f, checkpointZ);
        }
        checkpointInstances[index] = Instantiate(checkpoints[index], spawnPos, Quaternion.identity);
        checkpointInstances[index].transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        nextCheckpoint += 1;
        Debug.Log("Spawning checkpoint " + index);
    }

    public void resumeMovement(int index){
        CheckpointMoveScript moveScript = checkpointInstances[index].GetComponent<CheckpointMoveScript>();
        moveScript.resumeMovement();
        OnCheckpointPassed?.Invoke(this, EventArgs.Empty);
        GameStateManager.Instance.SetGameStage(index + 2);
    }
    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        KillAllActiveCheckpoints();
        
        //Default stuff for spawn logic
        nextCheckpoint = (int)GameStateManager.Instance.startingGameStage - 1;
        stopped = false;
        stopTimer = 7f;
        
        //Spawn the new checkpoint and make it move
        spawnCheckpoint((int)GameStateManager.Instance.startingGameStage - 1);
        CheckpointMoveScript moveScript = checkpointInstances[0].GetComponent<CheckpointMoveScript>();
        moveScript.SetAlreadyStoppedAtCenter(true);
        resumeMovement((int)GameStateManager.Instance.startingGameStage - 1);
    }
    
    void Update()
    {
        if (GameStateManager.Instance.currentGameState != GameStateManager.GameState.Playing)
        {
            //If the game isn't playing, then ignore this stuff
            return;
        }
        
        float currentScore = scoreSystem.GetCurrentScore();
        if(nextCheckpoint < numCheckpoints && currentScore >= checkpointScores[nextCheckpoint]){
            OnCheckpointReached?.Invoke(this, EventArgs.Empty);
            spawnCheckpoint(nextCheckpoint);
            stopped = true;
        }

        if(stopped){
            stopTimer -= Time.deltaTime;
        }

        if(stopTimer <= 0.0f){
            GameStateManager.Instance.SetGameStage(nextCheckpoint);
            resumeMovement(nextCheckpoint - 1);
            stopTimer = 7f;
            stopped = false;
        }
    }

    private void KillAllActiveCheckpoints()
    {
        foreach (GameObject checkpoint in checkpointInstances)
        {
            if (checkpoint != null)
            {
                Destroy(checkpoint);
            }
        }
    }
}