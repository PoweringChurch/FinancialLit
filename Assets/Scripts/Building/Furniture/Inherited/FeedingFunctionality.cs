using UnityEngine;

public class FeedingFunctionality : BaseFunctionality
{
    protected bool filled = false;
    protected override void Awake()
    {
        base.Awake();
        homeActions["Feed Pet"] = FeedPet;
        homeActions["Refill Bowl"] = RefillBowl;
    }
    protected virtual void FeedPet()
    {
        if (!filled)
        {
            Message("Not filled!");
            return;
        }
        PetStats.Instance.FeedPet(0.2f);
        filled = false;
    }
    protected virtual void RefillBowl()
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
