using UnityEngine;

public class CheckpointMoveScript : MonoBehaviour
{
    bool stoppedAtCenter = false;
    bool alreadyStopped = false;
    float moveSpeed = 2.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("stoppedAtCenter: " + stoppedAtCenter);
        Debug.Log("alreadyStopped: " + alreadyStopped);
        Debug.Log("Position: " + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(!stoppedAtCenter){
            transform.position = transform.position + (Vector3.down * moveSpeed) * Time.deltaTime;
            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
            if (screenPos.y < -0.1f) // Slightly below screen
            {
                Destroy(gameObject);
            }

            if(transform.position.y < 0.0f && !alreadyStopped){
                GameStateManager.Instance.SetGameStage(1);
                alreadyStopped = true;
                stoppedAtCenter = true;
            }
        }


    }


    public void resumeMovement(){
        stoppedAtCenter = false;
    }
}