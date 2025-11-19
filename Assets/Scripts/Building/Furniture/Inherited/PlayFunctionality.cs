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
        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Occupied;
        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());
    }
    protected virtual void StopPlaying()
    {
        inUse = false;
        homeActions["Go play"] = GoPlay;
        homeActions.Remove("Stop playing");
        PetStats.Instance.StopPlay();
        PetBehaviour.Instance.ActiveBehaviour = Behaviour.Default;
        PetAnimation.Instance.SetBoolParameter("IsPlaying", false);
    }
    private void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        PetStats.Instance.StartPlay();
        homeActions.Remove("Go play");
        homeActions["Stop playing"] = StopPlaying;
        PetAnimation.Instance.SetBoolParameter("IsPlaying", true);
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