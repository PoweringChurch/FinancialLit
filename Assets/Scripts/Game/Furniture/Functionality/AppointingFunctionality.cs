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
        globalActions["Membership"] = Membership;
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
        float avgHealth = (status["hunger"] + status["energy"] + status["hygiene"] + status["entertainment"]) / 400f;
        fee = baseline * (1.5f - avgHealth * 0.5f); 
        // show popup
        string header = "Pet treatment";
        string body = $"Scheduling an appointment costs ${fee:N2}. You can reduce treatment costs by keeping your pet healthy. Do you want to proceed with the treatment?";
        UIPopups.Instance.PopupYN(header,body,OnScheduleYes,() => {},"Yes","No");
    }
    // occurs when the player presses the membership action
    protected void Membership()
    {
        string header = "Petsy Health Maxx";
        string body = $"Do you want to sign up for the Petsy Health Maxx? While this membership is active, scheduling an appointment will cost less. \n\n $40 Signup fee \n $25 Weekly";
        UIPopups.Instance.PopupYN(header,body,OnMembershipYes,() => {},"Yes","No");
    }
    // occurs when the pop up informing user of membership is accepted
    protected void OnMembershipYes()
    {
        Membership healthMax = new("Petsy Health Maxx", 25, 40);
        if (!FinancialSpending.Instance.CanAfford(healthMax.signupFee))
        {
            UIPopups.Instance.PopupInfo("Cannot afford","You cannot afford the sign-up fee of this membership.");
            return;
        }
        FinancialSpending.Instance.Spend(healthMax.signupFee, "Memberships");
        Memberships.Instance.AddMembership("healthMax", healthMax);
    }
    // occurs when the pop up informing user of fee is accepted
    protected void OnScheduleYes()
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