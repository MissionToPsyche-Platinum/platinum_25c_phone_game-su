using System;
using UnityEngine;

public class CheckpointSpawnScript : MonoBehaviour, IListenToStartGame
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
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void OnStartGame(){
        resumeMovement(0);
    }
}