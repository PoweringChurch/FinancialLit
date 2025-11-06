using UnityEngine;

public class FeedingFunctionality : BaseFunctionality
{
    protected bool filled = false;
    protected override void Awake()
    {
        base.Awake();
        actions["Feed Pet"] = FeedPet;
        actions["Refill Bowl"] = RefillBowl;
    }
    protected virtual void FeedPet()
    {
        if (!filled)
        {
            Message("Not filled!");
            return;
        }
        Pet.Instance.FeedPet(0.2f);
        filled = false;
    }
    protected virtual void RefillBowl()
    {
        if (filled)
        {
            Message("Already filled!");
            return;
        }
        if (!PlayerData.Instance.CanConsumeFood())
        {
            Message("No pet food!");
            return;
        }
        PlayerData.Instance.ConsumeFood();
        filled = true;
    }
}
