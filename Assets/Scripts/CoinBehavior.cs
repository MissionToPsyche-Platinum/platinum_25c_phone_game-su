using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public float lifetime = 3f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }

        
        if (timer >= lifetime - 0.5f)
        {
            
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.enabled = (Time.time % 0.2f) > 0.1f;
            }
        }
    }
}