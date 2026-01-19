using UnityEngine;

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
        currentHealth = maxHealth;
        damageActive = true;
        shieldActive = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if(damageActive){
            if(shieldActive){
                shieldHealth -= damage;
                Debug.Log("Shield damaged");
                if(shieldHealth <= 0){
                    Debug.Log("Shield broken");
                    shieldActive = false;
                }
            } else {
                currentHealth -= damage;
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Game Over!");
        }
    }

    public void AddShield(int shieldHP){
        shieldActive = true;
        shieldHealth = shieldHP;
    }

    public void RemoveShield(){
        shieldActive = false;
        shieldHealth = 0;
    }

    public void DisableDamage(){
        damageActive = false;
    }

    public void EnableDamage(){
        damageActive = true;
    }
}