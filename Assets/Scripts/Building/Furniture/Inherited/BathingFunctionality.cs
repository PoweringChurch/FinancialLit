using UnityEngine;
using System;
public class BathingFunctionality : BaseFunctionality
{ 
    protected override void Awake()
    {
        base.Awake();
        homeActions["Clean Pet"] = CleanPet;
    }
    protected virtual void CleanPet()
    {
        if (!PlayerResources.Instance.CanConsumeShampoo())
        {
            Message("No pet shampoo!");
            return;
        }
        PlayerResources.Instance.ConsumeShampoo();
        PetStats.Instance.CleanPet(0.5f);
    }
}