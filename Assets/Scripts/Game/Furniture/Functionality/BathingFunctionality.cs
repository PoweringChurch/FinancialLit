using UnityEngine;
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
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;

        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());

        inUse = true;
    }
    protected virtual void OnReached()
    {
        PetHelper.petMover.OnReachedGoal -= OnReached;
        GetComponent<NavMeshObstacle>().enabled = false;

        PetHelper.petMover.petTransform.LookAt(PositionPetY() + transform.right);
        PetHelper.petMover.petTransform.position = PositionPetY();

        PlayerResources.Instance.ConsumeShampoo();
        PetHelper.petStats.StartBathing();
        waterFill.SetActive(true);

        homeActions.Remove("Bathe");
        homeActions["Stop bathing"] = StopBathing;

        PetHelper.petAnimation.SetBoolParameter("IsSitting", true);
    }
    protected virtual void StopBathing()
    {
        GetComponent<NavMeshObstacle>().enabled = true;

        PetHelper.petStats.StopBathing();
        waterFill.SetActive(false);

        homeActions["Bathe"] = Bathe;
        homeActions.Remove("Stop bathing");

        PetHelper.petAnimation.SetBoolParameter("IsSitting", false);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;

        inUse = false;
    }
}