using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class CheckpointMoveScript : MonoBehaviour
{
    private CheckpointSpawnScript checkpointSpawnScript;
    

    public event Action OnExitedScreen;
    bool currentlyStoppedAtCenter = false;
    bool alreadyStoppedAtCenter = false;
    float moveSpeed = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject checkpointSpawner = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawner.GetComponent<CheckpointSpawnScript>();
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

            if(transform.position.y < 0.0f && !alreadyStoppedAtCenter){
                alreadyStoppedAtCenter = true;
                currentlyStoppedAtCenter = true;
                checkpointSpawnScript.pauseMovement();
            }
        } else {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                resumeMovement();
            }
        }


    }

    public void SetAlreadyStoppedAtCenter(bool hasAlreadyStoppedAtCenter)
    {
        this.alreadyStoppedAtCenter = hasAlreadyStoppedAtCenter;
    }


    public void resumeMovement(){
        currentlyStoppedAtCenter = false;
        checkpointSpawnScript.resumeMovement(GameStateManager.Instance.GetGameStageInt() - 1);
        GameStateManager.Instance.SetGameStage(GameStateManager.Instance.GetGameStageInt() + 1);
    }
}