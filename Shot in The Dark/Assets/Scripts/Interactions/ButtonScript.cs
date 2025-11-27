using UnityEngine;
using UnityEngine.Events;

namespace Luci.Interactions
{
    public class ButtonScript : MonoBehaviour, IInteractable
    {
        public bool isactive = true;

        [Header("Button Events")]
        public UnityEvent LeftInteraction;
        public UnityEvent RightInteraction;
        public UnityEvent RegularInteraction;
        public UnityEvent ModifierInteraction;
        public bool isHoldInteract = false;

        public void PressInteract()
        {

        }

        public void ToggleInteract(bool isActive)
        {
            isactive = isActive;
        }
    }
}