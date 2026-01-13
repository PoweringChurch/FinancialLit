public class AppointingFunctionality : BaseFunctionality
{
    // fee of pet treatment
    float fee = 300;
    // the base value of the fee
    const float baseline = 300;
    protected override void Awake()
    {
        ignoreBase = true;
        base.Awake();
        globalActions["Talk to Vet"] = Schedule;
    }
    // schedule an appointment to the vet, curing pet sick status
    protected void Schedule()
    {
        // check if the pet is sick
        if (!PetHelper.petFlagManager.HasFlag(PetFlag.Sick))
        {
            print(string.Join(", ", PetHelper.petFlagManager.CurrentFlags));
            Message($"{PetHelper.petStats.petName} is not sick!");
            return;
        }
        // calculate fee
        var status = PetHelper.petStats.Status;
        float avgHealth = (status["hunger"] + status["energy"] + status["hygiene"] + status["entertainment"]) / 4f;
        fee = baseline * (1.5f - avgHealth * 0.5f); 
        // show popup
        string header = "Pet treatment";
        string body = $"Scheduling an appointment costs ${fee:N2}. You can reduce treatment costs by keeping your pet healthy. Do you want to proceed with the treatment?";
        UIPopups.Instance.PopupYN(header,body,OnYes,() => {},"Yes","No");
    }
    // occurs when the pop up informing user of fee is accepted
    protected void OnYes()
    {
        // check if player can afford fee
        if (!PlayerResources.Instance.CanAfford(fee))
        {
            UIPopups.Instance.PopupInfo("Cannot afford","You cannot afford an appointment.");
            return;
        }
        // spend money
        PlayerResources.Instance.Spend(fee, "Healthcare");
        PetHelper.petStats.CurePet();
        // inform player of cure
        Message("Pet cured!");   
    }
}