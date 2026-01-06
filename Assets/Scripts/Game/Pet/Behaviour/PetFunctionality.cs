using UnityEngine;

public class PetFunctionality : BaseFunctionality
{
    public ParticleSystem loveParticles;
    protected override void Awake()
    {
        ignoreBase = true;
        base.Awake();
        globalActions["Follow"] = Follow;
        globalActions["Sit"] = ToggleSit;

    }
    void Follow()
    {
        if (!PetHelper.petStateMachine.IsInState(PetState.Idle) || PetHelper.petBehaviour.ActiveBehaviour == Behaviour.Occupied) //it IS MEANT TO CHECK HERE FUTURE ME DONT DELETE
        {
            Message($"{PetHelper.petStats.petName} is occupied!");
            return;
        }
        
        PetHelper.petAnimation.SetBoolParameter("IsSitting",false);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;

        UICursor.Instance.SetCursor(UICursor.Instance.followingCursor);
        PlayerFlagManager.AddFlag(PlayerFlag.SetFollow);
        print("added set follow flag");
        PetHelper.petMover.OnReachedGoal += ReachedFollowTarget;
    }

    void ReachedFollowTarget()
    {
        PetHelper.petMover.OnReachedGoal -= ReachedFollowTarget;
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
    }
    void ToggleSit()
    {
        if (PetHelper.petStateMachine.IsInState(PetState.Sitting))
        {
            PetHelper.petStateMachine.SetState(PetState.Idle);
            globalActions.Remove("Rise");
            globalActions["Sit"] = ToggleSit;

            PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
            PetHelper.petAnimation.SetBoolParameter("IsSitting", false);
        }
        else
        {
            if (!PetHelper.petStateMachine.IsInState(PetState.Idle))
            {
                Message($"{PetHelper.petStats.petName} is occupied!");
                return;
            }
            PetHelper.petStateMachine.SetState(PetState.Sitting);
            globalActions.Remove("Sit");
            globalActions["Rise"] = ToggleSit;

            PetHelper.petMover.SetGoalPosition(PetHelper.petMover.petTransform.transform.position);
            PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
            PetHelper.petAnimation.SetBoolParameter("IsSitting", true);
        }
    }
}
