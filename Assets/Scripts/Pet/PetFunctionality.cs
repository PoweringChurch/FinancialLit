using UnityEngine;
public class PetFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        ignoreBase = true;
        base.Awake();
        globalActions["Toggle Follow"] = ToggleFollow;
        globalActions["Toggle Sit"] = ToggleSit;
    }
    void ToggleFollow()
    {
        PetBehaviour.Instance.followingCursor = !PetBehaviour.Instance.followingCursor;
    }
    void ToggleSit()
    {
        Debug.Log("sitting");
    }

    
}
