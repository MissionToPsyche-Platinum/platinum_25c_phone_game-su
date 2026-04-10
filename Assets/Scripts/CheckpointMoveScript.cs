using UnityEngine;
using System;

public class CheckpointMoveScript : MonoBehaviour
{
    public event Action OnExitedScreen;
    bool currentlyStoppedAtCenter = false;
    bool alreadyStoppedAtCenter = false;
    float moveSpeed = 2.0f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!currentlyStoppedAtCenter){
            transform.position = transform.position + (Vector3.down * moveSpeed) * Time.deltaTime;
            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
            if (screenPos.y < -0.1f) // Slightly below screen
            {
                OnExitedScreen?.Invoke();
                Destroy(gameObject);
            }

            if(transform.position.y < 0.0f && !alreadyStoppedAtCenter){
                alreadyStoppedAtCenter = true;
                currentlyStoppedAtCenter = true;
            }
        }


    }

    public void SetAlreadyStoppedAtCenter(bool hasAlreadyStoppedAtCenter)
    {
        this.alreadyStoppedAtCenter = hasAlreadyStoppedAtCenter;
    }


    public void resumeMovement(){
        currentlyStoppedAtCenter = false;
    }
}