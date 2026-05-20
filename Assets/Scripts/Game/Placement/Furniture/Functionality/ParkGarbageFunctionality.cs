public class ParkGarbageFunctionality : BaseFunctionality
{
    protected bool inUse;
    protected override void Awake()
    {
        ignoreBase = true;
        globalActions["Clean up"] = CleanUp;
    }
    public void CleanUp()
    {
        if ( PetHelper.petStateMachine.CurrentState != PetState.Idle || PetHelper.petBehaviour.ActiveBehaviour == Behaviour.Occupied) {
            PlacementUtils.Message($"{PetHelper.petStats.petName} is occupied!", transform.position);
            return; }
        inUse = true;
        // occupy pet with this task
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
    }
    void OnReached()
    {
        FinancialSpending.Instance.Earn(25, "Recycling");
        RecyclingJob.Instance.trashCount--;
        PlacementUtils.Message("+$25.00",transform.position);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
        PetHelper.petMover.OnReachedGoal -= OnReached;
        Destroy(gameObject);
    }
}