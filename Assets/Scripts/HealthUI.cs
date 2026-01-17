using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public ProbeHealth probeHealth;
    public Text healthText;

    void Update()
    {
        if (probeHealth != null && healthText != null)
        {
            healthText.text = "Health: " + probeHealth.currentHealth;
        }
    }
}