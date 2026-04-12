public class WorkingFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
        homeActions["Work"] = BeginWork;
    }
    // begin working
    protected void BeginWork()
    {
        UIWorkManager.Instance.EnterWork();
    }
}
