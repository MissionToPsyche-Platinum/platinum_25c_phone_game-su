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
        {0.5f, 0f, 0f, 0f, 0.5f},           //spawn probabilities at Mars checkpoint
        {0.25f, 0.25f, 0.25f, 0f, 0.25f},       //spawn probabilities at Psyche checkpoint
        {0.2f, 0.2f, 0.2f, 0.2f, 0.2f}      //spawn probabilities at Jupiter checkpoint and beyond
    };

    public float minSize = 0.3f;
    public float maxSize = 1f;

    private float gameTime = 0f;
    private float difficultyMultiplier = 1f;

    private float spawnTimer = 0f;
    private float spawnInterval = 0.15f;

    private List<GameObject> activeAsteroids = new List<GameObject>();

    int numLanes = 7;
    private List<float> laneXPos = new List<float>();
    private List<int> availableLanes = new List<int>();
    int currLane = 0;

    float minSpeed = 3f;
    float maxSpeed = 5f;
    float maxGap = 5f;
    float minGap = 3f;
    float minHomingAmount = 0.005f;
    float maxHomingAmount = 0.02f;
    float difficultyCutoff = 5000;                  //score when difficulty stops increasing

    private float tileSpawnProbability = 0.5f;      //chance of premade tile spawning instead of random spawn
    private float gapMultiplier = 1f;    //component modifier: >1 means longer interval (fewer spawns)
    ObstacleTileController tileController;

    public event EventHandler<EventArgs> OnDebrisSpawned;
    private bool spawnerActive = true;
    CheckpointSpawnScript checkpointSpawnScript;

    private ScoreIncrement scoreSystem;

    private float screenW;
    private float _baseScreenW;

    

    void Start()
    {
        _baseScreenW = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        screenW = _baseScreenW;
        this.gameObject.SetActive(false);
        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed += OnCheckpointPassed;

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;

        scoreSystem = scoreIncrement.GetComponent<ScoreIncrement>();

        tileController = this.gameObject.GetComponent<ObstacleTileController>();

        float minXPos = -screenW / 2f;
        float maxXPos = screenW / 2f;
        float laneWidth = (maxXPos - minXPos) / (float)(numLanes - 1);
        for(int i = 0; i < numLanes; i++){
            laneXPos.Add(minXPos + (float)i * laneWidth);
        }

        updateSpawnProbabilities();
        
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
        if (spawnerActive && GameStateManager.Instance.currentGameState == GameStateManager.GameState.Playing)
        {
            float score = scoreSystem.GetCurrentScore();
            difficultyMultiplier = score < difficultyCutoff ? score / difficultyCutoff : 1f;

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                float spawnTypeSeed = Random.Range(0f, 1f);
                if(spawnTypeSeed <= tileSpawnProbability || tileController.UsingTestMode())
                {
                    SpawnTile();
                }
                else
                {
                    SpawnDebris();
                }
            }
        }
    }

    void updateSpawnProbabilities(){
        int gameStage = GameStateManager.Instance.GetGameStageInt();
        float score = (float)scoreSystem.GetCurrentScore();

        if(gameStage < 2){                          //not at a checkpoint, invalid
            Array.Fill(spawnWeights, 0f);
        } else if(gameStage >= 6){
            for(int i = 0; i < 5; i++){
                spawnWeights[i] = spawnWeightsAtCheckpoints[4, i];
            }
        } else {
            float distanceBetweenCheckpoints = checkpointSpawnScript.GetCheckpointScore(gameStage - 1) - checkpointSpawnScript.GetCheckpointScore(gameStage - 2);
            float initialCheckpointScore = checkpointSpawnScript.GetCheckpointScore(gameStage - 2);
            float progress = (score - initialCheckpointScore) / distanceBetweenCheckpoints;

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

        //checks if there is room to spawn
        for(int i = 0; i < spawnInfos.Count; i++){
            updateActiveAsteroids();
            for(int j = 0; j < activeAsteroids.Count; j++){
                float newX = spawnInfos[i].GetPosX();
                float newY = spawnInfos[i].GetPosY();
                float currX = activeAsteroids[j].transform.position.x;
                float currY = activeAsteroids[j].transform.position.y;
                float gap = Mathf.Lerp(maxGap, minGap, difficultyMultiplier) * gapMultiplier;
                int type = spawnInfos[i].GetType();
                if((currX - newX) * (currX - newX) + (currY - newY) * (currY - newY) < gap * gap){
                    return;
                }

                int gameStage = GameStateManager.Instance.GetGameStageInt();
                if(type == 4 && gameStage < 3) return;
                if((type == 1 || type == 2) && gameStage < 4) return;
                if(type == 3 && gameStage < 5) return;
            }
        }

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
            if(debrisType == 1){

            }
            OnDebrisSpawned?.Invoke(this, EventArgs.Empty);
            activeAsteroids.Add(newDebris);

            // Size
            float scale = spawnInfo.GetScale() * 0.1f;
            newDebris.transform.localScale = Vector3.one * scale;

            // Speed with difficulty multiplier
            DebrisMoveScript script = newDebris.GetComponent<DebrisMoveScript>();
            if (script != null)
            {
                if(debrisType == 1){
                    script.homingAmount = Mathf.Lerp(minHomingAmount, maxHomingAmount, difficultyMultiplier);
                }
                script.velocity = Mathf.Lerp(minSpeed, maxSpeed, difficultyMultiplier) * Vector3.down; // 1% faster per second
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
        if(currLane >= numLanes){
            shuffleLanes();
            currLane = 0;
        }
        // Random position of spawn
        
        Vector3 spawnPos = new Vector3(
            laneXPos[availableLanes[currLane]],
            10f,
            0f
        );

        updateActiveAsteroids();
        for(int j = 0; j < activeAsteroids.Count; j++){
            float newX = spawnPos.x;
            float newY = spawnPos.y;
            float currX = activeAsteroids[j].transform.position.x;
            float currY = activeAsteroids[j].transform.position.y;
            float gap = Mathf.Lerp(maxGap, minGap, difficultyMultiplier) * gapMultiplier;
            if((currX - newX) * (currX - newX) + (currY - newY) * (currY - newY) < gap * gap){
                return;
            }
        }

        Debug.Log(difficultyMultiplier);
        currLane++;

        updateSpawnProbabilities();

        int debrisType = 0;
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
        
        int gameStage = GameStateManager.Instance.GetGameStageInt();
        if(debrisType == 4 && gameStage < 3) return;
        if((debrisType == 1 || debrisType == 2) && gameStage < 4) return;
        if(debrisType == 3 && gameStage < 5) return;
        
        // Spawn the debris
        GameObject newDebris = Instantiate(selectedDebris, spawnPos, Quaternion.identity);
        OnDebrisSpawned?.Invoke(this, EventArgs.Empty);
        activeAsteroids.Add(newDebris);

        // Random size
        float randomScale = Random.Range(minSize, maxSize) * 0.15f;
        newDebris.transform.localScale = Vector3.one * randomScale;

        // Random speed with difficulty multiplier
        DebrisMoveScript script = newDebris.GetComponent<DebrisMoveScript>();
        if (script != null)
        {
            if(debrisType == 1){
                script.homingAmount = Mathf.Lerp(minHomingAmount, maxHomingAmount, difficultyMultiplier);
            }
            script.velocity = Mathf.Lerp(minSpeed, maxSpeed, difficultyMultiplier) * Vector3.down; // 1% faster per second
            script.rotationSpeed = Random.Range(0.05f, 0.4f);
            script.type = debrisType;
            script.debrisScale = randomScale;
        }
    }

    void shuffleLanes(){
        List<int> order = new List<int>();
        bool duplicate = false;
        for(int i = 0; i < numLanes; i++){
            duplicate = true;
            int newIndex = -1;
            while(duplicate){
                duplicate = false;
                newIndex = Random.Range(0, numLanes);
                for(int j = 0; j < i; j++){
                    if(order[j] == newIndex){
                        duplicate = true;
                    }
                }
            }
            order.Add(newIndex);
        }

        availableLanes = order;
    }

    void updateActiveAsteroids(){
        activeAsteroids.RemoveAll(d => d == null);
    }

    public void SetSpawnIntervalMultiplier(float m)
    {
        gapMultiplier = m;
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

        float visionMult = ComponentManager.Instance != null ? ComponentManager.Instance.GetVisionMultiplier() : 1f;
        screenW = _baseScreenW * visionMult;
        float minXPos = -screenW / 2f;
        float maxXPos = screenW / 2f;
        float laneWidth = (maxXPos - minXPos) / (float)(numLanes - 1);
        laneXPos.Clear();
        for (int i = 0; i < numLanes; i++)
            laneXPos.Add(minXPos + (float)i * laneWidth);

        gameTime = 0f;
        difficultyMultiplier = 0f;
        currLane = 0;
        shuffleLanes();
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