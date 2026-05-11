using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProbeController : MonoBehaviour
{
    private InputSystem_Actions myInputActions;
    private Rigidbody2D myRigidbody2D;
    private ProbeHealth healthSystem;
    private Vector2 minAllowedPosition;
    private Vector2 maxAllowedPosition;
    private Vector2 additionalForceVector;
    private float _lateralMultiplier = 1f;
    private float _visionMultiplier  = 1f;
    private float _baseCameraSize;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float extraPadding = 0.1f; // Adjust value to keep entire probe from going offscreen
    [SerializeField] private float knockbackStrength = 7f;
    [SerializeField] private float knockbackDecayRate = 12f;
    [SerializeField] private float probeControlLoss = 1f;
    [SerializeField] private float controlRecoveryLerpSpeed = 6f;
    private float currentControlMultiplier = 1f;

    public int coinAmount = 0;
        

    private void Awake() {
        coinAmount = 0;
        myInputActions = new InputSystem_Actions();
        myRigidbody2D = GetComponent<Rigidbody2D>();

        healthSystem = GetComponent<ProbeHealth>();
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
        _baseCameraSize = Camera.main.orthographicSize;
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

        // Navigation Camera: expand movement bounds outward from center
        if (_visionMultiplier != 1f)
        {
            float cy = (minAllowedPosition.y + maxAllowedPosition.y) * 0.5f;
            float hh = (maxAllowedPosition.y - minAllowedPosition.y) * 0.5f;
            minAllowedPosition.y = cy - hh * _visionMultiplier;
            maxAllowedPosition.y = cy + hh * _visionMultiplier;
        }
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


    private void FixedUpdate() {
        // Debug.Log("Movement vector: " +  myInputActions.Player.Movement.ReadValue<Vector2>());

        Vector2 input = myInputActions.Player.Movement.ReadValue<Vector2>();
        Vector2 scaledInput = new Vector2(input.x * _lateralMultiplier, input.y) * moveSpeed * ProbeUpgradeManager.Instance.GetSpeedMultiplier() * currentControlMultiplier;
        
        myRigidbody2D.linearVelocity = scaledInput + additionalForceVector;

        //slowly lower the knockback's force
        additionalForceVector *= Mathf.Exp(-knockbackDecayRate * Time.fixedDeltaTime);
        if (additionalForceVector.sqrMagnitude < 0.0001f)
        {
            additionalForceVector = Vector2.zero;
        }
        
        //slowly give the probe back control
        currentControlMultiplier = Mathf.Lerp(
            currentControlMultiplier,
            1f,
            controlRecoveryLerpSpeed * Time.fixedDeltaTime
        );
        if (Mathf.Abs(currentControlMultiplier - 1f) < 0.001f)
        {
            currentControlMultiplier = 1f;
        }
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
    
    public void ApplyKnockback(Vector2 hitSourcePosition)
    {
        Vector2 direction = ((Vector2)transform.position - hitSourcePosition).normalized;
        additionalForceVector = direction * knockbackStrength;
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
        if (ComponentManager.Instance != null)
        {
            _lateralMultiplier = ComponentManager.Instance.GetLateralSpeedMultiplier();
            _visionMultiplier  = ComponentManager.Instance.GetVisionMultiplier();
            Camera.main.orthographicSize = _baseCameraSize * _visionMultiplier;
            CalculateScreenBoundaries();
        }
        ApplySkin();

        additionalForceVector = Vector2.zero;
        currentControlMultiplier = 1f;
    }
    
    private void OnStopPlaying(object sender, EventArgs e)
    {
        this.gameObject.SetActive(false);
        Camera.main.orthographicSize = _baseCameraSize;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public void SetLateralSpeedMultiplier(float m)
    {
        _lateralMultiplier = m;
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