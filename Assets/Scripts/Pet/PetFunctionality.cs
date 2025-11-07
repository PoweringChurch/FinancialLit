using UnityEngine;
public class PetFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        ignoreBase = true;
        base.Awake();
        globalActions["Follow"] = Follow;
        globalActions["Sit"] = ToggleSit;
    }
    void Follow()
    {
        if (PetStateManager.HasState(PetState.Sitting))
        {
            Message("Your pet is sitting!");
            return;
        }
        PetStateManager.AddState(PetState.Following);
        PetBehaviour.Instance.SetCursor(PetBehaviour.Instance.followingCursor);
    }
    void ToggleSit()
    {
        if (PetStateManager.HasState(PetState.Sitting)) //if the pet is sitting
        {
            PetStateManager.RemoveState(PetState.Sitting); //remove sit state
            globalActions.Remove("Rise"); //remove rise
            globalActions["Sit"] = ToggleSit; //and assign sit
            Debug.Log("rise set");
        }
        else //otherwise if the pet is not sitting (will NOT have sitting)
        {
            PetStateManager.AddState(PetState.Sitting); //add sit state
            globalActions.Remove("Sit"); //remove sit
            globalActions["Rise"] = ToggleSit; //and assign rise
            Debug.Log("sit set");
        }
    }

    
}
