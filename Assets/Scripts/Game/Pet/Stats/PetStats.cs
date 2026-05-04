using UnityEngine;
using System;
using System.Collections.Generic;
public class PetStats : MonoBehaviour
{
    // just as a placeholder so i know something is wrong
    public string petName = "unset";
    public PetBreed breed = PetBreed.Corgi;
    
    // consts
    const float tirednessRate = 0.1f;
    const float hungerRate = 0.12f;
    const float boredomRate = 0.24f;
    const float dirtinessRate = 0.14f;

    const float sleepRecoveryRate = 1.2f;
    const float entertainmentRecoveryRate = 1.8f;
    const float hygieneRecoveryRate = 1.5f;
    // does not include hunger recovery, as hunger works a little different from "recovery based" stats

    // vars
    private int immuneTickTimer = 0;
    private int playfulTickTimer = 0;
    private int lovedTickTimer = 0;
    private int wornOutTickTimer = 0;
    private int atParkWornOutTimer = 40; // might switch to be consistent

    public bool atPark = false;

    private Dictionary<string, float> status = new()
    {
        ["hygiene"] = 80f,
        ["entertainment"] = 80f,
        ["hunger"] = 80f,
        ["energy"] = 80f
    };
    // so that nothing can modify it externally, just in case
    public Dictionary<string, float> Status => status;

