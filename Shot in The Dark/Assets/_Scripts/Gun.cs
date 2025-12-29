using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Weapons/Gun")]
public class Gun : PrimaryWeapon
{
    public GunType gunType;
    public fireMode fireMode;
    public float fireRate; // rounds per minute

    public int magazineSize; // rounds per magazine
    public int reserveAmmo; // amount given on pickup, or starting reserve ammo
    public int maxReserveAmmo; // amount held at max from max ammo
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

public enum GunType
{
    Pistol,
    SMG,
    Rifle,
    Shotgun,
    Sniper,
    LMG,
    Heavy,
    Melee
}

public enum fireMode
{
    SemiAuto,
    Burst,
    FullAuto
}