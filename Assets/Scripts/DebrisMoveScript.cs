using UnityEngine;
using System;

public class DebrisMoveScript : MonoBehaviour
{
    public float moveSpeed = 5;
    [SerializeField] private float homingProbability = 0.2f;
    [SerializeField] private float homingAmount = 0.5f;
    bool homingAsteroid = false;
    private Vector3 velocityVector;

    bool movementPaused = false;

    CheckpointSpawnScript checkpointSpawnScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed += OnCheckpointPassed;

        //may spawn homing asteroid if at Mars or further
        if(GameStateManager.Instance.GetGameStage() >= 3 && this.gameObject.CompareTag("Debris")){
            float random = UnityEngine.Random.Range(0f, 1f);
            if(random <= homingProbability){
                homingAsteroid = true;
                SpriteRenderer objectRenderer = GetComponent<SpriteRenderer>();
                if(objectRenderer != null){
                    objectRenderer.material.color = Color.red;
                }
            }
        }
    }

    private void OnDestroy()
    {
        checkpointSpawnScript.OnCheckpointReached -= OnCheckpointReached;
        checkpointSpawnScript.OnCheckpointPassed -= OnCheckpointPassed;
    }

    // Update is called once per frame
    void Update()
    {
        if(!movementPaused){
            velocityVector = (Vector3.down * moveSpeed) * Time.deltaTime;
            if(homingAsteroid){

                Vector3 probePos = GameStateManager.Instance.GetProbePosition();

                //stops homing behavior if below probe
                if(probePos.y > transform.position.y){
                    homingAsteroid = false;
                }

                Vector3 normVelocity = velocityVector.normalized;
                Vector3 debrisToProbeVector = (probePos - transform.position).normalized;

                //rotate asteroid 
                float velocityAngle = Mathf.Atan2(normVelocity.x, normVelocity.y);
                velocityAngle = velocityAngle < 0f ? 2 * Mathf.PI + velocityAngle : velocityAngle;
                float toProbeAngle = Mathf.Atan2(debrisToProbeVector.x, debrisToProbeVector.y);
                toProbeAngle = toProbeAngle < 0f ? 2 * Mathf.PI + toProbeAngle : toProbeAngle;
                float rotationAngle = toProbeAngle - velocityAngle;
                rotationAngle *= homingAmount;

                velocityVector = rotate(velocityVector, rotationAngle);
            }

            transform.position = transform.position + velocityVector;
            Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
            if (screenPos.y < -0.1f) // Slightly below screen
            {
                Destroy(gameObject);
            }
        }
    }

    private Vector3 rotate(Vector3 v, float radians)
    {
        Vector3 rotated = new Vector3(v.x * Mathf.Cos(radians) + v.y * Mathf.Sin(radians), v.x * -Mathf.Sin(radians) + v.y * Mathf.Cos(radians), v.z);
        return rotated;
    }

    private void OnCheckpointReached(object sender, EventArgs e)
    {
        movementPaused = true;
    }

    private void OnCheckpointPassed(object sender, EventArgs e)
    {
        movementPaused = false;
    }
}
