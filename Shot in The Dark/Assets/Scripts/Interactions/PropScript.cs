using UnityEngine;

namespace Luci.Interactions
{
    public class PropScript : MonoBehaviour, IInteractable
    {
        public string itemName;
        public bool isHoldInteract = true;

        public void PressInteract()
        {
            //add a system eventually to pick up items? this must only be usable in the main hub then.
        }
    }
}
