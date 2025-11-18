using UnityEngine;

namespace Luci.Interactions 
{
    public class PickupItemInteract : MonoBehaviour, IInteractable
    {
        public string itemName;
        public PlayerInventory playerInventory;
        public bool isHoldInteract = false;

        public void PressInteract()
        {
            playerInventory.AddItemToInventory(this);
        }
    } 
}
