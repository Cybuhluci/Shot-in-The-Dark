using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public int health = 100;
    private int maxHealth = 250;

    public void AddHealth(int amount)
    {
        health += amount;
        
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void RemoveHealth(int amount)
    {
        health -= amount;

        if (health < 0)
        {
            health = 0;
            //link to die script later
        }
    }
}