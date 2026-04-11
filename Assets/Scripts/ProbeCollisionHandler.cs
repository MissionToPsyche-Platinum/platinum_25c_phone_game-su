using System;
using UnityEngine;

public class ProbeCollisionHandler : MonoBehaviour
{
    private ProbeHealth healthSystem;
    private PowerUpBehavior powerUpSystem;
    private bool hasDisplayedAsteroidPopup;
    [SerializeField] private GameObject asteroidPopupPrefab;

    [SerializeField] private AudioClip asteroidCollisionSoundClip;

    public event EventHandler<EventArgs> OnTakeDamage;
    public event EventHandler<EventArgs> OnCoinCollected;

    public bool HasCollided { get; private set; } = false;

    private void Awake()
    {
        healthSystem = GetComponent<ProbeHealth>();
        powerUpSystem = GetComponent<PowerUpBehavior>();
        hasDisplayedAsteroidPopup = false;
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
}