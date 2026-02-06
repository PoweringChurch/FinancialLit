using UnityEngine;

// handles spending parts of finance management
// keeps track of spending
public class FinancialSpending : MonoBehaviour
{
    public static FinancialSpending Instance;

    private float balance = 0; // players balance
    private float spentToday = 0; // amount spent in the current day
    private float weeklyBills = 0; // amount due at the end of the week

    void Awake() { Instance = this; }

    // spends the passed amount, also handles various other types of calcs
    public void Spend(float amount, string purchaseType)
    {
        spentToday += amount;
        balance -= amount;
    }
    public void Earn(float amount)
    {
    }
    // checks if the player can afford this amount
    public bool CanAfford(float amount)
    {
        return balance - amount > 0;
    }
    // bills increase based on water, electricity, gas, necessities, memberships
    // necessities is fixed
    // memberships make certain tasks easier (discounts?)
    // taking baths, having many electronics in house, or travelling frequently increases respective val
    public void CalculateBills()
    {

    }
}
