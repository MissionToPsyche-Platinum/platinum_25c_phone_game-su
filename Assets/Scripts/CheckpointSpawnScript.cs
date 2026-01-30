using System;
using UnityEngine;

public class CheckpointSpawnScript : MonoBehaviour
{

    public GameObject[] checkpoints;
    private GameObject[] checkpointInstances;

    private const int numCheckpoints = 1;
    private float checkpointZ = 0.9f;

    private void Awake()
    {
        checkpointInstances = new GameObject[numCheckpoints];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnCheckpoint(0);
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
    }

    public void spawnCheckpoint(int index){
        Vector3 spawnPos = new Vector3(0.0f, 0.00f, checkpointZ);
        checkpointInstances[index] = Instantiate(checkpoints[index], spawnPos, Quaternion.identity);
        checkpointInstances[index].transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
    }

    public void resumeMovement(int index){
        CheckpointMoveScript moveScript = checkpointInstances[index].GetComponent<CheckpointMoveScript>();
        moveScript.resumeMovement();
    }
    private void OnStartPlaying(object sender, EventArgs e)
    {
        resumeMovement(0);
    }
}