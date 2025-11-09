using UnityEngine;

public class FeedingFunctionality : BaseFunctionality
{
    protected bool filled = false;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go Eat"] = GoEat;
        homeActions["Refill"] = Refill;
    }
    protected virtual void GoEat()
    {
        if (!filled)
        {
            Message("Not filled!");
            return;
        }
        if (PetBehaviour.Instance.activeBehaviour == Behaviour.Occupied)
        {
            Message($"{PetStats.Instance.PetName} is occupied!");
            return;
        }
        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());
        
        
    }
    protected virtual void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        PetMover.Instance.petModel.position = PositionPetY();
        PetBehaviour.Instance.activeBehaviour = Behaviour.Default;

        PetStats.Instance.FeedPet(0.4f);
        filled = false;
    }
    protected virtual void Refill()
    {
        if (filled)
        {
            Message("Already filled!");
            return;
        }
        if (!PlayerResources.Instance.CanConsumeFood())
        {
            Message("No pet food!");
            return;
        }
        PlayerResources.Instance.ConsumeFood();
        filled = true;
    }
}
