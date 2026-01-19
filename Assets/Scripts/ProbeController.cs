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

    void beginPowerUp(int index){

        switch(index) 
        {
            case 0: //Hyperspace

                //disable damage
                //smoothly increase speed to max
                //increase score rate proportionally
                break;
            case 1: //Shield

                //set shield active in probe health
                break;
            case 2: //2x

                //
                break;
            case 3: //Star

                break;
            case 4: //Hex

                break;
            default:
                Debug.Log("Invalid index given to beginPowerUp()");
                break;
        }
    }

    void endPowerUp(int index){
        switch(index) 
        {
            case 0:

                //smoothly decrease speed to normal
                //reset score rate to normal
                //enable damage
                break;
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            default:
                Debug.Log("Invalid index given to endPowerUp()");
                break;
        }
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

        if (collision.gameObject.CompareTag("PowerUpHyperspace"))
        {
            beginPowerUp(0);
            Debug.Log("Collision with Hyperspace power up");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUpShield"))
        {
            beginPowerUp(1);
            Debug.Log("Collision with Shield power up");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUp2x"))
        {
            beginPowerUp(2);
            Debug.Log("Collision with 2x power up");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUpStar"))
        {
            beginPowerUp(3);
            Debug.Log("Collision with Star power up");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("PowerUpHex"))
        {
            beginPowerUp(4);
            Debug.Log("Collision with Hex power up");
            Destroy(collision.gameObject);
        }




    }

    private void FixedUpdate() {
        //set the position of this game object to that of the finger position
        
        // Debug.Log("Movement vector: " +  myInputActions.Player.Movement.ReadValue<Vector2>());
        myRigidbody2D.linearVelocity = myInputActions.Player.Movement.ReadValue<Vector2>() * moveSpeed;
    }
}