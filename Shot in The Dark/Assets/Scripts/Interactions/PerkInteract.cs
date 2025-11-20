using UnityEngine;

namespace Luci.Interactions
{

    public class PerkInteract : MonoBehaviour, IInteractable
    {
        public InteractType interactType = InteractType.Purchasable;
        public PerkACola perk; // weapon to buy, includes its name.
        public PerkManager perkScript;
        public int cost = 500; // 500: default cost for wallbuy weapon
        public bool isHoldInteract = true;

        public void PressInteract()
        {
            perkScript.BuyPerk(perk, cost); // example: Juggernog, 2500 = buying Juggernog for 2500 points = Hold F for Juggernog [Cost: 2500]
        }
    }
}
