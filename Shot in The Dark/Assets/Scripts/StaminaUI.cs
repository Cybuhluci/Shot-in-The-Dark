using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public FirstPersonController personcontrol;
    public Slider staminaSlider;

    private void Start()
    {
        // Initialize the stamina slider's max value
        staminaSlider.maxValue = personcontrol.MaxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the stamina UI based on the player's stamina
        float stamina = personcontrol.Stamina;
        // Update the UI element here
        staminaSlider.value = stamina;
    }
}
