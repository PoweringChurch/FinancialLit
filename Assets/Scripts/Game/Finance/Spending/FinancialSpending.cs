using System.Collections.Generic;
using UnityEngine;

// handles spending parts of finance management
// keeps track of spending
public class FinancialSpending : MonoBehaviour
{
    public static FinancialSpending Instance;

    private float balance = 0; // players balance
    public float Balance => balance;

    void Awake() { Instance = this; }

    // spends the passed amount, also handles various other types of calcs
    public void Spend(float amount)
    {
        balance -= amount;
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    // only for use in the load function in savehandler
    public void SetBalance(float to)
    {
        balance = to;
        print("Set balance to "+ to);
    }
    public void Earn(float amount)
    {
        balance += amount;
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    // checks if the player can afford this amount
    public bool CanAfford(float amount)
    {
        return balance - amount > 0;
    }
}
