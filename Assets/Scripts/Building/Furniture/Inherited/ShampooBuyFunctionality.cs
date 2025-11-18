using UnityEngine;

public class ShampooBuyFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Buy()
    {
        PlayerResources.Instance.Spend(price, "Hygiene");
        PlayerResources.Instance.AddShampoo(1);
    }
}
