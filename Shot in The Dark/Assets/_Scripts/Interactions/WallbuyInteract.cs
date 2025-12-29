using UnityEngine;

namespace Luci.Interactions
{
    public class WallbuyInteract : MonoBehaviour, IInteractable
    {
        public bool isactive = true;
        public InteractType interactType = InteractType.Purchasable;
        public PrimaryWeapon weapon; // weapon to buy, includes its name.
        public GunMainScript gunScript;
        public int cost = 750; // 750: default cost for wallbuy weapon
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
            gunScript.BuyWeapon(weapon, cost); // example: m16, 1200 = buying m16 for 1200 points
        }

        public void ToggleInteract(bool isActive)
        {
            isactive = isActive;
        }
    }
}