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
        balance += amount;
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    // checks if the player can afford this amount
    public bool CanAfford(float amount)
    {
        return balance - amount > 0;
    }
}
