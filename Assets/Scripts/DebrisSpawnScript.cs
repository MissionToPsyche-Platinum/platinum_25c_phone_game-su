using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DebrisSpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject scoreIncrement;

    //0 - standard, 1 - homing, 2 - exploding, 3 - duplicating, 4 - teleporting
    public GameObject[] debrisTypes;
    public float[] spawnWeights;

    private float[,] spawnWeightsAtCheckpoints =
    {
        {1f, 0f, 0f, 0f, 0f},                //spawn probabilities at Earth checkpoint
        {1f, 0f, 0f, 0f, 0f},                //spawn probabilities at Moon checkpoint
        {0.7f, 0.2f, 0.1f, 0f, 0f},          //spawn probabilities at Mars checkpoint
        {0.35f, 0.25f, 0.15f, 0.15f, 0.1f},  //spawn probabilities at Psyche checkpoint
    };

    public float minSpawnInterval = 0.2f; 
    public float maxSpawnInterval = 0.5f;
    public float minSize = 0.3f;
    public float maxSize = 1f;

    private float timer = 0f;
    private float nextSpawnTime;
    private float gameTime = 0f; 
    private float difficultyMultiplier = 1f;

    private float tileSpawnProbability = 0.5f;      //chance of premade tile spawning instead of random spawn
    private float _spawnIntervalMultiplier = 1f;    //component modifier: >1 means longer interval (fewer spawns)
    ObstacleTileController tileController;

    public event EventHandler<EventArgs> OnDebrisSpawned;
    private bool spawnerActive = true;
    CheckpointSpawnScript checkpointSpawnScript;

    private ScoreIncrement scoreSystem;

    void Start()
    {
        this.gameObject.SetActive(false);
        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed += OnCheckpointPassed;

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;

        scoreSystem = scoreIncrement.GetComponent<ScoreIncrement>();

        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);

        tileController = this.gameObject.GetComponent<ObstacleTileController>();
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
        if (spawnerActive)
        {
            
            gameTime += Time.deltaTime;
            difficultyMultiplier = 1f + (gameTime * 0.01f);

            timer += Time.deltaTime;

            
            float adjustedSpawnTime = nextSpawnTime * _spawnIntervalMultiplier / difficultyMultiplier;

            if (timer >= adjustedSpawnTime)
            {
                float spawnTypeSeed = Random.Range(0f, 1f);
                if(spawnTypeSeed <= tileSpawnProbability || tileController.UsingTestMode())
                {
                    SpawnTile();
                } 
                else 
                {
                    SpawnDebris();
                }
                timer = 0f;
                nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    void updateSpawnProbabilities(){
        int gameStage = GameStateManager.Instance.GetGameStageInt();
        float score = (float)scoreSystem.GetCurrentScore();

        if(gameStage < 2){                          //not at a checkpoint, invalid
            Array.Fill(spawnWeights, 0f);
        } else if(gameStage >= 5){                  //spawn weights have stopped changing, constant at last checkpoint's probabilities
            for(int i = 0; i < 5; i++){
                spawnWeights[i] = spawnWeightsAtCheckpoints[3, i];
            }
        } else {
            float distanceBetweenCheckpoints = checkpointSpawnScript.GetCheckpointScore(gameStage - 1) - checkpointSpawnScript.GetCheckpointScore(gameStage - 2);
            float initialCheckpointScore = checkpointSpawnScript.GetCheckpointScore(gameStage - 2);
            float progress = (score - initialCheckpointScore) / distanceBetweenCheckpoints;     //percentage of the way to the next checkpoint

            //linear interpolation between probabilities at previous and next checkpoint
            for(int i = 0; i < 5; i++){
                spawnWeights[i] = (spawnWeightsAtCheckpoints[gameStage - 1, i] - spawnWeightsAtCheckpoints[gameStage - 2, i]) * progress + spawnWeightsAtCheckpoints[gameStage - 2, i];
            }
        }

        //ensure that weights add to 1
        if(gameStage >= 2){
            float sum = 0f;
            for(int i = 0; i < 5; i++){
                sum += spawnWeights[i];
            }

            spawnWeights[0] -= (sum - 1f);
        }
    }

    void SpawnTile(){
        List<ObstacleTileController.DebrisSpawnInfo> spawnInfos = tileController.GetRandomTile();
        for(int i = 0; i < spawnInfos.Count; i++){
            ObstacleTileController.DebrisSpawnInfo spawnInfo = spawnInfos[i];

            // Spawn Position
            Vector3 spawnPos = new Vector3(
                spawnInfo.GetPosX(),
                spawnInfo.GetPosY(),
                0f
            );

            int debrisType = spawnInfo.GetType();
            GameObject selectedDebris = debrisTypes[debrisType];
    
            // Spawn the debris
            GameObject newDebris = Instantiate(selectedDebris, spawnPos, Quaternion.identity);
            OnDebrisSpawned?.Invoke(this, EventArgs.Empty);

            // Size
            float scale = spawnInfo.GetScale();
            newDebris.transform.localScale = Vector3.one * scale;

            // Speed with difficulty multiplier
            DebrisMoveScript script = newDebris.GetComponent<DebrisMoveScript>();
            if (script != null)
            {
                float baseSpeed = spawnInfo.GetSpeed();
                script.velocity = baseSpeed * difficultyMultiplier * Vector3.down; // 1% faster per second
                script.rotationSpeed = Random.Range(0.05f, 0.4f);
                script.type = debrisType;
                script.debrisScale = scale;
                script.pivotPoint = new Vector3(spawnInfo.GetPivotX(), spawnInfo.GetPivotY(), 0f);
                script.revSpeed = spawnInfo.GetRevSpeed();
            }
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

        updateSpawnProbabilities();
        // for(int i = 0; i < 5; i++){
        //     Debug.Log(spawnWeights[i]);
        // }

        int debrisType = debrisTypes.Length - 1;
        GameObject selectedDebris = debrisTypes[debrisType];
        float randVal = UnityEngine.Random.Range(0f, 1f); 
        float currSum = 0;

        for(int i = 0; i < 5; i++){
            currSum += spawnWeights[i];
            if(currSum >= randVal){             //checks if random value generated is between sum of first i-1 and first i weights
                selectedDebris = debrisTypes[i];
                debrisType = i;
                break;
            }
        }
        
        // Spawn the debris
        GameObject newDebris = Instantiate(selectedDebris, spawnPos, Quaternion.identity);
        OnDebrisSpawned?.Invoke(this, EventArgs.Empty);

        // Random size
        float randomScale = Random.Range(minSize, maxSize);
        newDebris.transform.localScale = Vector3.one * randomScale;

        // Random speed with difficulty multiplier
        DebrisMoveScript script = newDebris.GetComponent<DebrisMoveScript>();
        if (script != null)
        {
            float baseSpeed = Random.Range(2.0f, 8.0f);
            script.velocity = baseSpeed * difficultyMultiplier * Vector3.down; // 1% faster per second
            script.rotationSpeed = Random.Range(0.05f, 0.4f);
            script.type = debrisType;
            script.debrisScale = randomScale;
        }
    }

    public void SetSpawnIntervalMultiplier(float m)
    {
        _spawnIntervalMultiplier = m;
    }

    public void DisableSpawning()
    {
        spawnerActive = false;
    }

    public void EnableSpawning()
    {
        spawnerActive = true;
    }

    private void OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        this.gameObject.SetActive(true);
        
        gameTime = 0f;
        difficultyMultiplier = 1f;
        timer = 0f;
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void OnStopPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
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