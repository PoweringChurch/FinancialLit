using UnityEngine;
using System;
public class BathingFunctionality : BaseFunctionality
{ 
    protected override void Awake()
    {
        base.Awake();
        homeActions["Go Clean"] = GoClean;
    }
    protected virtual void GoClean()
    {
        if (!PlayerResources.Instance.CanConsumeShampoo())
        {
            Message("No pet shampoo!");
            return;
        }
        if (PetBehaviour.Instance.activeBehaviour == Behaviour.Occupied)
        {
            Message($"{PetStats.Instance.PetName} is occupied!");
            return;
        }
        PetBehaviour.Instance.activeBehaviour = Behaviour.Occupied;
        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());
    }
    protected virtual void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        PetMover.Instance.petModel.position = PositionPetY();
        PetBehaviour.Instance.activeBehaviour = Behaviour.Default;

        PlayerResources.Instance.ConsumeShampoo();
        PetStats.Instance.CleanPet(0.5f);
    }
}