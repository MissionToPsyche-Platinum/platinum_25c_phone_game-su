using UnityEngine;
using System;

public class ProbeHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public GameObject gameOverPanel;
    private bool damageActive;
    private bool shieldActive;
    private int shieldHealth;

    void Start()
    {
        
        damageActive = true;
        shieldActive = false;

        if (GameStateManager.Instance != null)
        {
            maxHealth = GameStateManager.Instance.startingHealth;
            Debug.Log("Max health set to: " + maxHealth);
        }

        currentHealth = maxHealth;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStartPlaying += OnGameStart;
        }
    }

    void OnDestroy()
    {
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnStartPlaying -= OnGameStart;
        }
    }

    void OnGameStart(object sender, EventArgs e)
    {
        
        if (GameStateManager.Instance != null)
        {
            maxHealth = GameStateManager.Instance.startingHealth;
            currentHealth = maxHealth;
            Debug.Log("Max health set to: " + maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (damageActive)
        {
            
            if (!shieldActive && GameStateManager.Instance != null)
            {
                int dodgeChance = GameStateManager.Instance.armorLevel * 10; 
                if (UnityEngine.Random.Range(0, 100) < dodgeChance)
                {
                    Debug.Log("Damage dodged by armor!");
                    return; 
                }
            }

            if (shieldActive)
            {
                shieldHealth -= damage;
                Debug.Log("Shield damaged");
                if (shieldHealth <= 0)
                {
                    Debug.Log("Shield broken");
                    shieldActive = false;
                }
            }
            else
            {
                currentHealth -= damage;
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameOverPanel.gameObject.SetActive(true);
            Debug.Log("Game Over!");
        }
    }

    public void AddShield(int shieldHP)
    {
        shieldActive = true;
        shieldHealth = shieldHP;
    }

    public void RemoveShield()
    {
        shieldActive = false;
        shieldHealth = 0;
    }

    public void DisableDamage()
    {
        damageActive = false;
    }

    public void EnableDamage()
    {
        damageActive = true;
    }

    public void SetToZeroLives()
    {
        TakeDamage(currentHealth);
    }
}