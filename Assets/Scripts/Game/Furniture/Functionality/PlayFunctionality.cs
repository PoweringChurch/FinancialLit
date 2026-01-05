public class PlayFunctionality : BaseFunctionality
{
    protected bool inUse = false;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go play"] = GoPlay;
    }
    protected virtual void GoPlay()
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
    protected virtual void StopPlaying()
    {
        inUse = false;
        homeActions["Go play"] = GoPlay;
        homeActions.Remove("Stop playing");
        PetHelper.petStats.StopPlay();
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", false);
    }
    private void OnReached()
    {
        PetHelper.petMover.OnReachedGoal -= OnReached;
        PetHelper.petStats.StartPlay();
        homeActions.Remove("Go play");
        homeActions["Stop playing"] = StopPlaying;
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", true);
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