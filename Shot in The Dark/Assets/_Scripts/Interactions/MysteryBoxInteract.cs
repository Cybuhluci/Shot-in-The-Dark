using UnityEngine;

namespace Luci.Interactions
{
    public class MysteryBoxInteract : MonoBehaviour, IInteractable
    {
        public bool isactive = true;
        public InteractType interactType = InteractType.MysteryBox;
        public MysteryBox box;
        public static int cost = 950; // 950: default cost for mystery box
        public bool isHoldInteract = true;

        public Transform boxLid;
        public Transform boxWeapon;

        public void PressInteract()
        {
            box.buyBox(cost, boxLid, boxWeapon, this);
        }

        public void ToggleInteract(bool isActive)
        {
            isactive = isActive;
        }
    }
}