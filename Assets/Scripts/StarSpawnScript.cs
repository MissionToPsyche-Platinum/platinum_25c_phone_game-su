using UnityEngine;
using System;

public class StarSpawnScript : MonoBehaviour
{
    public GameObject[] starSprites;   //list of all stars that can be spawned
    public int[] starWeights; //chance of each star spawned (Ex: chance of starSprites[i] spawning is starWeights[i] / sum(starWeights))
    public float[] starSpeeds; //make closer stars move faster for parallax effect
    public float[] flashRanges;

    public float minSpawnInterval = 0.1f; // Minimum time between spawns
    public float maxSpawnInterval = 0.5f; // Maximum time between spawns
    public int initialSpawnCount;
    public int gridColumns = 6;
    private float moveSpeed;
    private float timer = 0f;
    private float nextSpawnTime;
    private int totalWeight;
    private int nextColumn = 0;

    [SerializeField] private Transform starParent;

    private float screenW;
    private float screenH;

    private bool spawnerActive;

    CheckpointSpawnScript checkpointSpawnScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenW = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        screenH = Camera.main.orthographicSize * 2f;
        if (starParent == null)
        {
            GameObject container = new GameObject("BackgroundStars");
            starParent = container.transform;
        }

        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointStopped += OnCheckpointStopped;
        checkpointSpawnScript.OnCheckpointResumed += OnCheckpointResumed;

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;

        //throw an error if powerUps and powerUpSpawnWeights are different sizes
        if (starSprites.Length != starWeights.Length || starSprites.Length != starSpeeds.Length)
        {
            throw new Exception("Number of power ups and power up spawn weights do not match.");
        }

        //calculate total of all weights provided
        totalWeight = 0;
        for (int i = 0; i < starWeights.Length; i++)
        {
            totalWeight += starWeights[i];
        }

        int rows = Mathf.CeilToInt((float)initialSpawnCount / gridColumns);
        float cellW = screenW / gridColumns;
        float cellH = screenH / rows;
        for (int i = 0; i < initialSpawnCount; i++)
        {
            int col = i % gridColumns;
            int row = i / gridColumns;
            Vector3 spawnPos = new Vector3(
                -screenW / 2f + cellW * col + UnityEngine.Random.Range(0f, cellW),
                -screenH / 2f + cellH * row + UnityEngine.Random.Range(0f, cellH),
                1f
            );
            SpawnStar(spawnPos);
        }
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying -= OnStopPlaying;
        checkpointSpawnScript.OnCheckpointStopped -= OnCheckpointStopped;
        checkpointSpawnScript.OnCheckpointResumed -= OnCheckpointResumed;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnerActive)
        {
            timer += Time.deltaTime;

            if (timer >= nextSpawnTime)
            {
                float cellW = screenW / gridColumns;
                Vector3 spawnPos = new Vector3(
                    -screenW / 2f + cellW * nextColumn + UnityEngine.Random.Range(0f, cellW),
                    screenH / 2f,
                    1f
                );
                nextColumn = (nextColumn + 1) % gridColumns;

                SpawnStar(spawnPos);
                timer = 0f;

                nextSpawnTime = UnityEngine.Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }

    }

    void SpawnStar(Vector3 spawnPos)
    {
        // Random sprite
        GameObject selectedSprite = starSprites[starSprites.Length - 1];
        int randVal = UnityEngine.Random.Range(1, totalWeight + 1);
        int currSum = 0;

        int selectedIndex = 0;

        for (int i = 0; i < starWeights.Length; i++)
        {
            currSum += starWeights[i];
            if (randVal <= currSum)
            {
                selectedSprite = starSprites[i];
                moveSpeed = starSpeeds[i];
                selectedIndex = i;
                break;
            }
        }

        // Spawn the star
        GameObject newStar = Instantiate(selectedSprite, spawnPos, Quaternion.identity, starParent);

        // Random speed
        StarMoveScript script = newStar.GetComponent<StarMoveScript>();
        if (script != null)
        {
            script.velocity = Vector3.down * moveSpeed;
            script.alphaFlashSpeed = UnityEngine.Random.Range(5, 10);
            script.minAlpha = flashRanges[selectedIndex * 2];
            script.maxAlpha = flashRanges[selectedIndex * 2 + 1];
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

    private void OnCheckpointStopped(object sender, EventArgs e)
    {
        spawnerActive = false;
    }

    private void OnCheckpointResumed(object sender, EventArgs e)
    {
        spawnerActive = true;
    }
}