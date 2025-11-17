using UnityEngine;

public class WorkingFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        ignoreBase = true;
        base.Awake();
        globalActions["Begin Working"] = BeginWork;
    }
    protected void BeginWork()
    {
        UIHandler.Instance.WorkHandler.EnterWork();
    }
}
