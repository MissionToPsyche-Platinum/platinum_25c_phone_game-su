using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProbeController : MonoBehaviour
{
    private InputSystem_Actions myInputActions;
    private Rigidbody2D myRigidbody2D;
    private ProbeHealth healthSystem;
    private PowerUpBehavior powerUpSystem;
    private Vector2 minAllowedPosition;
    private Vector2 maxAllowedPosition;
    private Vector2 additionalForceVector;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private AudioClip asteroidCollisionSoundClip;
    [SerializeField] private float extraPadding = 0.1f; // Adjust value to keep entire probe from going offscreen

    public event EventHandler<EventArgs> OnTakeDamage;
    public event EventHandler<EventArgs> OnCoinCollected;

    public int coinAmount = 0;

    private bool gasActive = false;
    public float gasActiveLength = 10f;
    private float gasTimer = 0f;
        
    public bool HasCollided { get; private set; } = false;

    private void Awake() {
        coinAmount = 0;
        myInputActions = new InputSystem_Actions();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        healthSystem = GetComponent<ProbeHealth>();
        powerUpSystem = GetComponent<PowerUpBehavior>();
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
        GameStateManager.Instance.OnStartPlaying += OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying += OnStopPlaying;
        CalculateScreenBoundaries();
        additionalForceVector = new Vector3(0f, 0f, 0f);
        
       
    }

    // Calculates padding to add to keep probe fully on screen
    private void CalculateScreenBoundaries()
    {
        Camera cameraMain = Camera.main;
        // Viewport provides coordinates with (0,0) being bottom left and (1,1) being top right
        Vector2 minScreen = cameraMain.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector2 maxScreen = cameraMain.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Getting the component provides us the width of the probe, if not default to probe being 1f wide
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float playerHalfWidth = 0.5f; 
        float playerHalfHeight = 0.5f;

        if (spriteRenderer != null)
        {
            playerHalfWidth = spriteRenderer.bounds.extents.x;
            playerHalfHeight = spriteRenderer.bounds.extents.y;
        }
        
        // Adding half the player width ensures the entire probe remains on screen when colliding with the edge
        minAllowedPosition.x = minScreen.x + playerHalfWidth + extraPadding;
        minAllowedPosition.y = minScreen.y + playerHalfHeight + extraPadding;

        maxAllowedPosition.x = maxScreen.x - playerHalfWidth - extraPadding;
        maxAllowedPosition.y = maxScreen.y - playerHalfHeight - extraPadding;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= OnStartPlaying;
        GameStateManager.Instance.OnStopPlaying -= OnStopPlaying;
    }

    private void OnEnable()
    {
        myInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        myInputActions.Player.Disable();
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Debris"))
        {
            //if the collided object is asteroid
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

        if (collision.gameObject.CompareTag("Gas"))
        {
            //colliding with gas causes joystick direction to reverse
            if(!gasActive){
                gasActive = true;
                moveSpeed *= -1f;
            }
            gasTimer = gasActiveLength;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Coin"))
        {
            CoinBehavior coinScript = collision.gameObject.GetComponent<CoinBehavior>();
            GameStateManager.Instance.AddCoins(coinScript.collect());
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
        if(gasActive){
            gasTimer -= Time.deltaTime;
            if(gasTimer <= 0f){
                gasActive = false;
                moveSpeed *= -1f;
            }
        }

        myRigidbody2D.linearVelocity = myInputActions.Player.Movement.ReadValue<Vector2>() * moveSpeed + additionalForceVector * Time.deltaTime;
    }
    
    // Used here so this happens at the very end of a frame as to not mess up linearVelocity calculation
    private void LateUpdate() 
    {
        Vector3 viewPos = transform.position;
        
        // Clamp ensures the probe is within an allowed postion, if not it sets the value to either min or max
        viewPos.x = Mathf.Clamp(viewPos.x, minAllowedPosition.x, maxAllowedPosition.x);
        viewPos.y = Mathf.Clamp(viewPos.y, minAllowedPosition.y, maxAllowedPosition.y);

        transform.position = viewPos;
        GameStateManager.Instance.UpdateProbePosition(transform.position);
    }

    private void OnStartPlaying(object sender, EventArgs e)
    {
        // Debug.Log("ProbeController heard OnStartPlaying");
        this.transform.position = GameStateManager.Instance.startingProbePosition;
        
        this.gameObject.SetActive(true);
        if (GameStateManager.Instance != null)
        {
            moveSpeed = 3f + (GameStateManager.Instance.speedLevel * 0.5f);
        }
        ApplySkin();
    }
    
    private void OnStopPlaying(object sender, EventArgs e)
    {
        this.gameObject.SetActive(false);
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    //for additional forces that affect probe movement
    public void SetForceVector(Vector2 newForce)
    {
        additionalForceVector = newForce;
    }

    public void Kill()
    {
        healthSystem.SetToZeroLives();
    }

    void ApplySkin()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && GameStateManager.Instance != null)
        {
            int skinIndex = GameStateManager.Instance.GetProbeSkin();

            switch (skinIndex)
            {
                case 0:
                    spriteRenderer.color = Color.white; // Default
                    break;
                case 1:
                    spriteRenderer.color = Color.blue; // Blue
                    break;
            }

            Debug.Log("Applied skin: " + skinIndex + ", Color: " + spriteRenderer.color);
        }
    }
}