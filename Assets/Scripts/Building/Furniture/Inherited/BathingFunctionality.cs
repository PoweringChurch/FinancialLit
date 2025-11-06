using UnityEngine;
using System;
public class BathingFunctionality : BaseFunctionality
{ 
    protected override void Awake()
    {
        base.Awake();
        actions["Clean Pet"] = CleanPet;
    }
    protected virtual void CleanPet()
    {
        if (!PlayerData.Instance.CanConsumeShampoo())
        {
            Message("No pet shampoo!");
            return;
        }
        PlayerData.Instance.ConsumeShampoo();
        Pet.Instance.CleanPet(0.5f);
    }
}