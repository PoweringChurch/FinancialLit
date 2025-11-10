using UnityEngine;

public class PetFoodBuyFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Buy()
    {
        PlayerResources.Instance.Spend(price, "Food");
        PlayerResources.Instance.AddFood(1);
    }
}