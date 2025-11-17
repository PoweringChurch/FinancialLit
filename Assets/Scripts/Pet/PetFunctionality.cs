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
        if (!PetStateMachine.IsInState(PetState.Idle) || PetBehaviour.Instance.ActiveBehaviour == Behaviour.Occupied) //it IS MEANT TO CHECK HERE TRUST
        {
            Message($"{PetStats.Instance.PetName} is occupied!");
            return;
        }
        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Occupied;

        UIHandler.Instance.CursorHelper.SetCursor(UIHandler.Instance.CursorHelper.followingCursor);
        PlayerFlagManager.AddFlag(PlayerState.SetFollow);
        PetMover.Instance.OnReachedGoal += ReachedFollowTarget;
    }

    void ReachedFollowTarget()
    {
        Debug.Log("reached");
        PetMover.Instance.OnReachedGoal -= ReachedFollowTarget;
        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Default;
    }
    void ToggleSit()
    {
        if (PetStateMachine.IsInState(PetState.Sitting))
        {
            PetStateMachine.SetState(PetState.Idle);
            globalActions.Remove("Rise");
            globalActions["Sit"] = ToggleSit;

            PetBehaviour.Instance.ActiveBehaviour = Behaviour.Default;
            PetAnimation.Instance.SetBoolParameter("IsSitting", false);
        }
        else
        {
            if (!PetStateMachine.IsInState(PetState.Idle))
            {
                Message($"{PetStats.Instance.PetName} is occupied!");
                return;
            }
            PetStateMachine.SetState(PetState.Sitting);
            globalActions.Remove("Sit");
            globalActions["Rise"] = ToggleSit;

            PetMover.Instance.SetGoalPosition(PetMover.Instance.petTransform.transform.position);
            PetBehaviour.Instance.ActiveBehaviour = Behaviour.Occupied;
            PetAnimation.Instance.SetBoolParameter("IsSitting", true);
        }
    }

    
}
