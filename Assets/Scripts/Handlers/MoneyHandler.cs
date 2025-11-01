using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MoneyHandler : MonoBehaviour
{
    public static MoneyHandler Instance;
    private int totalSpent = 0;
    private int money = 0;
    public int TotalSpent => totalSpent;
    public int Money => money;
    void Awake()
    {
        Instance = this;
    }
    public void Spend(int cost)
    {
        if (CanAfford(cost))
        {
            totalSpent += cost;
            money -= cost;
            return;
        }
        Debug.Log("too expensive broke");
    }
    //helper
    public bool CanAfford(int cost)
    {
        if (money - cost <= 0)
        {
            return false;
        }
        return true;
    }
    public void AddMoney(int amount)
    {
        money += amount;
    }
}
