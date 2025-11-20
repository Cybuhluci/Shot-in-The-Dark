using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GunMainScript : MonoBehaviour
{
    [System.Serializable]
    public class WeaponInstance
    {
        public PrimaryWeapon weaponData;
        public GameObject weaponModel;
    }

    // List to hold weapon instances (ScriptableObject + model)
    public List<WeaponInstance> weapons = new List<WeaponInstance>();
    public int maxWeapons = 2; // Set to 3 if Mule Kick is active

    public PlayerInput playerInput;
    public GameObject weaponHolder; // assign in inspector
    public PrimaryWeapon StartingWeaponData; // assign in inspector (ScriptableObject)

    public PointsScript pointsScript;

    public int currentWeaponIndex = 0;

    private void Start()
    {
        AddNewWeapon(StartingWeaponData); // Start with 1 weapon
    }

    private void Update()
    {
        if (playerInput.actions["Next"].triggered)
        {
            SwitchWeapon(1);
        }
        else if (playerInput.actions["Previous"].triggered)
        {
            SwitchWeapon(-1);
        }

        // Shooting
        if (playerInput.actions["Fire"].IsPressed())
        {
            var gunController = GetCurrentWeaponModel()?.GetComponent<GunController>();
            if (gunController != null)
            {
                bool fireInput = playerInput.actions["Fire"].IsPressed();
                bool fireInputDown = playerInput.actions["Fire"].WasPressedThisFrame();
                gunController.TryFire(fireInput, fireInputDown);
            }
            else
            {
                var meleeController = GetCurrentWeaponModel()?.GetComponent<MeleeController>();
                if (meleeController != null)
                {
                    bool fireInput = playerInput.actions["Fire"].IsPressed();
                    bool fireInputDown = playerInput.actions["Fire"].WasPressedThisFrame();
                    meleeController.TryAttack(fireInput, fireInputDown);
                }
            }
        }
        // Reloading
        if (playerInput.actions["Reload"].WasPressedThisFrame())
        {
            var gunController = GetCurrentWeaponModel()?.GetComponent<GunController>();
            gunController?.Reload();
        }
    }

    public void SwitchWeapon(int direction)
    {
        if (weapons.Count == 0) return;
        int prevIndex = currentWeaponIndex;
        currentWeaponIndex += direction;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Count - 1;
        else if (currentWeaponIndex >= weapons.Count)
            currentWeaponIndex = 0;
        ShowOnlyCurrentWeaponModel();
    }

    private void ShowOnlyCurrentWeaponModel()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].weaponModel != null)
                weapons[i].weaponModel.SetActive(i == currentWeaponIndex);
        }
    }

    public void BuyWeapon(PrimaryWeapon weapon, int cost)
    {
         if (pointsScript.CanAfford(cost))
         {
             pointsScript.RemovePoints(cost);
             
             if (weapons.Count < maxWeapons)
             {
                 AddNewWeapon(weapon);
             }
             else
             {
                 ExchangeWeapon(currentWeaponIndex, weapon);
            }
        }
         else
         {
             Debug.Log("Not enough points to buy weapon!");
        }
    }

    // Add a new weapon (if inventory not full)
    public void AddNewWeapon(PrimaryWeapon weaponData)
    {
        if (weapons.Count >= maxWeapons)
        {
            Debug.LogWarning("Weapon inventory full!");
            return;
        }
        if (weaponData.weaponModel == null)
        {
            Debug.LogError($"Weapon data {weaponData.weaponName} has no weaponModel assigned!");
            return;
        }
        GameObject model = Instantiate(weaponData.weaponModel, weaponHolder.transform);
        model.SetActive(false); // Hide by default
        WeaponInstance instance = new WeaponInstance { weaponData = weaponData, weaponModel = model };
        weapons.Add(instance);
        currentWeaponIndex = weapons.Count - 1;
        ShowOnlyCurrentWeaponModel();
    }

    // Exchange weapon at index with a new one
    public void ExchangeWeapon(int index, PrimaryWeapon newWeaponData)
    {
        if (index < 0 || index >= weapons.Count) return;
        RemoveWeapon(index);
        // Insert new weapon at the same index
        if (newWeaponData.weaponModel == null)
        {
            Debug.LogError($"Weapon data {newWeaponData.weaponName} has no weaponModel assigned!");
            return;
        }
        GameObject model = Instantiate(newWeaponData.weaponModel, weaponHolder.transform);
        model.SetActive(false);
        WeaponInstance instance = new WeaponInstance { weaponData = newWeaponData, weaponModel = model };
        weapons.Insert(index, instance);
        currentWeaponIndex = index;
        ShowOnlyCurrentWeaponModel();
    }

    // Remove weapon at index
    public void RemoveWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;
        if (weapons[index].weaponModel != null)
            Destroy(weapons[index].weaponModel);
        weapons.RemoveAt(index);
        if (currentWeaponIndex >= weapons.Count)
            currentWeaponIndex = weapons.Count - 1;
        ShowOnlyCurrentWeaponModel();
    }

    // Get the current weapon's ScriptableObject
    public PrimaryWeapon GetCurrentWeaponData()
    {
        if (weapons.Count == 0) return null;
        return weapons[currentWeaponIndex].weaponData;
    }

    // Get the current weapon's model GameObject
    public GameObject GetCurrentWeaponModel()
    {
        if (weapons.Count == 0) return null;
        return weapons[currentWeaponIndex].weaponModel;
    }

    public GunController GunController()
    {
        return GetCurrentWeaponModel()?.GetComponent<GunController>();
    }

    public MeleeController MeleeController()
    {
        return GetCurrentWeaponModel()?.GetComponent<MeleeController>();
    }
}
