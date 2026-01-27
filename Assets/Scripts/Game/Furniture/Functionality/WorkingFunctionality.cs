public class WorkingFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go to work"] = BeginWork;
    }
    //Begin work when go to work is called upon
    protected void BeginWork()
    {
        UIWorkManager.Instance.EnterWork();
    }
}
