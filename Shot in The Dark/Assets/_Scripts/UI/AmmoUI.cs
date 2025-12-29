using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    public GunMainScript gunscript;
    public TMP_Text ammoText;

    private void Update()
    {
        var weaponModel = gunscript.GetCurrentWeaponModel();
        if (weaponModel == null)
        {
            ammoText.text = "";
            return;
        }

        var gunController = weaponModel.GetComponent<GunController>();
        if (gunController != null)
        {
            ammoText.text = $"{gunController.GetCurrentAmmo()}/{gunController.GetReserveAmmo()}";
        }
        else
        {
            // Assume melee if not a gun
            ammoText.text = "0";
        }
    }
}
