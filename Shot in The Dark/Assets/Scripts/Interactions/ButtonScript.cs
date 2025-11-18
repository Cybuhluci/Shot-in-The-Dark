using UnityEngine;
using UnityEngine.Events;

namespace Luci.Interactions
{
    public class ButtonScript : MonoBehaviour, IInteractable
    {
        [Header("Button Events")]
        public UnityEvent LeftInteraction;
        public UnityEvent RightInteraction;
        public UnityEvent RegularInteraction;
        public UnityEvent ModifierInteraction;
        public bool isHoldInteract = false;

        public void PressInteract()
        {

        }
    }
}