using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using NUnit.Framework.Constraints;

public class CheckpointMoveScript : MonoBehaviour
{
    private CheckpointSpawnScript checkpointSpawnScript;
    

    public event Action OnExitedScreen;
    bool currentlyStoppedAtCenter = false;
    bool alreadyStoppedAtCenter = false;
    private bool hasContinued = false; 
    float moveSpeed = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject checkpointSpawner = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();

        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        checkpointSpawnScript.OnCheckpointResumed += CheckpointSpawnScript_OnCheckpointResumed;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        checkpointSpawnScript.OnCheckpointResumed -= CheckpointSpawnScript_OnCheckpointResumed;
    }

    // Update is called once per frame
    void Update()
    {
        if(!currentlyStoppedAtCenter){
            transform.position = transform.position + (Vector3.down * moveSpeed) * Time.deltaTime;
            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
            if (screenPos.y < -0.1f) // Slightly below screen
            {
                //OnExitedScreen?.Invoke();
                checkpointSpawnScript.HandleCheckpointExited();
                Destroy(gameObject);
            }

            if(transform.position.y < 0.0f && !alreadyStoppedAtCenter && !hasContinued){
                alreadyStoppedAtCenter = true;
                currentlyStoppedAtCenter = true;
                checkpointSpawnScript.pauseMovement();
            }
        } else {
            // if (GameStateManager.Instance.GetGameStageInt() >= 2 && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            // {
            //     resumeMovement();
            // }
        }

    }

    public void SetAlreadyStoppedAtCenter(bool hasAlreadyStoppedAtCenter)
    {
        this.alreadyStoppedAtCenter = hasAlreadyStoppedAtCenter;
    }


    public void resumeMovement(){
        // Debug.Log("CheckpointMoveScript::resumeMovement()");
        currentlyStoppedAtCenter = false;
        hasContinued = true;
        GameStateManager.Instance.SetGameStage(GameStateManager.Instance.GetGameStageInt() + 1);
    }

    private void OnStartPlaying(object Sender, EventArgs e){
        resumeMovement();
    }

    private void CheckpointSpawnScript_OnCheckpointResumed(object sender, EventArgs e)
    {
        resumeMovement();
    }
}