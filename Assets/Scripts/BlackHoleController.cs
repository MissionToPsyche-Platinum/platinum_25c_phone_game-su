using UnityEngine;

public class BlackHoleController : MonoBehaviour
{

    private GameObject probe;
    ProbeController probeController;
    public float size = 1f;
    public float initialSize = 0.001f;
    private float currSize;
    private float strength;
    private float minDistance;

    private float growthRate = 1f;
    private float shrinkRate = 2f;
    private float lifespan = 5f;

    bool keepGrowing = true;
    bool shrinking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        probe = GameObject.Find("Probe");
        probeController = probe.GetComponent<ProbeController>();
        lifespan = Random.Range(5f, 10f);
        minDistance = 0.1f;
        currSize = initialSize;
    }

    // Update is called once per frame
    void Update()
    {
        lifespan -= Time.deltaTime;
        transform.Rotate(0f, 0f, 50f * Time.deltaTime);

        CalculateForceVector();

        //growing animation as black hole is born
        if(keepGrowing){
            currSize += Time.deltaTime * growthRate;
            strength = currSize * 100f;
            transform.localScale = Vector3.one * currSize;
            if(currSize > size) {
                keepGrowing = false;
            }
        }
        
        if(lifespan <= 0f){
            shrinking = true;
            keepGrowing = false;
        }

        //collapsing animation when black hole dies
        if(shrinking){
            currSize -= Time.deltaTime * shrinkRate;
            strength = currSize * 100f;
            if(currSize <= 0f){
                Destroy(gameObject);
            }
            transform.localScale = Vector3.one * currSize;
        }
    }

    private void CalculateForceVector()
    {
        Vector3 probePosition = GameStateManager.Instance.GetProbePosition();
        Vector3 directionVector = transform.position - probePosition;
        float distance = directionVector.magnitude;
        if(distance < minDistance){
            Debug.Log("Absorbed by black hole");
            probeController.Kill();
            distance = 1f;
        }
        Vector3 forceVector = directionVector.normalized * strength / (distance * distance);
        Debug.Log(forceVector.magnitude);
        probeController.SetForceVector(forceVector);
    }
}
