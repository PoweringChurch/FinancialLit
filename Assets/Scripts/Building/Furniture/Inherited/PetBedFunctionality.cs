using UnityEngine;

public class PetBedFunctionality : BaseFunctionality
{
    protected bool inUse;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go to Sleep"] = GoSleep;
    }
    protected virtual void GoSleep()
    {
        if (DefaultChecks())
        {
            return;
        }
        inUse = true;
        PetBehaviour.Instance.activeBehaviour = Behaviour.Occupied;
        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());
    }
    protected virtual void StopSleeping()
    {
        inUse = false;
        homeActions["Go to Sleep"] = GoSleep;
        homeActions.Remove("Stop Sleeping");
        PetStats.Instance.StopSleep();
        PetBehaviour.Instance.activeBehaviour = Behaviour.Default;
    }
    private void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        PetMover.Instance.petModel.position = PositionPetY();
        PetStats.Instance.StartSleep();
        homeActions.Remove("Go to Sleep");
        homeActions["Stop Sleeping"] = StopSleeping;
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