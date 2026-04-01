using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

struct DebrisSpawnInfo {
    int type;
    float scale;
    float initialX;
    float initialY;
    float moveSpeed;

    public DebrisSpawnInfo(int debrisType, float size, float xPos, float yPos, float speed)
    {
        type = debrisType;
        scale = size;
        initialX = xPos;
        initialY = yPos;
        moveSpeed = speed;
    }

    public int GetType()
    {
        return type;
    }

    public float GetScale()
    {
        return scale;
    }

    public float GetPosX()
    {
        return initialX;
    }

    public float GetPosY()
    {
        return initialY;
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }
};

public class DebrisSpawnScript : MonoBehaviour
{
    //0 - standard, 1 - homing, 2 - exploding, 3 - duplicating, 4 - teleporting
    public GameObject[] debrisTypes;
    public int[] spawnWeights;
    private int totalWeight = 0;

    public float minSpawnInterval = 0.2f; 
    public float maxSpawnInterval = 0.5f;
    public float minSize = 0.3f;
    public float maxSize = 1f;

    private float timer = 0f;
    private float nextSpawnTime;
    private float gameTime = 0f; 
    private float difficultyMultiplier = 1f;

    private float tileSpawnProbability = 0.5f;
    private List<List<DebrisSpawnInfo>>[] obstacleTiles;

    /*
        Arrays below are used to initialize the obstacleTiles list

        Each line of each array below must have the same number of elements
        Value of -1 is used to delineate different lines in types array, 0 is used for consistency in other arrays

        Each entry (apart from line delineators) correspond to one obstacle attribute value, a single GameObject
        Each line corresponds to attribute values for one obstacle tile, an assortment of debris GameObjects
        Matching indices correspond to different attributes of the same GameObject
    */
    private int[] types = 
    {
        1, 1, -1,           //one homing above another
        0, 0, 0, -1,        //two medium standards on the sides, one smaller standard in the middle
        0, -1,              //standard on the left
        0, -1,              //standard on the right
        0                   //standard in middle
    };

    private float[] scales = 
    {
        0.3f, 0.3f, 0f,
        0.3f, 0.3f, 0.1f, 0f,
        0.5f, 0f,
        0.5f, 0f,
        0.5f
    };

    private float[] xPositions = 
    {
        0f, 0f, 0f,
        -1.5f, 1.5f, 0f, 0f,
        -1f, 0f, 
        0f, 0f, 
        1f
    };

    private float[] yPositions = 
    {
        10f, 12f, 0f,
        10f, 10f, 15f, 0f,
        10f, 0f,
        10f, 0f,
        10f
    };

    private float[] speeds = 
    {
        5f, 5f, 0f,
        3f, 3f, 6f, 0f,
        3f, 0f, 
        3f, 0f, 
        3f
    };

    private int[] sets = {1, 1, 0, 0, 0};

    private float[] setSpawnProbabilities = {0.75f, 0.175f, 0.075f};

    public event EventHandler<EventArgs> OnDebrisSpawned;
    private bool spawnerActive = true;
    CheckpointSpawnScript checkpointSpawnScript;

    void Start()
    {
        this.gameObject.SetActive(false);
        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed += OnCheckpointPassed;

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;

        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);

        for(int i = 0; i < 5; i++)
        {
            totalWeight += spawnWeights[i];
        }

        obstacleTiles = new List<List<DebrisSpawnInfo>>[3];
        for(int i = 0; i < 3; i++){
            obstacleTiles[i] = new List<List<DebrisSpawnInfo>>();
        }

        if(types.Length != scales.Length || types.Length != xPositions.Length || types.Length != yPositions.Length || types.Length != speeds.Length)
        {
            Debug.Log(types.Length);
            Debug.Log(scales.Length);
            Debug.Log(xPositions.Length);
            Debug.Log(yPositions.Length);
            Debug.Log(speeds.Length);
            throw new Exception("Obstacle tile attribute array lengths do not match");
        } else {
            int currIndex = 0;
            int setIndex = 0;
            while(currIndex < types.Length)
            {
                List<DebrisSpawnInfo> newList = new List<DebrisSpawnInfo>();
                while(currIndex < types.Length && types[currIndex] != -1)
                {
                    DebrisSpawnInfo newSpawnInfo = new DebrisSpawnInfo(types[currIndex], scales[currIndex], xPositions[currIndex], yPositions[currIndex], speeds[currIndex]);
                    newList.Add(newSpawnInfo);
                    currIndex++;
                }
                currIndex++;
                if(setIndex < sets.Length){
                    obstacleTiles[sets[setIndex]].Add(newList);
                } else {
                    throw new Exception("Number of assigned sets does not match number of sets given");
                }
                setIndex++;
            }
        }

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

            
            float adjustedSpawnTime = nextSpawnTime / difficultyMultiplier;

            if (timer >= adjustedSpawnTime)
            {
                float spawnType = Random.Range(0f, 1f);
                if(spawnType <= tileSpawnProbability)
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

    void SpawnTile(){

        float seed = Random.Range(0f, 1f);
        int chosenTileSet = -1;
        float currTotal = setSpawnProbabilities[0];
        for(int i = 0; i < setSpawnProbabilities.Length - 1; i++)
        {
            if(seed < currTotal)
            {
                chosenTileSet = i;
                break;
            }
            currTotal += setSpawnProbabilities[i + 1];
        }
        if(chosenTileSet == -1){
            chosenTileSet = setSpawnProbabilities.Length - 1;
        }

        int chosenTile = Random.Range(0, obstacleTiles[chosenTileSet].Count);
        List<DebrisSpawnInfo> spawnInfos = obstacleTiles[chosenTileSet][chosenTile];
        for(int i = 0; i < spawnInfos.Count; i++){
            DebrisSpawnInfo spawnInfo = spawnInfos[i];

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
                script.type = debrisType;
                script.debrisScale = scale;
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

        int debrisType = debrisTypes.Length - 1;
        GameObject selectedDebris = debrisTypes[debrisType];
        int randVal = UnityEngine.Random.Range(1, totalWeight + 1); 
        int currSum = 0;

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
            script.type = debrisType;
            script.debrisScale = randomScale;
        }
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