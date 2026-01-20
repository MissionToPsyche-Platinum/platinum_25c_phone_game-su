using UnityEngine;

public class CheckpointSpawnScript : MonoBehaviour
{

    public GameObject checkpointObj;
    public Vector3 spawnPos;
    public float scale;

    private bool gameStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject checkpoint = Instantiate(checkpointObj, spawnPos, Quaternion.identity);
        checkpoint.transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
