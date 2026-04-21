using UnityEngine;
using System;

public class StarSpawnScript : MonoBehaviour
{
    public GameObject[] starSprites;   //list of all stars that can be spawned
    public int[] starWeights; //chance of each star spawned (Ex: chance of starSprites[i] spawning is starWeights[i] / sum(starWeights))
    public float[] starSpeeds; //make closer stars move faster for parallax effect

    public float minSpawnInterval = 0.1f; // Minimum time between spawns
    public float maxSpawnInterval = 0.5f; // Maximum time between spawns
    public int initialSpawnCount;
    private float moveSpeed;
    private float timer = 0f;
    private float nextSpawnTime;
    private int totalWeight;

    [SerializeField] private Transform starParent;

    private bool spawnerActive;

    CheckpointSpawnScript checkpointSpawnScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        for (int i = 0; i < initialSpawnCount; i++)
        {

            Vector3 spawnPos = new Vector3(
                UnityEngine.Random.Range(-2.0f, 2.0f),
                UnityEngine.Random.Range(-5.0f, 5.0f),
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

    }

    void SpawnStar(Vector3 spawnPos)
    {
        // Random sprite
        GameObject selectedSprite = starSprites[starSprites.Length - 1];
        int randVal = UnityEngine.Random.Range(1, totalWeight + 1);
        int currSum = 0;

        for (int i = 0; i < starWeights.Length; i++)
        {
            currSum += starWeights[i];
            if (randVal <= currSum)
            {
                selectedSprite = starSprites[i];
                moveSpeed = starSpeeds[i];
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