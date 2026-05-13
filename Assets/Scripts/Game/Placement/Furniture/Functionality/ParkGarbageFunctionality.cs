public class ParkGarbageFunctionality : BaseFunctionality
{
    protected bool inUse;
    protected override void Awake()
    {
        ignoreBase = true;
        base.Awake();
        homeActions["Clean up"] = CleanUp;
    }
    public void CleanUp()
    {
        if (DefaultChecks())
            return;
        inUse = true;
        // occupy pet with this task
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
    }
    void OnReached()
    {
        FinancialSpending.Instance.Earn(15, "Recycling");
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        if (PetHelper.petMover != null) PetHelper.petMover.OnReachedGoal -= OnReached;
    }
}