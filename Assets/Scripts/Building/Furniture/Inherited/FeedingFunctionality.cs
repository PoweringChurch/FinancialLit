using UnityEngine;

public class FeedingFunctionality : BaseFunctionality
{
    protected bool filled = false;
    private ParticleSystem eatParticles;
    [SerializeField] protected Transform foodTransform;
    [SerializeField] protected float filledY;
    [SerializeField] protected float emptyY;
    protected override void Awake()
    {
        base.Awake();
        eatParticles = GetComponentInChildren<ParticleSystem>();
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
        PetBehaviour.Instance.activeBehaviour = Behaviour.Occupied;
        PetMover.Instance.OnReachedGoal += OnReached;
        PetMover.Instance.SetGoalPosition(PositionPetY());
    }
    protected virtual void OnReached()
    {
        PetMover.Instance.OnReachedGoal -= OnReached;
        //PetMover.Instance.petModel.position = PositionPetY() + transform.forward;
        PetMover.Instance.petModel.LookAt(PositionPetY());

        PetAnimation.Instance.SetTrigger("Eat");
        PetStats.Instance.FeedPet(0.4f);
        filled = false;
        //wait for 0.5 sec
        Invoke(nameof(LowerFood), 0.7f);
    }
    void LowerFood()
    {
        eatParticles.Play();
        foodTransform.localPosition = new Vector3(0, emptyY, 0);
        PetBehaviour.Instance.activeBehaviour = Behaviour.Default;
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
        foodTransform.localPosition = new Vector3(0, filledY, 0);
    }
}
