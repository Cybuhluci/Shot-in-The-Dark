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
            healthScript.ChangeMaxHealth(healthIncrease, true); // increase max health
        }
    }

    private void OnDestroy()
    {
        if (healthScript != null)
        {
            healthScript.ChangeMaxHealth(healthIncrease, false); // revert max health increase
        }
    }
}
