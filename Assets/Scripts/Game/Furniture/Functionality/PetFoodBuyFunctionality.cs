public class PetFoodBuyFunctionality : BaseFunctionality
{
    // override to make pet food enter players inventory instead of the item
    protected override void Buy()
    {
        if (!PlayerResources.Instance.CanAfford(price)) return;
        SFXPlayer.Instance.Play(purchaseSfx);
        
        PlayerResources.Instance.Spend(price, "Food");
        PlayerResources.Instance.AddFood(1);
    }
}