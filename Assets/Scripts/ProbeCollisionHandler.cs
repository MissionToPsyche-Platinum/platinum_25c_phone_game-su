using System;
using System.Collections;
using UnityEngine;

public class ProbeCollisionHandler : MonoBehaviour
{
    private ProbeController probeController;
    private ProbeHealth healthSystem;
    private PowerUpBehavior powerUpSystem;
    private CameraShake cameraShakeScript;
    private StatsScreenController statsScreenController;
    private bool hasDisplayedAsteroidPopup;

    [SerializeField] private AudioClip asteroidCollisionSoundClip;
    [SerializeField] private AudioClip dodgeSoundClip;
    [SerializeField] private GameObject statsScreenPanel;

    private Coroutine blinkCoroutine;
    private SpriteRenderer probeSprite;
    private bool justGotHit = false;

    public class DamageEventArgs : EventArgs
    {
        public int newHealth;

        public DamageEventArgs(int newHealth)
        {
            this.newHealth = newHealth;
        }
    }
    
    public class CoinCollectedEventArgs : EventArgs
    {
        public int value;
        public CoinCollectedEventArgs(int value) { this.value = value; }
    }

    public event EventHandler<DamageEventArgs> OnTakeDamage;
    public event EventHandler<CoinCollectedEventArgs> OnCoinCollected;

    public bool HasCollided { get; private set; } = false;

    private void Awake()
    {
        healthSystem = GetComponent<ProbeHealth>();
        powerUpSystem = GetComponent<PowerUpBehavior>();
        cameraShakeScript = GetComponent<CameraShake>();
        probeSprite = GetComponent<SpriteRenderer>();
        probeController = GetComponent<ProbeController>();
        statsScreenController = statsScreenPanel.GetComponent<StatsScreenController>();
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

            bool dodged = false;
            if (healthSystem != null)
            {
                dodged = healthSystem.TakeDamage(1);
                if (!dodged)
                    OnTakeDamage?.Invoke(this, new DamageEventArgs(healthSystem.currentHealth));
            }

            if (dodged)
            {
                if (dodgeSoundClip != null)
                    SFXController.instance.PlaySoundFXClip(dodgeSoundClip, transform, 1f);
            }
            else
            {
                SFXController.instance.PlaySoundFXClip(asteroidCollisionSoundClip, transform, 1f);
                if (probeController != null)
                    probeController.ApplyKnockback(collision.transform.position);

                GameStateManager.Instance.TotalAsteroidsHit++;
            }

            // Destroy the debris/asteroid
            Destroy(collision.gameObject);

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (dodged)
            {
                StartCoroutine(DodgeFlashRoutine());
            }
            else
            {
                StartCoroutine(cameraShakeScript.Shake(.3f, 1f));
                StartCoroutine(HitEffectRoutine());
            }
        }

        if (collision.gameObject.CompareTag("Coin"))
        {
            CoinBehavior coinScript = collision.gameObject.GetComponent<CoinBehavior>();
            int numCoins = coinScript.collect();
            GameStateManager.Instance.AddCoins(numCoins);
            statsScreenController.updateCoinsCollected(numCoins);
            OnCoinCollected?.Invoke(this, new CoinCollectedEventArgs(numCoins));
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
    
    private IEnumerator DodgeFlashRoutine()
    {
        if (probeSprite == null) { justGotHit = false; yield break; }
        Color original = probeSprite.color;
        probeSprite.color = Color.green;
        yield return new WaitForSecondsRealtime(0.2f);
        probeSprite.color = original;
        justGotHit = false;
    }

    private IEnumerator HitEffectRoutine()
    {
        yield return StartCoroutine(FreezeFrame(.25f));

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