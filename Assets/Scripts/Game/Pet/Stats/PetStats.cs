using UnityEngine;
using System;
using System.Collections.Generic;
public class PetStats : MonoBehaviour
{
    // just as a placeholder so i know something is wrong
    public string petName = "unset";
    public PetBreed breed = PetBreed.Corgi;
    
    // consts
    const float tirednessRate = 0.001f;
    const float hungerRate = 0.002f;
    const float boredomRate = 0.0024f;
    const float dirtinessRate = 0.0013f;
    const float sleepRecoveryRate = 0.012f;
    const float entertainmentRecoveryRate = 0.018f;
    const float hygieneRecoveryRate = 0.015f;


    // vars
    private int immuneTickTimer = 0;
    private int playfulTickTimer = 0;
    private int lovedTickTimer = 0;
    private int wornOutTickTimer = 0;
    private int atParkWornOutTimer = 40; // might switch to be consistent

    public bool atPark = false;

    private Dictionary<string, float> status = new()
    {
        ["hygiene"] = 0.8f,
        ["entertainment"] = 0.8f,
        ["hunger"] = 0.8f,
        ["energy"] = 0.8f
    };
    // so that nothing can modify it externally, just in case
    public Dictionary<string, float> Status => status;

    [SerializeField] private ParticleSystem StinkyParticles;
    [SerializeField] private ParticleSystem BatheParticles;

    // for ticking the game
    private float elapsed = 0;
    private const float tickspeed = 1.2f; // tick once every tickspeed seconds, only written like this to be more verbose

