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
        Pet.Instance.CleanPet(0.1f);
    }
}