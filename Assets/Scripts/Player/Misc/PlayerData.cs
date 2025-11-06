using UnityEngine;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    private float money = 10.01f;
    private int food = 10;
    private int shampoo = 10;
    void Awake()
    {
        Instance = this;
    }
    private Dictionary<string, float> spendings = new()
    {
        ["Furniture"] = 0f,
        ["Healthcare"] = 0f,
        ["Hygiene"] = 0f,
        ["Food"] = 0f,
    };

    public Dictionary<string, float> Spendings => spendings;
    public float Money => money;
    public int Food => food;
    public int Shampoo => shampoo;

    public void Spend(float cost, string purchaseType)
    {
        if (!spendings.ContainsKey(purchaseType))
        {
            return;
        }
        if (CanAfford(cost))
        {
            spendings[purchaseType] += cost;
            money -= cost;
            UIHandler.Instance.ItemUpdater.UpdateText();
            return;
        }
    }
    public void ConsumeFood()
    {
        if (CanConsumeFood())
        {
            food -= 1;
            UIHandler.Instance.ItemUpdater.UpdateText();
        }
    }
    public void ConsumeShampoo()
    {
        if (CanConsumeShampoo())
        {
            shampoo -= 1;
            UIHandler.Instance.ItemUpdater.UpdateText();

        }
    }
    //helper
    public bool CanConsumeFood()
    {
        return (food - 1) >= 0;
    }
    public bool CanConsumeShampoo()
    {
        return (shampoo - 1) >= 0;
    }
    public bool CanAfford(float cost)
    {
        return (money - cost) >= 0;
    }
    public void AddMoney(float amount)
    {
        money += amount;
        UIHandler.Instance.ItemUpdater.UpdateText();
    }
}