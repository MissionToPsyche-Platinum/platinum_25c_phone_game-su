using UnityEngine;
using UnityEngine.InputSystem;

public class ProbeController : MonoBehaviour
{
    private InputSystem_Actions myInputActions;
    private Rigidbody2D myRigidbody2D;
    private ProbeHealth healthSystem;

    [SerializeField] private float moveSpeed = 1f;
        
    public bool HasCollided { get; private set; } = false;
    private void Awake() {
        myInputActions = new InputSystem_Actions();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        myInputActions.Player.Enable();

        healthSystem = GetComponent<ProbeHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<AsteroidController>() != null)
        {
            //if the collided object is asteroid
            Debug.Log("Collision with asteroid!");
            HasCollided = true;

            // Take damage
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(1);
            }
            // Destroy the debris/asteroid
            Destroy(collision.gameObject);
        }




    }

    private void FixedUpdate() {
        //set the position of this game object to that of the finger position
        
        // Debug.Log("Movement vector: " +  myInputActions.Player.Movement.ReadValue<Vector2>());
        myRigidbody2D.linearVelocity = myInputActions.Player.Movement.ReadValue<Vector2>() * moveSpeed;
    }
}