using System.Collections.Generic;
using UnityEngine;

// handles spending parts of finance management
// keeps track of spending
public class FinancialSpending : MonoBehaviour
{
    public static FinancialSpending Instance;

    private float balance = 0; // players balance
    public float Balance => balance;

    public Dictionary<string, float> spending = new()
    {
        { "Healthcare", 0f},
        { "Pet care", 0f},
        { "Home decor", 0f}
    };
    public Dictionary<string, float> earning = new()
    {
        { "Work", 0f},
        { "Recycling", 0f},
        { "Returns", 0f}
    };
    void Awake() { Instance = this; }
    // spends the passed amount, also handles various other types of calcs
    public void Spend(float amount, string key = "Home decor")
    {
        balance -= amount;
        spending[key] += amount;
        FinanceUI.Instance.UpdateItem(key, true);
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    // only for use in the load function in savehandler
    public void SetBalance(float to)
    {
        balance = to;
    }
    public void Earn(float amount, string key = "Work")
    {
        balance += amount;
        earning[key] += amount;
        FinanceUI.Instance.UpdateItem(key, false);
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    // checks if the player can afford this amount
    public bool CanAfford(float amount)
    {
        return balance - amount > 0;
    }
}
