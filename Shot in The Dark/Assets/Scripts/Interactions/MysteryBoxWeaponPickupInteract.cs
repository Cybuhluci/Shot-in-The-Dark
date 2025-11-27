using UnityEngine;
namespace Luci.Interactions
{
    public class MysteryBoxWeaponPickupInteract : MonoBehaviour, IInteractable
    {
        public bool isactive = true;
        public InteractType interactType = InteractType.Pickup;
        public PrimaryWeapon weapon;
        public GunMainScript gunScript;
        public bool isHoldInteract = true;

        public void Awake()
        {
            if (gunScript == null)
            {
                gunScript = FindAnyObjectByType<GunMainScript>();
            }
        }

        public void PressInteract()
        {
            gunScript.BuyWeapon(weapon, 0);
            Destroy(gameObject);
        }

        public void ToggleInteract(bool isActive)
        {
            isactive = isActive;
        }
    }
}