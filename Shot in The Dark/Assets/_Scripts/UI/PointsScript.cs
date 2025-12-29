using UnityEngine;

public class PointsScript : MonoBehaviour
{
    public int points = 500;
    private int maxPoints = 100000000;
    public bool isDoublePointsActive = false;
    public GunMainScript gunmain;

    public void AddPoints(int amount)
    {
        if (isDoublePointsActive)
        {
            points += amount * 2;
        }
        else
        {
            points += amount;
        }

        if (points > maxPoints)
        {
            points = maxPoints;
        }
    }

    public void RemovePoints(int amount)
    {
        points -= amount;
    }

    public void ResetPoints()
    {
        points = 0;
    }

    public bool CanAfford(int cost)
    {
        return points >= cost;
    }
}
