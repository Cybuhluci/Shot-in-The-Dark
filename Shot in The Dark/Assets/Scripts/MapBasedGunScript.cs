using UnityEngine;

[CreateAssetMenu(fileName = "NewMapBasedGunScript", menuName = "Weapons/MapBasedGuns")]
public class MapBasedGunScript : ScriptableObject // mainly used to set the guns available on each map's mystery box
{
    public MapGuns[] Guns;
}

[System.Serializable]
public class MapGuns
{
    public string Name;
    public PrimaryWeapon Gun;
}