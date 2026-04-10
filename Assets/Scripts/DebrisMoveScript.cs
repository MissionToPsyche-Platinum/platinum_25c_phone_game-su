using UnityEngine;
using System;

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

    private Vector3 velocityScaled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocityScaled = velocity * Time.deltaTime;
        rotationSpeed = Random.Range(0.05f, 0.4f);
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

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        if(!hasAppeared && (screenPos.x >= 0f && screenPos.x <= 1f && screenPos.y >= 0f && screenPos.y <= 1f)){
            hasAppeared = true;
        }

        transform.position = transform.position + velocityScaled;
        pivotPoint = pivotPoint + velocityScaled;
        currRotation += Time.deltaTime * rotationSpeed;
        transform.Rotate(0f, 0f, 360f * Time.deltaTime * rotationSpeed);
        transform.RotateAround(pivotPoint, Vector3.forward, revSpeed * Time.deltaTime * 360);
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
                    Vector3 offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0f);
                    script.rotationSpeed = 0f;
                    script.velocity = velocity + offset;
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
        if(!activated && Mathf.Abs(screenPos.y - activationY) <= 0.01f){
            activated = true;
            transform.position = new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-3f, 3f), 0f);
        }
    }

    private Vector3 rotate(Vector3 v, float radians)
    {
        Vector3 rotated = new Vector3(v.x * Mathf.Cos(radians) + v.y * Mathf.Sin(radians), v.x * -Mathf.Sin(radians) + v.y * Mathf.Cos(radians), v.z);
        return rotated;
    }
}
