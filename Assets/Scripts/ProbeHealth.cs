using UnityEngine;
using System;

public class ProbeHealth : MonoBehaviour
{
    [SerializeField] private GameObject healthController;
    [SerializeField] private GameObject scoreSystem;
    private HealthUI healthUI;
    private ScoreIncrement score;

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
        healthUI = healthController.GetComponent<HealthUI>();
        score = scoreSystem.GetComponent<ScoreIncrement>();
        
        if (GameStateManager.Instance != null)
        {
            
            int calculatedMaxHealth = 3 + GameStateManager.Instance.maxHealthLevel;

            
            GameStateManager.Instance.startingHealth = calculatedMaxHealth;

            maxHealth = GameStateManager.Instance.startingHealth;
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

    void OnGameStart(object sender, GameStateManager.GameStateChangeEventArgs e)
    {
        if (GameStateManager.Instance != null)
        {
            
            int calculatedMaxHealth = 3 + GameStateManager.Instance.maxHealthLevel;
            GameStateManager.Instance.startingHealth = calculatedMaxHealth;

            maxHealth = GameStateManager.Instance.startingHealth;
            currentHealth = maxHealth;
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
                    return;
                }
            }

            if (shieldActive)
            {
                shieldHealth -= damage;
                if (shieldHealth <= 0)
                {
                    shieldActive = false;
                }
            }
            else
            {
                if(currentHealth >= damage)
                {
                    healthUI.TakeDamage(damage);
                } 
                else{
                    healthUI.TakeDamage(currentHealth);
                }
                currentHealth -= damage;
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameOverPanel.gameObject.SetActive(true);
            score.PauseScore();
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