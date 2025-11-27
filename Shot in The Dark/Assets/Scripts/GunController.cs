using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    public Gun gunData; // Assign in prefab or at runtime

    public int currentAmmo;
    public int reserveAmmo;
    public float fireCooldown = 0f;
    public bool isBursting = false;
    public bool triggerReleasedSinceLastShot = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gunData != null)
        {
            currentAmmo = gunData.magazineSize;
            reserveAmmo = gunData.reserveAmmo;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        // Always update triggerReleasedSinceLastShot at the end
        triggerReleasedSinceLastShot = true;
    }

    // Call this from GunMainScript, passing fire input state
    public void TryFire(bool fireInput, bool fireInputDown)
    {
        if (gunData == null || isBursting) return;

        switch (gunData.fireMode)
        {
            case fireMode.SemiAuto:
                if (fireInputDown && triggerReleasedSinceLastShot)
                {
                    FireOnce();
                    triggerReleasedSinceLastShot = false;
                }
                break;
            case fireMode.FullAuto:
                if (fireInput)
                {
                    FireOnce();
                }
                break;
            case fireMode.Burst:
                if (fireInputDown)
                {
                    StartCoroutine(BurstFire());
                }
                break;
        }
    }

    private void FireOnce()
    {
        if (fireCooldown > 0f || currentAmmo <= 0)
            return;

        Attack();
        fireCooldown = 60f / gunData.fireRate; // fireRate is rounds per minute
        currentAmmo--;
    }

    private IEnumerator BurstFire()
    {
        isBursting = true;
        int burstCount = gunData.bulletsPerShot > 1 ? gunData.bulletsPerShot : 3; // Default to 3-round burst if not set
        float burstDelay = 60f / gunData.fireRate;

        for (int i = 0; i < burstCount; i++)
        {
            if (currentAmmo > 0)
            {
                Attack();
                currentAmmo--;
            }
            else
            {
                break;
            }
            if (i < burstCount - 1)
                yield return new WaitForSeconds(burstDelay);
        }
        fireCooldown = burstDelay; // Add a delay after burst
        isBursting = false;
    }

    private void Attack()
    {
        // TODO: Raycast or spawn projectile, play effects, etc.
        for (int i = 0; i < gunData.bulletsPerShot; i++)
        {
            Debug.Log($"Fired bullet {i + 1} of {gunData.bulletsPerShot} from {gunData.weaponName} for {gunData.damage} damage!");
        }
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => gunData != null ? gunData.magazineSize : 0;
    public int GetReserveAmmo() => reserveAmmo;
    public int GetMaxReserveAmmo() => gunData != null ? gunData.maxReserveAmmo : 0;

    public void Reload()
    {
        if (gunData == null) return;
        int needed = gunData.magazineSize - currentAmmo;
        int toReload = Mathf.Min(needed, reserveAmmo);
        if (toReload > 0)
        {
            currentAmmo += toReload;
            reserveAmmo -= toReload;
        }
    }
}