    void Update()
    {
        // tick
        elapsed += Time.deltaTime;
        if (elapsed > tickspeed)
        {
            elapsed = 0;
            Tick();
        }
    }
    // called whenever the game ticks
    void Tick()
    {
        // ensure the player is in the game before ticking
        if (!CameraHandler.Instance.GameCamEnabled()) return;
        // if the player has the sick flag, reduce stat recovery by half
        float recoveryMultiplier = PetHelper.petFlagManager.HasFlag(PetFlag.Sick) ? 0.5f : 1f;

        // set to 1.6f just to make the game more active, might reduce, might increase
        float drainMultiplier = 1.6f;

        // adjust multiplier in regard to pet flags
        if (PetHelper.petFlagManager.HasFlag(PetFlag.Content)) drainMultiplier *= 0.9f;
        if (PetHelper.petFlagManager.HasFlag(PetFlag.Loved)) drainMultiplier *= 0.95f;

        // lower energy
        status["energy"] = Math.Max(0, status["energy"] - tirednessRate * drainMultiplier);
        // check if the pet is sleeping
        if (PetHelper.petStateMachine.IsInState(PetState.Sleeping))
        {
            // if the player has the worn out flag, add 15% more recovery for sleeping
            float sleepBonus = PetHelper.petFlagManager.HasFlag(PetFlag.WornOut) ? 1.15f : 1f;
            status["energy"] = Math.Clamp(status["energy"] 
            + (sleepRecoveryRate + (tirednessRate*drainMultiplier)) // account for lowered energy + add sleep recovery
            * recoveryMultiplier * sleepBonus, 0, 1); // apply mults
        }
        // lower entertainment
        status["entertainment"] = Math.Max(0, status["entertainment"] - boredomRate * drainMultiplier);
        // check if the pet is playing
        if (PetHelper.petStateMachine.IsInState(PetState.Playing) || atPark)
        {
            // if the player has the playful bonus, add 10% more entertainment recovery
            float playBonus = PetHelper.petFlagManager.HasFlag(PetFlag.Playful) ? 1.1f : 1f;
            // if the player is at the park, add 10% more entertainment recovery
            float parkBonus = atPark ? 1.1f : 1f;
            status["entertainment"] = Math.Clamp(status["entertainment"] 
            + (entertainmentRecoveryRate + (boredomRate * drainMultiplier)) // account for lowered entertainment + add entertainment recovery
            * recoveryMultiplier * playBonus*parkBonus, 0, 1); // apply mults
        }

        // lower hygiene
        status["hygiene"] = Math.Max(0, status["hygiene"] - dirtinessRate * drainMultiplier);
        // check if the pet is bathing
        if (PetHelper.petStateMachine.IsInState(PetState.Bathing))
            status["hygiene"] = Math.Clamp(status["hygiene"] 
            + (hygieneRecoveryRate + (drainMultiplier*dirtinessRate) ), 0, 1); // account for lowered hygiene + add hygiene recovery

        status["hunger"] = Math.Max(0, status["hunger"] - hungerRate * drainMultiplier);

        // check if the player can get the content flag
        if (status["energy"] > 0.7f && status["entertainment"] > 0.7f && status["hygiene"] > 0.7f && status["hunger"] > 0.7f)
        {
            if (!PetHelper.petFlagManager.HasFlag(PetFlag.Content))
                PetHelper.petFlagManager.AddFlag(PetFlag.Content);
        }
        else
            PetHelper.petFlagManager.RemoveFlag(PetFlag.Content);
        // try get loved flag if requirements fulfilled and pet doesnt already have it
        if (status["energy"] > 0.6f && status["entertainment"] > 0.6f && UnityEngine.Random.Range(0f, 1f) < 0.004f)
        {
            playfulTickTimer = 30;
            if (!PetHelper.petFlagManager.HasFlag(PetFlag.Playful))
                PetHelper.petFlagManager.AddFlag(PetFlag.Playful);
        }

        // try get loved flag if requirements fulfilled and pet doesnt already have it
        if (status["hunger"] > 0.6f && status["hygiene"] > 0.6f && UnityEngine.Random.Range(0f, 1f) < 0.004f)
        {
            lovedTickTimer = 40;
            if (!PetHelper.petFlagManager.HasFlag(PetFlag.Loved))
                PetHelper.petFlagManager.AddFlag(PetFlag.Loved);
        }

        // check stinky, add visuals
        var emission = StinkyParticles.emission;
        emission.enabled = status["hygiene"] < 0.5f;
        
        // calculation sickness chance contributions
        // the mults to the right add up to 1, higher values means that the stat contributes more
        float hungerContribution = (1 - status["hunger"]) * 0.4f;
        float energyContribution = (1 - status["energy"]) * 0.3f;
        float hygieneContribution = (1 - status["hygiene"]) * 0.2f;
        float entertainmentContribution = (1 - status["entertainment"]) * 0.1f;

        // try get sick
        float sickChance = hungerContribution + energyContribution + hygieneContribution + entertainmentContribution;
        if (0.7f < sickChance // if the sick chance if reasonably high
        && UnityEngine.Random.Range(0, 1f) < sickChance*0.008f  // and the sick chance hits
        && !PetHelper.petFlagManager.HasFlag(PetFlag.Sick) // and the pet isnt already sick
        && !PetHelper.petFlagManager.HasFlag(PetFlag.Immune)) // and the pet isnt immune
        {
            PetHelper.petFlagManager.AddFlag(PetFlag.Sick); // get sick
            PetHelper.petAnimation.SetBoolParameter("IsSick", true);
            // popup the notification that the pet is sick
            UIPopups.Instance.PopupInfo(
                "Oh no",
                "Your pet is sick! Recovery from eating, playing, and sleeping is halved. Visit the vet!");
        }  
        // try get pet worn out
        if (atPark)
        {
            atParkWornOutTimer--;
            if (atParkWornOutTimer <= 0)
            {
                PetHelper.petFlagManager.AddFlag(PetFlag.WornOut);
                wornOutTickTimer = 50;
            }
        }
        else atParkWornOutTimer = 40;
        
        // make worn out status expire
        wornOutTickTimer--;
        if (wornOutTickTimer <= 0 && PetHelper.petFlagManager.HasFlag(PetFlag.WornOut))
            PetHelper.petFlagManager.RemoveFlag(PetFlag.WornOut);
        // make immune status expire
        immuneTickTimer--;
        if (immuneTickTimer <= 0 && PetHelper.petFlagManager.HasFlag(PetFlag.Immune))
            PetHelper.petFlagManager.RemoveFlag(PetFlag.Immune);
        // make playful status expire
        playfulTickTimer--;
        if (playfulTickTimer <= 0 && PetHelper.petFlagManager.HasFlag(PetFlag.Playful))
            PetHelper.petFlagManager.RemoveFlag(PetFlag.Playful);
        // make loved status expire
        lovedTickTimer--;
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
        status["hunger"] = Math.Min(1, status["hunger"] + amount);
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
