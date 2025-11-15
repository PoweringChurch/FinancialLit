using UnityEngine;
using System;
using UnityEngine.AI;
public class BathingFunctionality : BaseFunctionality
{
    [SerializeField] GameObject waterFill;
    protected bool inUse = false;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Bathe"] = Bathe;
    }
    protected virtual void Bathe()
    {
        if (!PlayerResources.Instance.CanConsumeShampoo())
        {
            Message("No pet shampoo!");
            return;
        }
        if (DefaultChecks())
        {
            return;
        }
        PetBehaviour.Instance.activeBehaviour = Behaviour.Occupied;

        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());

        inUse = true;
    }
    protected virtual void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        GetComponent<NavMeshObstacle>().enabled = false;

        PetMover.Instance.petTransform.LookAt(PositionPetY() + transform.right);
        PetMover.Instance.petTransform.position = PositionPetY();

        PlayerResources.Instance.ConsumeShampoo();
        PetStats.Instance.StartBathing();
        waterFill.SetActive(true);

        homeActions.Remove("Bathe");
        homeActions["Stop Bathing"] = StopBathing;

        PetAnimation.Instance.SetBoolParameter("IsSitting", true);
    }
    protected virtual void StopBathing()
    {
        GetComponent<NavMeshObstacle>().enabled = true;

        PetStats.Instance.StopBathing();
        waterFill.SetActive(false);

        homeActions["Bathe"] = Bathe;
        homeActions.Remove("Stop Bathing");

        PetAnimation.Instance.SetBoolParameter("IsSitting", false);
        PetBehaviour.Instance.activeBehaviour = Behaviour.Default;

        inUse = false;
    }
}