    [SerializeField] private ParticleSystem StinkyParticles;
    [SerializeField] private ParticleSystem BatheParticles;
    // called whenever a minute passes
    public void Tick(int count = 1, bool atWork = false)
    {
        // if the player has the sick flag, reduce stat recovery by half
        float recoveryMultiplier = PetHelper.petFlagManager.HasFlag(PetFlag.Sick) ? 0.5f : 1f;

        float drainMultiplier = atWork ? 0.35f : 1f; //might reduce, might increase; when the right balance is found itll be fixed again
        
        // adjust multiplier in regard to pet flags
        if (PetHelper.petFlagManager.HasFlag(PetFlag.Content)) drainMultiplier *= 0.9f;
        if (PetHelper.petFlagManager.HasFlag(PetFlag.Loved)) drainMultiplier *= 0.95f;

        // lower energy
        status["energy"] = Math.Max(0, status["energy"] - (tirednessRate * drainMultiplier * count));
        // check if the pet is sleeping
        if (PetHelper.petStateMachine.IsInState(PetState.Sleeping))
        {
            // if the player has the worn out flag, add 15% more recovery for sleeping
            float sleepBonus = PetHelper.petFlagManager.HasFlag(PetFlag.WornOut) ? 1.15f : 1f;
            status["energy"] = Math.Clamp(status["energy"] 
            + ((sleepRecoveryRate + (tirednessRate*drainMultiplier)) * recoveryMultiplier * sleepBonus * count), 0, 100);
        }
        // lower entertainment
        status["entertainment"] = Math.Max(0, status["entertainment"] - (boredomRate * drainMultiplier * count));
        // check if the pet is playing
        if (PetHelper.petStateMachine.IsInState(PetState.Playing) || atPark)
        {
            // if the player has the playful bonus, add 10% more entertainment recovery
            float playBonus = PetHelper.petFlagManager.HasFlag(PetFlag.Playful) ? 1.1f : 1f;
            // if the player is at the park, add 10% more entertainment recovery
            float parkBonus = atPark ? 1.1f : 1f;
            status["entertainment"] = Math.Clamp(status["entertainment"] 
            + ((entertainmentRecoveryRate + (boredomRate * drainMultiplier)) * recoveryMultiplier * playBonus * parkBonus * count), 0, 100);
        }
        // lower hygiene
        float parkDirty = atPark ? 1.2f : 1f;
        status["hygiene"] = Math.Max(0, status["hygiene"] - (dirtinessRate * drainMultiplier * parkDirty * count));
        // check if the pet is bathing
        if (PetHelper.petStateMachine.IsInState(PetState.Bathing))
            status["hygiene"] = Math.Clamp(status["hygiene"] 
            + ((hygieneRecoveryRate + (drainMultiplier*dirtinessRate)) * count), 0, 100);

        status["hunger"] = Math.Max(0, status["hunger"] - (hungerRate * drainMultiplier * count));

        // check if the player can get the content flag
        if (status["energy"] > 70f && status["entertainment"] > 70f && status["hygiene"] > 70f && status["hunger"] > 70f)
        {
            if (!PetHelper.petFlagManager.HasFlag(PetFlag.Content))
                PetHelper.petFlagManager.AddFlag(PetFlag.Content);
        }
        else
            PetHelper.petFlagManager.RemoveFlag(PetFlag.Content);
        
        // try get playful flag if requirements fulfilled and pet doesnt already have it
        if (status["energy"] > 60f && status["entertainment"] > 60f && UnityEngine.Random.Range(0f, 1f) < (0.004f * count))
        {
            playfulTickTimer = 30;
            if (!PetHelper.petFlagManager.HasFlag(PetFlag.Playful))
                PetHelper.petFlagManager.AddFlag(PetFlag.Playful);
        }

        // try get loved flag if requirements fulfilled and pet doesnt already have it
        if (status["hunger"] > 60f && status["hygiene"] > 60f && UnityEngine.Random.Range(0f, 1f) < (0.004f * count))
        {
            lovedTickTimer = 40;
            if (!PetHelper.petFlagManager.HasFlag(PetFlag.Loved))
                PetHelper.petFlagManager.AddFlag(PetFlag.Loved);
        }

        // check stinky, add visuals
        var emission = StinkyParticles.emission;
        emission.enabled = status["hygiene"] < 50f;
        
        // calculation sickness chance contributions
        // the mults to the right add up to 1, higher values means that the stat contributes more
        float hungerContribution = (100 - status["hunger"]) / 100f * 0.4f;
        float energyContribution = (100 - status["energy"]) / 100f * 0.3f;
        float hygieneContribution = (100 - status["hygiene"]) / 100f * 0.2f;
        float entertainmentContribution = (100 - status["entertainment"]) / 100f * 0.1f;

        // try get sick
        float sickChance = hungerContribution + energyContribution + hygieneContribution + entertainmentContribution;
        if (0.7f < sickChance 
        && UnityEngine.Random.Range(0, 1f) < (sickChance * 0.008f * count)
        && !PetHelper.petFlagManager.HasFlag(PetFlag.Sick)
        && !PetHelper.petFlagManager.HasFlag(PetFlag.Immune))
        {
            PetHelper.petFlagManager.AddFlag(PetFlag.Sick);
            PetHelper.petAnimation.SetBoolParameter("IsSick", true);
            UIPopups.Instance.PopupInfo(
                "Oh no",
                "Your pet is sick! Recovery from eating, playing, and sleeping is halved. Visit the vet!");
        }  
        
        // try get pet worn out
        if (atPark)
        {
            atParkWornOutTimer -= count;
            if (atParkWornOutTimer <= 0)
            {
                PetHelper.petFlagManager.AddFlag(PetFlag.WornOut);
                wornOutTickTimer = 50;
            }
        }
        else atParkWornOutTimer = 40;
        
        // make worn out status expire
        wornOutTickTimer -= count;
        if (wornOutTickTimer <= 0 && PetHelper.petFlagManager.HasFlag(PetFlag.WornOut))
            PetHelper.petFlagManager.RemoveFlag(PetFlag.WornOut);
        // make immune status expire
        immuneTickTimer -= count;
        if (immuneTickTimer <= 0 && PetHelper.petFlagManager.HasFlag(PetFlag.Immune))
            PetHelper.petFlagManager.RemoveFlag(PetFlag.Immune);
        // make playful status expire
        playfulTickTimer -= count;
        if (playfulTickTimer <= 0 && PetHelper.petFlagManager.HasFlag(PetFlag.Playful))
            PetHelper.petFlagManager.RemoveFlag(PetFlag.Playful);
        // make loved status expire
        lovedTickTimer -= count;
        if (lovedTickTimer <= 0 && PetHelper.petFlagManager.HasFlag(PetFlag.Loved))
            PetHelper.petFlagManager.RemoveFlag(PetFlag.Loved);
    }
    // called when the pet stats bathing
    public void StartBathing()
    {
        PetHelper.petStateMachine.SetState(PetState.Bathing);
        var emission = BatheParticles.emission;
        emission.enabled = true;
    }
    // called when the pet stops bathing
    public void StopBathing()
    {
        PetHelper.petStateMachine.SetState(PetState.Idle);
        var emission = BatheParticles.emission;
        emission.enabled = false;
    }
    // called when the pet is fed
    public void FeedPet(float amount)
    {
        if (PetHelper.petFlagManager.HasFlag(PetFlag.Sick)) amount /= 2;
        status["hunger"] = Math.Min(100, status["hunger"] + amount);
    }
    // called when the pet starts playing. these next few functions are really only written like this so incase anything should change further down the line I can know whats being called and how
    public void StartPlay() { PetHelper.petStateMachine.SetState(PetState.Playing); }
    public void StopPlay() { PetHelper.petStateMachine.SetState(PetState.Idle); }
    public void StartSleep() { PetHelper.petStateMachine.SetState(PetState.Sleeping); }
    public void StopSleep() { PetHelper.petStateMachine.SetState(PetState.Idle); }

    // called when the pet is cured
    public void CurePet()
    {
        immuneTickTimer = 120;
        PetHelper.petFlagManager.AddFlag(PetFlag.Immune);
        PetHelper.petFlagManager.RemoveFlag(PetFlag.Sick);
        PetHelper.petAnimation.SetBoolParameter("IsSick", false);
    }
}
