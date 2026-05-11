using System;
using System.Collections;
using UnityEngine;

public class ProbeCollisionHandler : MonoBehaviour
{
    private ProbeController probeController;
    private ProbeHealth healthSystem;
    private PowerUpBehavior powerUpSystem;
    private CameraShake cameraShakeScript;
    private bool hasDisplayedAsteroidPopup;
    [SerializeField] private GameObject asteroidPopupPrefab;

    [SerializeField] private AudioClip asteroidCollisionSoundClip;

    private Coroutine blinkCoroutine;
    private SpriteRenderer probeSprite;
    private bool justGotHit = false;

    public event EventHandler<EventArgs> OnTakeDamage;
    public event EventHandler<EventArgs> OnCoinCollected;

    public bool HasCollided { get; private set; } = false;

    private void Awake()
    {
        healthSystem = GetComponent<ProbeHealth>();
        powerUpSystem = GetComponent<PowerUpBehavior>();
        cameraShakeScript = GetComponent<CameraShake>();
        probeSprite = GetComponent<SpriteRenderer>();
        probeController = GetComponent<ProbeController>();
        hasDisplayedAsteroidPopup = false;
    }

    private void Start()
    {
        GameStateManager.Instance.OnStartPlaying += GameStateManager_OnStartPlaying;
    }

    private void GameStateManager_OnStartPlaying(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        justGotHit = false;
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.OnStartPlaying -= GameStateManager_OnStartPlaying;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Debris"))
        {
            //if the collided object is asteroid
            if (justGotHit)
            {
                return;
            }

            HasCollided = true;
            justGotHit = true;
            SFXController.instance.PlaySoundFXClip(asteroidCollisionSoundClip, transform, 1f);
            // Take damage
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(1);
                OnTakeDamage?.Invoke(this, EventArgs.Empty);
            }
            
            if (probeController != null)
            {
                probeController.ApplyKnockback(collision.transform.position);
            }
            
            // Destroy the debris/asteroid
            Destroy(collision.gameObject);


            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            
            StartCoroutine(cameraShakeScript.Shake(.3f, 1f));
            StartCoroutine(HitEffectRoutine());
            
            if (!hasDisplayedAsteroidPopup)
            {
                hasDisplayedAsteroidPopup = true;
                PopupManager.Instance.DisplayPopup(asteroidPopupPrefab, 5f);
            }
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
    
    private IEnumerator HitEffectRoutine()
    {
        yield return StartCoroutine(FreezeFrame(.15f));

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkAfterHit(2f, 0.1f));
    }

    public IEnumerator FreezeFrame(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
    }

    private IEnumerator BlinkAfterHit(float totalDuration, float blinkInterval)
    {
        if (probeSprite == null)
        {
            justGotHit = false;
            yield break;
        }
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            probeSprite.enabled = !probeSprite.enabled;
            yield return new WaitForSecondsRealtime(blinkInterval);
            elapsed += blinkInterval;
        }

        probeSprite.enabled = true;
        justGotHit = false;
        blinkCoroutine = null;
    }
}