using UnityEngine;

public class PetFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        // ignoring the base so that we dont allow the player to delete the pet
        ignoreBase = true;
        base.Awake();
        globalActions["Follow"] = Follow;
        globalActions["Sit"] = ToggleSit;

    }
    // occurs when the follow action is pressed. essentially just lets the player set the pet's goal position. most of the "stuff" of this script actually occurs in PlayerInputHandler's HandleMisc function
    void Follow()
    {
        // previously removed all instances of this exact line, but this one needs to stay
        if (PetHelper.petStateMachine.CurrentState != PetState.Idle )
        {
            PlacementUtils.Message($"{PetHelper.petStats.petName} is occupied!", transform.position);
            return;
        }
        // make pet stop sitting in prep to move
        PetHelper.petAnimation.SetBoolParameter("IsSitting",false);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        // set the cursor to the follow cursor
        CursorUtils.Instance.SetCursor(CursorUtils.Instance.followingCursor);
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
        if (PetHelper.petStateMachine.CurrentState == PetState.Sitting) // if the pet is sitting 
        {
            PetHelper.petStateMachine.CurrentState = PetState.Idle; // make the pet idle in the state machine
            globalActions.Remove("Rise"); // remove the rise action, as the pet is now standing
            globalActions["Sit"] = ToggleSit; // and add the sit action

            PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default; // set the active behavior to default
            PetHelper.petAnimation.SetBoolParameter("IsSitting", false); // make the pet stop sitting anim
        }
        else // if the pet is not sitting
        {
            if ( PetHelper.petStateMachine.CurrentState != PetState.Idle || PetHelper.petBehaviour.ActiveBehaviour == Behaviour.Occupied) // if the pet is not idle
            {
                PlacementUtils.Message($"{PetHelper.petStats.petName} is occupied!", transform.position);
                return;
            }
            PetHelper.petStateMachine.CurrentState = PetState.Sitting; // make the petting in the state machine
            globalActions.Remove("Sit"); // remove the sit action, as the pet is now sitting
            globalActions["Rise"] = ToggleSit; // and add the rise action

            PetHelper.petMover.SetGoalPosition(PetHelper.petMover.petTransform.transform.position); // set the pet's goal position to it's current position, incase it is moving but not busy
            PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied; // set the active behaviour to occupied
            PetHelper.petAnimation.SetBoolParameter("IsSitting", true); // make the pet play sitting anim
        }
    }
}