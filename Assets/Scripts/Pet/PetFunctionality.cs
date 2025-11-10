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
        if (PetBehaviour.Instance.activeBehaviour == Behaviour.Occupied)
        {
            Message($"{PetStats.Instance.PetName} is occupied!");
            return;
        }
        if (PetStateManager.HasState(PetState.Sitting))
        {
            Message("Your pet is sitting!");
            return;
        }
        
        PetBehaviour.Instance.activeBehaviour = Behaviour.Occupied;
        UIHandler.Instance.CursorHelper.SetCursor(UIHandler.Instance.CursorHelper.followingCursor);
        PlayerStateManager.AddState(PlayerState.SetFollow);
        PetMover.Instance.OnReachedGoal += ReachedFollowTarget;
    }

    void ReachedFollowTarget()
    {
        Debug.Log("reached");
        PetMover.Instance.OnReachedGoal -= ReachedFollowTarget;
        PetBehaviour.Instance.activeBehaviour = Behaviour.Default;
    }
    void ToggleSit()
    {
        if (PetStateManager.HasState(PetState.Sitting))
        {
            //not checking for occupied as we know what its occupied with (sitting)
            PetStateManager.RemoveState(PetState.Sitting);
            globalActions.Remove("Rise");
            globalActions["Sit"] = ToggleSit;

            PetBehaviour.Instance.activeBehaviour = Behaviour.Default;
            PetAnimation.Instance.SetBoolParameter("IsSitting", false);
        }
        else
        {
            if (PetBehaviour.Instance.activeBehaviour == Behaviour.Occupied)
            {
                Message($"{PetStats.Instance.PetName} is occupied!");
                return;
            }
            PetStateManager.AddState(PetState.Sitting);
            globalActions.Remove("Sit");
            globalActions["Rise"] = ToggleSit;

            PetMover.Instance.SetGoalPosition(PetMover.Instance.petModel.transform.position);
            PetBehaviour.Instance.activeBehaviour = Behaviour.Occupied;
            PetAnimation.Instance.SetBoolParameter("IsSitting", true);
        }
    }

    
}
