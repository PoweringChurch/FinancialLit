public class AppointingFunctionality : BaseFunctionality
{
    float fee = 300;
    float baseline = 300;
    protected override void Awake()
    {
        ignoreBase = true;
        base.Awake();
        globalActions["Talk to Vet"] = Schedule;
    }
    protected void Schedule()
    {
        if (!PetFlagManager.HasFlag(PetFlag.Sick))
        {
            print(string.Join(", ", PetFlagManager.CurrentFlags));
            Message($"{PetStats.Instance.PetName} is not sick!");
            return;
        }
        
        string header = "Pet treatment";

        var status = PetStats.Instance.Status;
        float avgHealth = (status["hunger"] + status["energy"] + status["hygiene"] + status["entertainment"]) / 4f;
        fee = baseline * (1.5f - avgHealth * 0.5f); 
        string body = $"Scheduling an appointment costs ${fee:N2}. You can reduce treatment costs by keeping your pet healthy. Do you want to proceed with the treatment?";
        UIHandler.Instance.PopupManager.PopupYN(header,body,OnYes,() => {},"Yes","No");
    }
    protected void OnYes()
    {
        if (!PlayerResources.Instance.CanAfford(fee))
        {
            UIHandler.Instance.PopupManager.PopupInfo("Cannot afford","You cannot afford an appointment.");
            return;
        }
        Message("Pet cured!");    
        PlayerResources.Instance.Spend(fee, "Healthcare");
        PetStats.Instance.CurePet();
    }
}