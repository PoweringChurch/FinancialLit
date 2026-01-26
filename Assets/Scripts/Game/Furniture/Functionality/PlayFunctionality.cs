public class PlayFunctionality : BaseFunctionality
{
    protected bool inUse = false;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go play"] = GoPlay;
    }
    // called when go play action is pressed
    protected virtual void GoPlay()
    {
        if (DefaultChecks())
            return;
        
        inUse = true;
        // make pet occupied
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
    }
    // called when stop playing action is pressed
    protected virtual void StopPlaying()
    {
        inUse = false;
        // add go play action and remove stop playing action
        homeActions["Go play"] = GoPlay;
        homeActions.Remove("Stop playing");
        // make pet stop playing
        PetHelper.petStats.StopPlay();
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", false);
    }
    // called when pet reaches furniture after go play is pressed
    private void OnReached()
    {
        PetHelper.petMover.OnReachedGoal -= OnReached;
        PetHelper.petStats.StartPlay();
        homeActions.Remove("Go play");
        homeActions["Stop playing"] = StopPlaying;
        PetHelper.petAnimation.SetBoolParameter("IsPlaying", true);
    }
    // override for use check
    protected override void Move()
    {
        // check if in use
        if (inUse)
        {
            Message("In use!");
            return;
        }
        base.Move();
    }
    // override for use check
    protected override void Remove()
    {
        // check if in use
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