using UnityEngine;
using UnityEngine.UI;

public class PerkManager : MonoBehaviour
{
    public PointsScript pointsScript; // Reference to the PointsScript to manage player points
    public GameObject perkIconHolder; // UI holder for perk icons   

    public void BuyPerk(PerkACola perk, int cost)
    {
        if (pointsScript.CanAfford(cost))
        {
            pointsScript.RemovePoints(cost);

            AddPerkACola(perk);
        }
        else
        {
            Debug.Log("Not enough points to buy perk!");
        }
    }

    public void AddPerkACola(PerkACola perk)
    {
        Debug.Log($"Perk A Cola added to player: {perk.perkName}");
        // Instantiate the perk icon in the UI
        GameObject icon = Instantiate(perk.perkObject, perkIconHolder.transform);
        icon.name = perk.perkName;
    }

    public void RemovePerkACola(PerkACola perk)
    {
        Debug.Log($"Perk A Cola removed from player: {perk.perkName}");
        // Find and destroy the perk icon in the UI
        Transform iconTransform = perkIconHolder.transform.Find(perk.perkName);
        if (iconTransform != null)
        {
            Destroy(iconTransform.gameObject);
        }
    }
}
