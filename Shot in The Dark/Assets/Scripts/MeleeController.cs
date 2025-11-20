using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public Melee meleeData; // Assign in prefab or at runtime

    private float swingCooldown = 0f;
    private bool triggerReleasedSinceLastSwing = true;

    void Update()
    {
        if (swingCooldown > 0f)
            swingCooldown -= Time.deltaTime;

        triggerReleasedSinceLastSwing = true;
    }

    // Call this from GunMainScript, passing attack input state
    public void TryAttack(bool attackInput, bool attackInputDown)
    {
        if (swingCooldown > 0f) return;

        if (attackInputDown && triggerReleasedSinceLastSwing)
        {
            Attack();
            swingCooldown = 1f / meleeData.swingSpeed;
            triggerReleasedSinceLastSwing = false;
        }
    }

    private void Attack()
    {
        // TODO: Play animation, check for hit, apply damage, etc.
        Debug.Log($"Swung {meleeData.weaponName} for {meleeData.damage} damage!");
    }
}