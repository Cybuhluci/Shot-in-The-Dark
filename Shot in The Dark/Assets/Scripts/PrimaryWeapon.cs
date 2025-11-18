using UnityEngine;

public abstract class PrimaryWeapon : ScriptableObject
{
    public string weaponName;
    public GameObject weaponModel;

    public int damage;
    public Rank rank;
    public PackAPunchLevel packAPunchLevel;
    // Shared methods...

    public enum Rank
    {
        Normal = 1,
        Common = 2,
        Uncommon = 3,
        Rare = 4,
        Epic = 5,
        Legendary = 6
    }

    public enum PackAPunchLevel
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3
    }

    public enum MultiplierTarget
    {
        Head,
        Chest,
        Stomach,
        Limbs
    }
}
