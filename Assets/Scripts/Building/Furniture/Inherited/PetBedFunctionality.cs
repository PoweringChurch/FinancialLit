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
        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Occupied;
        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());
    }
    protected virtual void StopResting()
    {
        inUse = false;
        homeActions["Go rest"] = GoRest;
        homeActions.Remove("Stop resting");
        PetStats.Instance.StopSleep();
        PetAnimation.Instance.SetBoolParameter("IsSitting",false);
        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Default;
    }
    private void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        PetMover.Instance.petTransform.position = PositionPetY();
        PetStats.Instance.StartSleep();
        PetAnimation.Instance.SetBoolParameter("IsSitting",true);
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
        if (PetMover.Instance != null) PetMover.Instance.OnReachedGoal -= OnReached;
    }
}