using UnityEngine;

public class DebrisMoveScript : MonoBehaviour
{
    public float moveSpeed = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.down * moveSpeed) * Time.deltaTime;
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.y < -0.1f) // Slightly below screen
        {
            Destroy(gameObject);
        }
    }
}
