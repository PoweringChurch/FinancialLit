public class ShampooBuyFunctionality : BaseFunctionality
{
    // override to make add inventory instead of adding furniture object
    protected override void Buy()
    {
        if (!FinancialSpending.Instance.CanAfford(price)) return;
        SFXPlayer.Instance.Play(purchaseSfx);
        FinancialSpending.Instance.Spend(price, "Hygiene");
        PlayerResources.Instance.AddShampoo(1);
    }
}
