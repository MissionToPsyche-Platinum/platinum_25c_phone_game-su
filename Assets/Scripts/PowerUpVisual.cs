using UnityEngine;

public class PowerUpVisual : MonoBehaviour
{
    float maxScale = 0.1f;
    float minScale = 0.001f;
    float currScale;
    float totalTime = 0f;
    float growthAnimationLength = 1f;
    float aliveLength = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one * minScale;
        currScale = minScale;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        if(totalTime < growthAnimationLength)
        {
            currScale = Mathf.Sin( Mathf.PI / (2 * growthAnimationLength) * totalTime ) * ( maxScale - minScale ) + minScale;
            if(currScale > maxScale)
            {
                currScale = maxScale;
            }
            transform.localScale = Vector3.one * currScale;
        }

        if(totalTime >= aliveLength){
            Destroy(gameObject);
        }
    }
}
