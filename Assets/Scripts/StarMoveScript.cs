using UnityEngine;
using System;

public class StarMoveScript : MonoBehaviour
{
    public Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        transform.position += velocity * Time.deltaTime;
        if (screenPos.y < -0.1f) // Slightly below screen
        {
            Destroy(gameObject);
        }
    }
}