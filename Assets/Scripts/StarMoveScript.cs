using UnityEngine;
using System;

public class StarMoveScript : MonoBehaviour
{
    public Vector3 velocity;
    bool movementActive = true;

    CheckpointSpawnScript checkpointSpawnScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        GameObject checkpointSpawner = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointStopped += OnCheckpointStopped;
        checkpointSpawnScript.OnCheckpointResumed += OnCheckpointResumed;

    }

    void OnDestroy(){
        checkpointSpawnScript.OnCheckpointStopped -= OnCheckpointStopped;
        checkpointSpawnScript.OnCheckpointResumed -= OnCheckpointResumed;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("movementActive: " + movementActive + ", velocity: " + velocity);
        if(movementActive && GameStateManager.Instance.GetGameStageInt() != 1){
            //if the stars aren't told to stop, and the game stage isn't in transition
            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

            transform.position += velocity * Time.deltaTime;
            if (screenPos.y < -0.1f) // Slightly below screen
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCheckpointStopped(object sender, EventArgs e)
    {
        movementActive = false;
        // Debug.Log("Star stopped moving");
    }

    private void OnCheckpointResumed(object sender, EventArgs e)
    {
        movementActive = true;
        // Debug.Log("Star resumed moving");
    }
}