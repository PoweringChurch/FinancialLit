public class WorkingFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
        homeActions["Work"] = BeginWork;
    }
    protected void BeginWork()
    {
        UIWorkManager.Instance.EnterWork();
    }
}
