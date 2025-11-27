using UnityEngine;

namespace Luci.Interactions 
{
    public class PickupItemInteract : MonoBehaviour, IInteractable
    {
        public bool isactive = true;
        public string itemName;
        public PlayerInventory playerInventory;
        public bool isHoldInteract = false;

        public void PressInteract()
        {
            playerInventory.AddItemToInventory(this);
        }

        public void ToggleInteract(bool isActive)
        {
            isactive = isActive;
        }
    } 
}
