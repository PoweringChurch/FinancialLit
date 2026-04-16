public class PetBedFunctionality : BaseFunctionality
{
    protected bool inUse;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go rest"] = GoRest;
    }
    // called when go rest action is pressed
    protected virtual void GoRest()
    {
        if (DefaultChecks())
            return;
        inUse = true;
        // occupy pet with this task
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
    }
    // called when stop resting action is pressed
    protected virtual void StopResting()
    {
        inUse = false;
        // add go rest action and remove stop resting action
        homeActions["Go rest"] = GoRest;
        homeActions.Remove("Stop resting");
        // make the pet stop sleeping
        PetHelper.petStats.StopSleep();
        PetHelper.petAnimation.SetBoolParameter("IsSitting",false);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
    }
    // called when the pet reaches the location after go rest action is pressed
    private void OnReached()
    {
        // disconnect onreached
        PetHelper.petMover.OnReachedGoal -= OnReached;
        // set the pets position to over the furniture
        PetHelper.petMover.petTransform.position = PositionPetY();
        // make pet sleep
        PetHelper.petStats.StartSleep();
        PetHelper.petAnimation.SetBoolParameter("IsSitting",true);
        // remove go rest action and add stop resting action
        homeActions.Remove("Go rest");
        homeActions["Stop resting"] = StopResting;
    }
    // only override to add use checks
    protected override void Move()
    {
        if (inUse)
        {
            Message("In use!");
            return;
        }
        base.Move();
    }
    // only override to add use checks
    protected override void Remove()
    {
        if (inUse)
        {
            Message("In use!");
            return;
        } 
        base.Remove();
    }
    // safety
    void OnDestroy()
    {
        if (PetHelper.petMover != null) PetHelper.petMover.OnReachedGoal -= OnReached;
    }
}