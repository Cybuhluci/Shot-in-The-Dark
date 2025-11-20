using UnityEngine;

public class Juggernog : MonoBehaviour
{
    // This perk increases the player's health
    public int healthIncrease = 100;
    private HealthScript healthScript;

    private void OnEnable()
    {
        healthScript = GetComponent<HealthScript>();
        if (healthScript != null)
        {
            healthScript.ChangeMaxHealth(healthIncrease, true);
        }
    }

    private void OnDisable()
    {
        if (healthScript != null)
        {
            healthScript.ChangeMaxHealth(healthIncrease, false);
        }
    }
}
