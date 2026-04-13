using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LandmarkEntry
{
    public GameObject prefab;
    public float triggerScore;
    [Tooltip("How far above the camera to spawn the landmark")]
    public float spawnYOffset;
    public float spawnX;

}

public class LandmarkSpawner : MonoBehaviour
{
    [Header("Landmarks")]
    public LandmarkEntry[] landmarks;

    private HashSet<int> alreadySpawnedLandmarks = new HashSet<int>();
    private Camera myCamera;
    
    [SerializeField] private ScoreIncrement myScoreIncrement;

    void Start()
    {
        myCamera = Camera.main;

        if (myScoreIncrement == null)
            Debug.LogError("LandmarkSpawner: Could not find a ScoreIncrement component in the scene.");

        Array.Sort(landmarks, (a, b) => a.triggerScore.CompareTo(b.triggerScore));

        GameStateManager.Instance.OnStartPlaying += GameStateManager_OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= GameStateManager_OnStartPlaying;
    }


    void Update()
    {
        if (myScoreIncrement == null) return;
        if (GameStateManager.Instance == null) return;
        if (GameStateManager.Instance.currentGameState != GameStateManager.GameState.Playing) return;

        float currentScore = myScoreIncrement.GetCurrentScore();

        for (int i = 0; i < landmarks.Length; i++)
        {
            if (!alreadySpawnedLandmarks.Contains(i) && currentScore >= landmarks[i].triggerScore)
            {
                SpawnLandmark(i);
            }
        }
    }

    void SpawnLandmark(int index)
    {
        LandmarkEntry entry = landmarks[index];

        if (entry.prefab == null)
        {
            Debug.LogWarning($"LandmarkSpawner: entry at index {index} has no prefab assigned.");
            alreadySpawnedLandmarks.Add(index);
            return;
        }

        float camTopY = myCamera.transform.position.y + myCamera.orthographicSize + entry.spawnYOffset;
        Vector3 spawnPos = new Vector3(entry.spawnX, camTopY, 0f);

        GameObject spawned = Instantiate(entry.prefab, spawnPos, Quaternion.identity);
        spawned.name = $"{entry.prefab.name}_score{entry.triggerScore}";

        alreadySpawnedLandmarks.Add(index);
        Debug.Log($"LandmarkSpawner: Spawned '{entry.prefab.name}' at score {entry.triggerScore}");
    }
    
    private void GameStateManager_OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        ResetSpawner();
    }

    public void ResetSpawner()
    {
        Debug.Log("ResetSpawner() called");
        alreadySpawnedLandmarks.Clear();
    }
}