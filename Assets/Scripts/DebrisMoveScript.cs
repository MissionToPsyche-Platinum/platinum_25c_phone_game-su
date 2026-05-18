using UnityEngine;
using System;
using System.Collections;

using Random = UnityEngine.Random;

public class DebrisMoveScript : MonoBehaviour
{
    public Vector3 velocity = new Vector3(0f, -5f, 0f);
    public float rotationSpeed = 1f;
    public int type = 0;
    public float debrisScale;

    public Vector3 pivotPoint;
    public float revSpeed;

    private float currRotation = 0f;

    private bool hasAppeared = false;

    //area where certain special behaviors will occur (explosion, duplication, teleports)
    public float activationY = 0.5f;
    private bool activated = false;

    [SerializeField] private GameObject[] explodingFragments;
    [SerializeField] private int numFragments;

    [SerializeField] private float homingAmount = 0.5f;
    [SerializeField] private float fragmentSpreadSpeed = 2f;

    private Vector3 velocityScaled;

    private GameObject teleportGhost;

    CheckpointSpawnScript checkpointSpawnScript;
    DebrisSpawnScript debrisSpawnScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ComponentManager.Instance != null)
            velocity *= ComponentManager.Instance.GetDebrisSpeedMultiplier();
        velocityScaled = velocity * Time.deltaTime;
        rotationSpeed = Random.Range(0.05f, 0.4f);

        GameObject checkpointSpawn = GameObject.Find("CheckpointSpawner");
        checkpointSpawnScript = checkpointSpawn.GetComponent<CheckpointSpawnScript>();
        checkpointSpawnScript.OnCheckpointReached += OnCheckpointReached;

        GameObject debrisSpawn = GameObject.Find("DebrisSpawner");
        debrisSpawnScript = debrisSpawn.GetComponent<DebrisSpawnScript>();
    }

    void OnDestroy(){
        checkpointSpawnScript.OnCheckpointReached -= OnCheckpointReached;
        if (teleportGhost != null) Destroy(teleportGhost);
    }

    // Update is called once per frame
    void Update()
    {
        switch(type){
            case 0:
            UpdateStandard();
            break;

            case 1:
            UpdateHoming();
            break;

            case 2:
            UpdateExploding();
            break;

            case 3:
            UpdateDuplicating();
            break;

            case 4:
            UpdateTeleporting();
            break;

            default:
            UpdateStandard();
            break;
        }

        currRotation += Time.deltaTime * rotationSpeed;
        transform.Rotate(0f, 0f, 180f / (float)Math.PI * Time.deltaTime * rotationSpeed);
        transform.RotateAround(pivotPoint, Vector3.forward, revSpeed * Time.deltaTime * 180f / (float)Math.PI);
        transform.position = transform.position + velocityScaled;
        pivotPoint = pivotPoint + velocityScaled;

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        
        if(!hasAppeared && (screenPos.x >= 0f && screenPos.x <= 1f && screenPos.y >= 0f && screenPos.y <= 1f)){
            hasAppeared = true;
        }

        if (hasAppeared && (screenPos.y < -0.1f || screenPos.y > 1.1f || screenPos.x < -0.1f || screenPos.x > 1.1f)) // Slightly off screen
        {
            Destroy(gameObject);
        }
    }

    void UpdateStandard()
    {
        velocityScaled = velocity * Time.deltaTime;
    }

    void UpdateHoming()
    {
        Vector3 probePos = GameStateManager.Instance.GetProbePosition();

        //stops homing behavior if below probe
        if(probePos.y > transform.position.y){
            homingAmount = 0f;
        }

        
        Vector3 normVelocity = velocityScaled.normalized;
        velocityScaled = normVelocity * (velocity * Time.deltaTime).magnitude;
        Vector3 debrisToProbeVector = (probePos - transform.position).normalized;

        //rotate asteroid 
        float velocityAngle = Mathf.Atan2(normVelocity.x, normVelocity.y);
        velocityAngle = velocityAngle < 0f ? 2 * Mathf.PI + velocityAngle : velocityAngle;
        float toProbeAngle = Mathf.Atan2(debrisToProbeVector.x, debrisToProbeVector.y);
        toProbeAngle = toProbeAngle < 0f ? 2 * Mathf.PI + toProbeAngle : toProbeAngle;
        float rotationAngle = toProbeAngle - velocityAngle;
        rotationAngle *= homingAmount;

        velocityScaled = rotate(velocityScaled, rotationAngle);
    }

    void UpdateExploding()
    {
        velocityScaled = velocity * Time.deltaTime;
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if(!activated && Mathf.Abs(screenPos.y - activationY) <= 0.01f){
            activated = true;
            for(int i = 0; i < numFragments; i++){
                GameObject newDebris = Instantiate(explodingFragments[i], transform.position, Quaternion.identity);

                //match relative size of original asteroid
                newDebris.transform.localScale = Vector3.one * 0.3f * debrisScale; //temporary scale of 0.3 because sprites are messed up

                DebrisMoveScript script = newDebris.GetComponent<DebrisMoveScript>();
                if (script != null)
                {
                    float baseAngle = transform.eulerAngles.z;
                    float fragmentAngle = (baseAngle + (360f / numFragments) * i) * Mathf.Deg2Rad;
                    Vector3 outward = new Vector3(Mathf.Cos(fragmentAngle), Mathf.Sin(fragmentAngle), 0f);
                    script.velocity = velocity + outward * fragmentSpreadSpeed;
                    script.type = 0;
                }
            }
            Destroy(gameObject);
        }
    }

    void UpdateDuplicating()
    {
        velocityScaled = velocity * Time.deltaTime;
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if(!activated && Mathf.Abs(screenPos.y - activationY) <= 0.01f){
            activated = true;
            GameObject copy = Instantiate(gameObject, transform.position, Quaternion.identity);

            //match relative size of original asteroid
            copy.transform.localScale = Vector3.one * debrisScale;

            DebrisMoveScript script = copy.GetComponent<DebrisMoveScript>();
            if (script != null)
            {
                Vector3 newVelocity = rotate(velocity, UnityEngine.Random.Range(-2f, 2f));
                script.velocity = newVelocity;
                script.type = 0;
            }
        }
    }

    void UpdateTeleporting()
    {
        velocityScaled = velocity * Time.deltaTime;
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if(!activated && screenPos.y <= 0.7f){
            activated = true;
            Vector3 destination = new Vector3(
                transform.position.x + Random.Range(-0.5f, 0.5f),
                transform.position.y - Random.Range(3f, 5f),
                0f
            );
            StartCoroutine(TeleportSequence(destination));
        }
    }

    private IEnumerator TeleportSequence(Vector3 destination)
    {
        Vector3 originalVelocity = velocity;
        velocity = Vector3.zero;
        velocityScaled = Vector3.zero;

        float totalDuration = 0.75f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr != null ? sr.color : Color.white;

        teleportGhost = CreateTeleportGhost(destination);

        float elapsed = 0f;
        float flashTimer = 0f;
        bool flashState = true;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            flashTimer += Time.deltaTime;

            float t = elapsed / totalDuration;
            float flashInterval = Mathf.Lerp(0.2f, 0.05f, t);

            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                flashState = !flashState;

                if (sr != null)
                    sr.color = flashState ? Color.white : new Color(1f, 0.2f, 0.2f, 1f);

                if (teleportGhost != null)
                {
                    SpriteRenderer ghostSr = teleportGhost.GetComponent<SpriteRenderer>();
                    if (ghostSr != null)
                        ghostSr.color = new Color(1f, 0f, 0f, flashState ? 0.6f : 0.15f);
                }
            }

            yield return null;
        }

        if (sr != null) sr.color = originalColor;
        if (teleportGhost != null) { Destroy(teleportGhost); teleportGhost = null; }

        transform.position = destination;
        velocity = originalVelocity;
        velocityScaled = originalVelocity * Time.deltaTime;
    }

    private GameObject CreateTeleportGhost(Vector3 position)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        GameObject ghost = new GameObject("TeleportGhost");
        ghost.transform.position = position;
        ghost.transform.localScale = transform.localScale;
        ghost.transform.rotation = transform.rotation;

        SpriteRenderer ghostSr = ghost.AddComponent<SpriteRenderer>();
        if (sr != null)
        {
            ghostSr.sprite = sr.sprite;
            ghostSr.sortingLayerName = sr.sortingLayerName;
            ghostSr.sortingOrder = sr.sortingOrder;
        }
        ghostSr.color = new Color(1f, 0f, 0f, 0.4f);

        return ghost;
    }

    private Vector3 rotate(Vector3 v, float radians)
    {
        Vector3 rotated = new Vector3(v.x * Mathf.Cos(radians) + v.y * Mathf.Sin(radians), v.x * -Mathf.Sin(radians) + v.y * Mathf.Cos(radians), v.z);
        return rotated;
    }

    private void OnCheckpointReached(object sender, EventArgs e){
        Destroy(gameObject);
    }
}
