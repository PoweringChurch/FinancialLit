public class WorkingFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go to work!"] = BeginWork;
    }
    protected void BeginWork()
    {
        UIHandler.Instance.WorkManager.EnterWork();
    }
}
