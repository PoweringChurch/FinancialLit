using System.Collections.Generic;
using UnityEngine;

// handles spending parts of finance management
// keeps track of spending
public class FinancialSpending : MonoBehaviour
{
    public static FinancialSpending Instance;

    private float balance = 0; // players balance
    private float spentToday = 0; // amount spent in the current day
    private float weeklyBills = 0; // amount due at the end of the week

    public float Balance => balance;

    void Awake() { Instance = this; }
    private Dictionary<string, float> spendings = new()
    {
        ["Home"] = 0f,
        ["Healthcare"] = 0f,
        ["Hygiene"] = 0f,
        ["Food"] = 0f,
    };

    // spends the passed amount, also handles various other types of calcs
    public void Spend(float amount, string purchaseType)
    {
        if (!spendings.ContainsKey(purchaseType))
        {
            Debug.LogWarning("No purchase type '"+purchaseType+"' exists");
            return;
        }
        spentToday += amount;
        spendings[purchaseType] += amount;
        balance -= amount;

        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    public void SetBalance(float to)
    {
        balance = to;
        print("Set balance to "+ to);
    }
    public void Earn(float amount)
    {
    }
    // checks if the player can afford this amount
    public bool CanAfford(float amount)
    {
        return balance - amount > 0;
    }
}
