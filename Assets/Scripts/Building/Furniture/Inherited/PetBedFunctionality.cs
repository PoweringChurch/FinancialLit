using UnityEngine;

public class PetBedFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go to Sleep"] = GoSleep;
    }
    protected virtual void GoSleep()
    {
        if (PetBehaviour.Instance.activeBehaviour == Behaviour.Occupied)
        {
            Message($"{PetStats.Instance.PetName} is occupied!");
            return;
        }
        PetBehaviour.Instance.activeBehaviour = Behaviour.Occupied;
        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());
    }
    protected virtual void StopSleeping()
    {
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
    //safety
    void OnDestroy()
    {
        if (PetMover.Instance != null) PetMover.Instance.OnReachedGoal -= OnReached;
    }
}