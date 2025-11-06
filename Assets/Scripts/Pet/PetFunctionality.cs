using UnityEngine;

public class PetFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
        ignoreBase = true;
        actions["Follow Cursor"] = Follow;
        actions["Toggle Sit"] = ToggleSit;
    }
    void Follow()
    {
        Debug.Log("now following");
    }
    void ToggleSit()
    {
        Debug.Log("sitting");
    }
}
