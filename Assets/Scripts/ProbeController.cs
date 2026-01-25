using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProbeController : MonoBehaviour, IListenToStartGame
{
    private InputSystem_Actions myInputActions;
    private Rigidbody2D myRigidbody2D;
    private ProbeHealth healthSystem;
    private PowerUpBehavior powerUpSystem;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private AudioClip asteroidCollisionSoundClip;

    public event EventHandler<EventArgs> OnTakeDamage;
        
    public bool HasCollided { get; private set; } = false;
    private void Awake() {
        myInputActions = new InputSystem_Actions();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        myInputActions.Player.Enable();

        healthSystem = GetComponent<ProbeHealth>();
        powerUpSystem = GetComponent<PowerUpBehavior>();
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<AsteroidController>() != null)
        {
            //if the collided object is asteroid
            Debug.Log("Collision with asteroid!");
            HasCollided = true;
            SFXController.instance.PlaySoundFXClip(asteroidCollisionSoundClip, transform, 1f);
            // Take damage
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(1);
                OnTakeDamage?.Invoke(this, EventArgs.Empty);
            }
            // Destroy the debris/asteroid
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUpHyperspace"))
        {
            powerUpSystem.beginPowerUp(0);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUpShield"))
        {
            powerUpSystem.beginPowerUp(1);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUp2x"))
        {
            powerUpSystem.beginPowerUp(2);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUpStar"))
        {
            powerUpSystem.beginPowerUp(3);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUpHex"))
        {
            powerUpSystem.beginPowerUp(4);
            Destroy(collision.gameObject);
        }
    }

    private void FixedUpdate() {
        //set the position of this game object to that of the finger position
        
        // Debug.Log("Movement vector: " +  myInputActions.Player.Movement.ReadValue<Vector2>());
        myRigidbody2D.linearVelocity = myInputActions.Player.Movement.ReadValue<Vector2>() * moveSpeed;
    }

    public void OnStartGame()
    {
        this.gameObject.SetActive(true);
    }
}