using UnityEngine;
using UnityEngine.AI;
public class BathingFunctionality : BaseFunctionality
{
    // the gameobject that appears when the bath is being used
    [SerializeField] GameObject waterFill;
    
    protected bool inUse = false;
    protected override void Awake()
    {
        // use base actions, add bathe action
        base.Awake();
        homeActions["Bathe"] = Bathe;
    }
    // called when player selects the bathe option
    protected virtual void Bathe()
    {
        // check if the player can consume shampoo
        if (!PlayerResources.Instance.CanConsumeShampoo())
        {
            PlacementUtils.Message("No pet shampoo!", transform.position);
            return;
        }
        // default checks
        if (PetHelper.petStateMachine.CurrentState != PetState.Idle || PetHelper.petBehaviour.ActiveBehaviour == Behaviour.Occupied) {
            PlacementUtils.Message($"{PetHelper.petStats.petName} is occupied!", transform.position);
            return; }
        // set pet as occupied
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        // when pet has reached goal position, run onreached
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
        inUse = true;
    }
    // runs when pet reaches goal; should only ever be called when goes to bathe
    protected virtual void OnReached()
    {
        // remove assignment
        PetHelper.petMover.OnReachedGoal -= OnReached;
        // disable navmeshobstacle so pet will sit still on the bath
        GetComponent<NavMeshObstacle>().enabled = false;
        // place pet
        PetHelper.petMover.petTransform.LookAt(PositionPetY() + transform.right);
        PetHelper.petMover.petTransform.position = PositionPetY();
        // consume shampoo & start bathing
        PlayerResources.Instance.ConsumeShampoo();
        PetHelper.petStats.StartBathing();
        waterFill.SetActive(true);
        // remove bathing action and add stop bathing action
        homeActions.Remove("Bathe");
        homeActions["Stop bathing"] = StopBathing;
        // make pet sit
        PetHelper.petAnimation.SetBoolParameter("IsSitting", true);
    }
    protected override void Move()
    {
        if (!inUse)
            base.Move();
        else
            PlacementUtils.Message("The bath is in use!", transform.position);
    }
    protected override void Remove()
    {
        if (!inUse)
            base.Remove();
        else
            PlacementUtils.Message("The bath is in use!", transform.position);
    }
    // stop bathing pet, called when stop bathing action is called
    protected virtual void StopBathing()
    {
        // reenable navmeshobstacle
        GetComponent<NavMeshObstacle>().enabled = true;
        // call petstats' stopbathing func
        PetHelper.petStats.StopBathing();
        // hide water fill
        waterFill.SetActive(false);
        // remove stop bathing action and add bathing action
        homeActions["Bathe"] = Bathe;
        homeActions.Remove("Stop bathing");
        // make pet stop sitting and set active behaviour to default
        PetHelper.petAnimation.SetBoolParameter("IsSitting", false);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;

        inUse = false;
    }
}