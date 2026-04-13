using UnityEngine;

public class MoveDownScript : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        transform.position += velocity * Time.deltaTime;
        if (screenPos.y < -10f)
        {
            Destroy(gameObject);
        }
    }
    
}