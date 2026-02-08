using UnityEngine;
using System.Collections.Generic;

public class PlayerResources : MonoBehaviour
{
    public static PlayerResources Instance;
    private int food = 10;
    private int shampoo = 10;
    void Awake()
    {
        Instance = this;

        PlayerFlagManager.AddFlag(PlayerFlag.Home);
    }
    public int Food => food;
    public int Shampoo => shampoo;
    public void ConsumeFood()
    {
        if (CanConsumeFood())
        {
            food -= 1;
            UIOverlay.Instance.UpdateResourcesAndBal();
        }
    }
    public void ConsumeShampoo()
    {
        if (CanConsumeShampoo())
        {
            shampoo -= 1;
            UIOverlay.Instance.UpdateResourcesAndBal();
        }
    }
    public void AddFood(int count)
    {
        food += count;
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    public void AddShampoo(int count)
    {
        shampoo += count;
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    public void SetShampoo(int to)
    {
        shampoo = to;
    }
    public void SetFood(int to)
    {
        food = to;
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
}