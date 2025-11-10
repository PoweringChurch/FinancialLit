using UnityEngine;

public class PlayFunctionality : BaseFunctionality
{
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go Play"] = GoPlay;
    }
    protected virtual void GoPlay()
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
    protected virtual void StopPlaying()
    {
        homeActions["Go Play"] = GoPlay;
        homeActions.Remove("Stop Playing");
        PetStats.Instance.StopPlay();
        PetBehaviour.Instance.activeBehaviour = Behaviour.Default;
        PetAnimation.Instance.SetBoolParameter("IsPlaying", false);
    }
    private void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        PetMover.Instance.petModel.position = PositionPetY();
        PetStats.Instance.StartPlay();
        homeActions.Remove("Go Play");
        homeActions["Stop Playing"] = StopPlaying;
        PetAnimation.Instance.SetBoolParameter("IsPlaying", true);
    }
    //safety
    void OnDestroy()
    {
        if (PetMover.Instance != null) PetMover.Instance.OnReachedGoal -= OnReached;
    }
}