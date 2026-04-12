public class PetFoodBuyFunctionality : BaseFunctionality
{
    // override to make pet food enter players inventory instead of the furniture object
    protected override void Buy()
    {
        if (!FinancialSpending.Instance.CanAfford(price)) return;
        SFXPlayer.Instance.Play(purchaseSfx);
        
        FinancialSpending.Instance.Spend(price);
        PlayerResources.Instance.AddFood(1);
    }
}