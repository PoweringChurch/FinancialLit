public class ShampooBuyFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Buy()
    {
        if (!PlayerResources.Instance.CanAfford(price)) return;
        SFXPlayer.Instance.Play(purchaseSfx);
        PlayerResources.Instance.Spend(price, "Hygiene");
        PlayerResources.Instance.AddShampoo(1);
    }
}
