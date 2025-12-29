using StarterAssets;
using UnityEngine;

public class StaminUp : MonoBehaviour
{
    public float staminaIncrease = 50f;
    public float regenIncrease = 10f;
    private FirstPersonController controller;
    private float originalMaxStamina;
    private float originalRegenRate;

    private void OnEnable()
    {
        controller = GetComponent<FirstPersonController>();
        if (controller != null)
        {
            originalMaxStamina = controller.MaxStamina;
            originalRegenRate = controller.StaminaRegenRate;
            controller.MaxStamina += staminaIncrease;
            controller.StaminaRegenRate += regenIncrease;
            controller.Stamina = controller.MaxStamina; // Optionally refill stamina
        }
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.MaxStamina = originalMaxStamina;
            controller.StaminaRegenRate = originalRegenRate;
            if (controller.Stamina > controller.MaxStamina)
                controller.Stamina = controller.MaxStamina;
        }
    }
}
