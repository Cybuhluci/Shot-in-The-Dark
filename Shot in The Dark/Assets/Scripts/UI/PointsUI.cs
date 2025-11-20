using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    public PointsScript pointsScript;
    public TMP_Text pointsText;

    public void LateUpdate()
    {
        pointsText.text = pointsScript.points.ToString();
    }

    // later add animation for when points are added and removed using a instantiated text object
}
