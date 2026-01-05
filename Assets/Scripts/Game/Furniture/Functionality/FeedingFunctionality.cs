using UnityEngine;

public class FeedingFunctionality : BaseFunctionality
{
    public bool filled = false;
    private ParticleSystem eatParticles;
    [SerializeField] protected Transform foodTransform;
    [SerializeField] protected float filledY;
    [SerializeField] protected float emptyY;
    protected override void Awake()
    {
        base.Awake();
        eatParticles = GetComponentInChildren<ParticleSystem>();
        homeActions["Go eat"] = GoEat;
        homeActions["Refill"] = Refill;
    }
    protected virtual void GoEat()
    {
        if (!filled)
        {
            Message("Not filled!");
            return;
        }
        if (DefaultChecks())
        {
            return;
        }
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
    }
    protected virtual void OnReached()
    {
        PetHelper.petMover.OnReachedGoal -= OnReached;
        //PetMover.Instance.petModel.position = PositionPetY() + transform.forward;
        PetHelper.petMover.petTransform.LookAt(PositionPetY());

        PetHelper.petAnimation.SetTrigger("Eat");
        PetHelper.petStats.FeedPet(0.4f);
        //wait for 0.5 sec
        Invoke(nameof(EatFood), 0.7f);
    }
    void EatFood()
    {
        eatParticles.Play();
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
        SetFilled(false);
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
        SetFilled(true);
        
    }
    public virtual void SetFilled(bool to)
    {
        filled = to;
        if (filled)
            foodTransform.localPosition = new Vector3(0, filledY, 0);
        else
            foodTransform.localPosition = new Vector3(0, emptyY, 0);
    }
}
