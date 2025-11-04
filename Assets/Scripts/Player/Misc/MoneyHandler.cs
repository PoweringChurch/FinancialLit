using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MoneyHandler : MonoBehaviour
{
    public static MoneyHandler Instance;
    private Dictionary<string, float> spendings = new Dictionary<string, float>()
    {
        ["Furniture"] = 0f,
        ["Healthcare"] = 0f,
        ["Hygiene"] = 0f,
        ["Food"] = 0f,
    };
    private float money = 0;
    public Dictionary<string, float> Spendings => spendings;
    public float Money => money;

    void Awake()
    {
        Instance = this;
    }
    public void Spend(float cost, string purchaseType)
    {
        if (!spendings.ContainsKey(purchaseType))
        {
            Debug.Log(purchaseType + " does not exist");
            return;
        }
        if (CanAfford(cost))
        {
            spendings[purchaseType] += cost;
            money -= cost;
            return;
        }
        Debug.Log("too expensive broke");
    }
    //helper
    public bool CanAfford(float cost)
    {
        if (money - cost <= 0)
        {
            return false;
        }
        return true;
    }
    public void AddMoney(float amount)
    {
        money += amount;
    }
}
