public class WallFunctionality : BaseFunctionality
{
    public Wall walldata;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Remove"] = Remove;
    }
    protected override void Remove()
    {
        FinancialSpending.Instance.Earn(walldata.sellVal);
        WallPlacement.Instance.placedWalls.Remove(walldata);
        Destroy(gameObject);
    }
}
