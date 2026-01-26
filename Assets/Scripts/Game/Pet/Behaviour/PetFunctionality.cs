using UnityEngine;

public class PetFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        // ignoring the base so that we dont allow the player to delete the desk
        ignoreBase = true;
        base.Awake();
        globalActions["Follow"] = Follow;
        globalActions["Sit"] = ToggleSit;

    }
    // occurs when the follow action is pressed. essentially just lets the player set the pet's goal position. most of the "stuff" of this script actually occurs in PlayerInputHandler's HandleMisc function
    // might go back to fix
    void Follow()
    {
        // previously removed all instances of this exact line, but this one needs to stay
        if (!PetHelper.petStateMachine.IsInState(PetState.Idle) || PetHelper.petBehaviour.ActiveBehaviour == Behaviour.Occupied)
        {
            Message($"{PetHelper.petStats.petName} is occupied!");
            return;
        }
        // make pet stop sitting in prep to move
        PetHelper.petAnimation.SetBoolParameter("IsSitting",false);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        // set the cursor to the follow cursor
        UICursor.Instance.SetCursor(UICursor.Instance.followingCursor);
        PlayerFlagManager.AddFlag(PlayerFlag.SetFollow);
        
        PetHelper.petMover.OnReachedGoal += ReachedFollowTarget;
    }
    // occurs when the pet reaches the goal that was set by follow target
    void ReachedFollowTarget()
    {
        PetHelper.petMover.OnReachedGoal -= ReachedFollowTarget;
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
    }
    // called when the sit action is pressed; does as the name says
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
