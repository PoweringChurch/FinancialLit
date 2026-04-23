using UnityEngine;

public class FeedingFunctionality : BaseFunctionality
{
    public bool filled = false;
    private ParticleSystem eatParticles;
    // display when a food bowl is filled
    [SerializeField] protected Transform foodTransform;
    [SerializeField] protected float filledY;
    [SerializeField] protected float emptyY;

    protected override void Awake()
    {
        base.Awake();
        eatParticles = GetComponentInChildren<ParticleSystem>();
        // set actions
        homeActions["Go eat"] = GoEat;
        homeActions["Refill"] = Refill;
    }
    // called when go eat action is pressed
    protected virtual void GoEat()
    {
        // check if food bowl is filled before letting pet eat
        if (!filled)
        {
            PlacementUtils.Message("Not filled!", transform.position);
            return;
        }
        if (DefaultChecks()) // tb deleted
            return;
        // make pet occupied
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Occupied;
        PetHelper.petMover.OnReachedGoal += OnReached;
        PetHelper.petMover.SetGoalPosition(PositionPetY());
    }
    // called when pet reaches food bowl
    protected virtual void OnReached()
    {
        // disconnect onreached
        PetHelper.petMover.OnReachedGoal -= OnReached;
        // make pet look at food
        PetHelper.petMover.petTransform.LookAt(PositionPetY());
        // play eat animation
        PetHelper.petAnimation.SetTrigger("Eat");
        // refill pet hunger by 30%
        PetHelper.petStats.FeedPet(25f);
        // matched with time of animation to eat
        Invoke(nameof(EatFood), 0.7f);
    }
    // play the eat particles and remove the visual cue, for use in onreached
    void EatFood()
    {
        eatParticles.Play();
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
        SetFilled(false);
    }
    // refill the bowl
    protected virtual void Refill()
    {
        // check if already filled
        if (filled)
        {
            PlacementUtils.Message("Already filled!", transform.position);
            return;
        }
        // check if the player has enough feed
        if (!PlayerResources.Instance.CanConsumeFood())
        {
            PlacementUtils.Message("No pet food!", transform.position);
            return;
        }
        PlayerResources.Instance.ConsumeFood();
        SetFilled(true);
    }
    // update the food visual
    public virtual void SetFilled(bool to)
    {
        filled = to;
        if (filled)
            foodTransform.localPosition = new Vector3(0, filledY, 0);
        else
            foodTransform.localPosition = new Vector3(0, emptyY, 0);
    }
}
