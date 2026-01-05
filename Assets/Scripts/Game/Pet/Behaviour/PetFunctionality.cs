using UnityEngine;

public class PetFunctionality : BaseFunctionality
{
    PetStateMachine petStateMachine;
    PetBehaviour petBehaviour;
    PetAnimation petAnimation;
    PetStats petStats;
    PetMover petMover;
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
        if (!petStateMachine.IsInState(PetState.Idle) || petBehaviour.ActiveBehaviour == Behaviour.Occupied) //it IS MEANT TO CHECK HERE FUTURE ME DONT DELETE
        {
            Message($"{petStats.petName} is occupied!");
            return;
        }
        
        petAnimation.SetBoolParameter("IsSitting",false);
        petBehaviour.ActiveBehaviour = Behaviour.Occupied;

        UICursor.Instance.SetCursor(UICursor.Instance.followingCursor);
        PlayerFlagManager.AddFlag(PlayerFlag.SetFollow);
        print("added set follow flag");
        petMover.OnReachedGoal += ReachedFollowTarget;
    }

    void ReachedFollowTarget()
    {
        petMover.OnReachedGoal -= ReachedFollowTarget;
        petBehaviour.ActiveBehaviour = Behaviour.Default;
    }
    void ToggleSit()
    {
        if (petStateMachine.IsInState(PetState.Sitting))
        {
            petStateMachine.SetState(PetState.Idle);
            globalActions.Remove("Rise");
            globalActions["Sit"] = ToggleSit;

            petBehaviour.ActiveBehaviour = Behaviour.Default;
            petAnimation.SetBoolParameter("IsSitting", false);
        }
        else
        {
            if (!petStateMachine.IsInState(PetState.Idle))
            {
                Message($"{petStats.petName} is occupied!");
                return;
            }
            petStateMachine.SetState(PetState.Sitting);
            globalActions.Remove("Sit");
            globalActions["Rise"] = ToggleSit;

            petMover.SetGoalPosition(petMover.petTransform.transform.position);
            petBehaviour.ActiveBehaviour = Behaviour.Occupied;
            petAnimation.SetBoolParameter("IsSitting", true);
        }
    }
}
