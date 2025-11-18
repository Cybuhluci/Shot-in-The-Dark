using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Weapons/Gun")]
public class Gun : PrimaryWeapon
{
    public int magazineSize;
    public int reserveAmmo; // amount given on pickup, or starting reserve ammo
    public int maxReserveAmmo;
    public float fireRate; // rounds per minute
    public int bulletsPerShot; // e.g., shotguns fire multiple pellets per shot
    public int penetrationPower; // number of surfaces bullet can penetrate
    public float reloadTime;

    public BodyLocationMultiplier[] bodyLocationMultipliers;
}

[System.Serializable]
public class BodyLocationMultiplier
{
    public PrimaryWeapon.MultiplierTarget target;
    public float multiplier;
}