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
        if(movementActive && GameStateManager.Instance.GetGameStageInt() >= 2){
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
    }

    private void OnCheckpointResumed(object sender, EventArgs e)
    {
        movementActive = true;
    }
}