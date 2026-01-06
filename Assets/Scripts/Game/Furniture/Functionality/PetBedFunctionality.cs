public class PetBedFunctionality : BaseFunctionality
{
    protected bool inUse;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go rest"] = GoRest;
    }
    protected virtual void GoRest()
    {
        if (DefaultChecks())
        {
            return;
        }
        inUse = true;
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
    }
    protected virtual void StopResting()
    {
        inUse = false;
        homeActions["Go rest"] = GoRest;
        homeActions.Remove("Stop resting");
        PetHelper.petStats.StopSleep();
        PetHelper.petAnimation.SetBoolParameter("IsSitting",false);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
    }
    private void OnReached()
    {
        PetHelper.petMover.OnReachedGoal -= OnReached;
        PetHelper.petMover.petTransform.position = PositionPetY();
        PetHelper.petStats.StartSleep();
        PetHelper.petAnimation.SetBoolParameter("IsSitting",true);
        homeActions.Remove("Go rest");
        homeActions["Stop resting"] = StopResting;
    }
    protected override void Move()
    {
        if (inUse)
        {
            Message("In use!");
            return;
        }
        base.Move();
    }
    protected override void Remove()
    {
        if (inUse)
        {
            Message("In use!");
            return;
        } 
        base.Remove();
    }
    //safety
    void OnDestroy()
    {
        if (PetHelper.petMover != null) PetHelper.petMover.OnReachedGoal -= OnReached;
    }
}