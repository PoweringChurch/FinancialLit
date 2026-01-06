using UnityEngine;
using System;
using System.Collections.Generic;
public class PetStats : MonoBehaviour
{
    public PetFlagManager petFlagManager;
    public PetStateMachine petStateMachine;
    public PetAnimation petAnimation;
    public string petName = "unset";
    public PetBreed breed = PetBreed.Corgi;
    const float tirednessRate = 0.001f;
    const float hungerRate = 0.002f;
    const float boredomRate = 0.0024f;
    const float dirtinessRate = 0.0013f;
    const float sleepRecoveryRate = 0.012f;
    const float entertainmentRecoveryRate = 0.018f;
    const float hygieneRecoveryRate = 0.015f;
    public bool atPark = false;
    private int immuneTickTimer = 0;
    private int playfulTickTimer = 0;
    private int lovedTickTimer = 0;
    private int wornOutTickTimer = 0;
    private int atParkWornOutTimer = 40; //if at park for 40 ticks
    private Dictionary<string, float> status = new()
    {
        ["hygiene"] = 0.8f,
        ["entertainment"] = 0.8f,
        ["hunger"] = 0.8f,
        ["energy"] = 0.8f
    };
    public Dictionary<string, float> Status => status;
    [SerializeField] private ParticleSystem StinkyParticles;
    [SerializeField] private ParticleSystem BatheParticles;
    private float elapsed = 0;
    private readonly float tickspeed = 1.2f; //tick once every tickspeed seconds
    void Update()
    {
        //tick stats
        elapsed += Time.deltaTime;
        if (elapsed > tickspeed)
        {
            elapsed = 0;
            Step();
        }
    }
    void Step()
    {
        if (!CameraHandler.Instance.GameCamEnabled()) return;
        float recoveryMultiplier = petFlagManager.HasFlag(PetFlag.Sick) ? 0.5f : 1f;
        float drainMultiplier = 1.6f;
        if (petFlagManager.HasFlag(PetFlag.Content)) drainMultiplier *= 0.9f;
        if (petFlagManager.HasFlag(PetFlag.Loved)) drainMultiplier *= 0.95f;

        status["energy"] = Math.Max(0, status["energy"] - tirednessRate * drainMultiplier);
        if (petStateMachine.IsInState(PetState.Sleeping))
        {
            float sleepBonus = petFlagManager.HasFlag(PetFlag.WornOut) ? 1.15f : 1f;
            status["energy"] = Math.Clamp(status["energy"] + (sleepRecoveryRate + tirednessRate) * recoveryMultiplier * sleepBonus, 0, 1);
        }

        status["entertainment"] = Math.Max(0, status["entertainment"] - boredomRate * drainMultiplier);
        if (petStateMachine.IsInState(PetState.Playing) || atPark)
        {
            float playBonus = petFlagManager.HasFlag(PetFlag.Playful) ? 1.1f : 1f;
            float parkBonus = atPark ? 1.1f : 1f;
            status["entertainment"] = Math.Clamp(status["entertainment"] + (entertainmentRecoveryRate + boredomRate) * recoveryMultiplier * playBonus*parkBonus, 0, 1);
        }

        status["hygiene"] = Math.Max(0, status["hygiene"] - dirtinessRate * drainMultiplier);
        if (petStateMachine.IsInState(PetState.Bathing))
            status["hygiene"] = Math.Clamp(status["hygiene"] + (dirtinessRate + hygieneRecoveryRate), 0, 1);

        status["hunger"] = Math.Max(0, status["hunger"] - hungerRate * drainMultiplier);
        // Check for Content flag
        if (status["energy"] > 0.7f && status["entertainment"] > 0.7f && status["hygiene"] > 0.7f && status["hunger"] > 0.7f)
        {
            if (!petFlagManager.HasFlag(PetFlag.Content))
                petFlagManager.AddFlag(PetFlag.Content);
        }
        else
            petFlagManager.RemoveFlag(PetFlag.Content);
        // Check for playful flag
        if (status["energy"] > 0.6f && status["entertainment"] > 0.6f && UnityEngine.Random.Range(0f, 1f) < 0.004f)
        {
            playfulTickTimer = 30;
            if (!petFlagManager.HasFlag(PetFlag.Playful))
                petFlagManager.AddFlag(PetFlag.Playful);
        }

        // Check for loved flag
        if (status["hunger"] > 0.6f && status["hygiene"] > 0.6f && UnityEngine.Random.Range(0f, 1f) < 0.004f)
        {
            lovedTickTimer = 40;
            if (!petFlagManager.HasFlag(PetFlag.Loved))
                petFlagManager.AddFlag(PetFlag.Loved);
        }

        //check stinky
        var emission = StinkyParticles.emission;
        emission.enabled = status["hygiene"] < 0.5f;
        
        //try get sick
        float hungerContribution = (1 - status["hunger"]) * 0.4f;
        float energyContribution = (1 - status["energy"]) * 0.3f;
        float hygieneContribution = (1 - status["hygiene"]) * 0.2f;
        float entertainmentContribution = (1 - status["entertainment"]) * 0.1f;
        
        float sickChance = hungerContribution + energyContribution + hygieneContribution + entertainmentContribution;
        float hit = UnityEngine.Random.Range(0, 1f);
        if (0.7f < sickChance 
        && hit < sickChance*0.008f 
        && !petFlagManager.HasFlag(PetFlag.Sick) 
        && !petFlagManager.HasFlag(PetFlag.Immune))
        {
            petFlagManager.AddFlag(PetFlag.Sick);
            petAnimation.SetBoolParameter("IsSick", true);
            UIPopups.Instance.PopupInfo(
                "Oh no",
                "Your pet is sick! Recovery from eating, playing, and sleeping is halved. Visit the vet!");
        }  
        if (atPark)
        {
            atParkWornOutTimer--;
            if (atParkWornOutTimer <= 0)
            {
                petFlagManager.AddFlag(PetFlag.WornOut);
                wornOutTickTimer = 50;
            }
        }
        else atParkWornOutTimer = 40;

        wornOutTickTimer--;
        if (wornOutTickTimer <= 0 && petFlagManager.HasFlag(PetFlag.WornOut))
            petFlagManager.RemoveFlag(PetFlag.WornOut);
        
        immuneTickTimer -= 1;
        if (immuneTickTimer <= 0 && petFlagManager.HasFlag(PetFlag.Immune))
            petFlagManager.RemoveFlag(PetFlag.Immune);
        
        playfulTickTimer -= 1;
        if (playfulTickTimer <= 0 && petFlagManager.HasFlag(PetFlag.Playful))
            petFlagManager.RemoveFlag(PetFlag.Playful);
        
        lovedTickTimer -= 1;
        if (lovedTickTimer <= 0 && petFlagManager.HasFlag(PetFlag.Loved))
            petFlagManager.RemoveFlag(PetFlag.Loved);
    }
    public void StartBathing()
    {
        petStateMachine.SetState(PetState.Bathing);
        var emission = BatheParticles.emission;
        emission.enabled = true;
    }
    public void StopBathing()
    {
        petStateMachine.SetState(PetState.Idle);
        var emission = BatheParticles.emission;
        emission.enabled = false;
    }
    public void FeedPet(float amount)
    {
        if (petFlagManager.HasFlag(PetFlag.Sick)) amount /= 2;
        status["hunger"] = Math.Min(1, status["hunger"] + amount);
    }
    public void StartPlay()
    {
        petStateMachine.SetState(PetState.Playing);
    }
    public void StopPlay()
    {
        petStateMachine.SetState(PetState.Idle);
    }
    public void StartSleep()
    {
        petStateMachine.SetState(PetState.Sleeping);
    }
    public void StopSleep()
    {
        petStateMachine.SetState(PetState.Idle);
    }
    public void CurePet()
    {
        immuneTickTimer = 120;
        petFlagManager.AddFlag(PetFlag.Immune);
        petFlagManager.RemoveFlag(PetFlag.Sick);
        petAnimation.SetBoolParameter("IsSick", false);
    }
}
