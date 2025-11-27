using UnityEngine;

[CreateAssetMenu(fileName = "NewMapBasedGunScript", menuName = "Weapons/MapBasedGuns")]
public class MapBasedGunScript : ScriptableObject // mainly used to set the guns available on each map's mystery box
{
    public MapGuns[] Guns;
    public MapPerks[] Perks;
}

[System.Serializable]
public class MapGuns
{
    public string Name;
    public PrimaryWeapon Gun;
}

[System.Serializable]
public class MapPerks
{
    public string Name;
    public GameObject Perk;
    public PerkACola PerkData;
}