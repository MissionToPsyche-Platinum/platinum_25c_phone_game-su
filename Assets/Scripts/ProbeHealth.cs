using UnityEngine;

public class ProbeHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public GameObject gameOverPanel;

    void Start()
    {
        currentHealth = maxHealth;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //Debug.Log("Game Over!");
        }
    }
}