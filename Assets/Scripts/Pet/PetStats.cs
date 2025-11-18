using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
public class PetStats : MonoBehaviour
{
    public static PetStats Instance;
    public string PetName
    {
        get
        {
            return petName;
        }
    }
    [Header("Decay Rates")]
    [SerializeField] private float tirednessRate;
    [SerializeField] private float hungerRate;
    [SerializeField] private float boredomRate;
    [SerializeField] private float dirtinessRate;
    [SerializeField] private float sleepRecoveryRate;
    [SerializeField] private float entertainmentRecoveryRate;
    [SerializeField] private float hygieneRecoveryRate;
    private string petName = "Foobar";

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
    [SerializeField] private ParticleSystem stinkyParticles;
    [SerializeField] private ParticleSystem batheParticles;
    void Awake()
    {
        Instance = this;
    }
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
    public void SetName(string newPetName)
    {
        petName = newPetName;
    }
    void Step()
    {
        float recoveryMultiplier = PetFlagManager.HasFlag(PetFlag.Sick) ? 0.5f : 1f;
        float drainMultiplier = 1.3f;
        if (PetFlagManager.HasFlag(PetFlag.Content)) drainMultiplier *= 0.9f;
        if (PetFlagManager.HasFlag(PetFlag.Loved)) drainMultiplier *= 0.95f;

        status["energy"] = Math.Max(0, status["energy"] - tirednessRate * drainMultiplier);
        if (PetStateMachine.IsInState(PetState.Sleeping))
        {
            float sleepBonus = PetFlagManager.HasFlag(PetFlag.WornOut) ? 1.15f : 1f;
            status["energy"] = Math.Clamp(status["energy"] + (sleepRecoveryRate + tirednessRate) * recoveryMultiplier * sleepBonus, 0, 1);
        }

        status["entertainment"] = Math.Max(0, status["entertainment"] - boredomRate * drainMultiplier);
        if (PetStateMachine.IsInState(PetState.Playing) || atPark)
        {
            float playBonus = PetFlagManager.HasFlag(PetFlag.Playful) ? 1.1f : 1f;
            float parkBonus = atPark ? 1.05f : 1f;
            status["entertainment"] = Math.Clamp(status["entertainment"] + (entertainmentRecoveryRate + boredomRate) * recoveryMultiplier * playBonus*parkBonus, 0, 1);
        }

        status["hygiene"] = Math.Max(0, status["hygiene"] - dirtinessRate * drainMultiplier);
        if (PetStateMachine.IsInState(PetState.Bathing))
            status["hygiene"] = Math.Clamp(status["hygiene"] + (dirtinessRate + hygieneRecoveryRate), 0, 1);

        status["hunger"] = Math.Max(0, status["hunger"] - hungerRate * drainMultiplier);
        // Check for Content flag
        if (status["energy"] > 0.7f && status["entertainment"] > 0.7f && status["hygiene"] > 0.7f && status["hunger"] > 0.7f)
        {
            if (!PetFlagManager.HasFlag(PetFlag.Content))
                PetFlagManager.AddFlag(PetFlag.Content);
        }
        else
            PetFlagManager.RemoveFlag(PetFlag.Content);
        // Check for Playful flag
        if (status["energy"] > 0.6f && status["entertainment"] > 0.6f && UnityEngine.Random.Range(0f, 1f) < 0.004f)
        {
            playfulTickTimer = 30;
            if (!PetFlagManager.HasFlag(PetFlag.Playful))
                PetFlagManager.AddFlag(PetFlag.Playful);
        }

        // Check for Loved flag
        if (status["hunger"] > 0.6f && status["hygiene"] > 0.6f && UnityEngine.Random.Range(0f, 1f) < 0.004f)
        {
            lovedTickTimer = 40;
            if (!PetFlagManager.HasFlag(PetFlag.Loved))
                PetFlagManager.AddFlag(PetFlag.Loved);
        }

        //check stinky
        var emission = stinkyParticles.emission;
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
        && !PetFlagManager.HasFlag(PetFlag.Sick) 
        && !PetFlagManager.HasFlag(PetFlag.Immune))
        {
            PetFlagManager.AddFlag(PetFlag.Sick);
            PetAnimation.Instance.SetBoolParameter("IsSick", true);
            UIHandler.Instance.PopupManager.PopupInfo(
                "Oh no",
                "Your pet is sick! Recovery from eating, playing, and sleeping is halved. Visit the vet!");
        }  
        if (atPark)
        {
            atParkWornOutTimer--;
            if (atParkWornOutTimer <= 0)
            {
                PetFlagManager.AddFlag(PetFlag.WornOut);
                wornOutTickTimer = 50;
            }
        }
        else atParkWornOutTimer = 40;

        wornOutTickTimer--;
        if (wornOutTickTimer <= 0 && PetFlagManager.HasFlag(PetFlag.WornOut))
            PetFlagManager.RemoveFlag(PetFlag.WornOut);
        
        immuneTickTimer -= 1;
        if (immuneTickTimer <= 0 && PetFlagManager.HasFlag(PetFlag.Immune))
            PetFlagManager.RemoveFlag(PetFlag.Immune);
        
        playfulTickTimer -= 1;
        if (playfulTickTimer <= 0 && PetFlagManager.HasFlag(PetFlag.Playful))
            PetFlagManager.RemoveFlag(PetFlag.Playful);
        
        lovedTickTimer -= 1;
        if (lovedTickTimer <= 0 && PetFlagManager.HasFlag(PetFlag.Loved))
            PetFlagManager.RemoveFlag(PetFlag.Loved);
    }
    public void StartBathing()
    {
        PetStateMachine.SetState(PetState.Bathing);
        var emission = batheParticles.emission;
        emission.enabled = true;
    }
    public void StopBathing()
    {
        PetStateMachine.SetState(PetState.Idle);
        var emission = batheParticles.emission;
        emission.enabled = false;
    }
    public void FeedPet(float amount)
    {
        if (PetFlagManager.HasFlag(PetFlag.Sick)) amount /= 2;
        status["hunger"] = Math.Min(1, status["hunger"] + amount);
    }
    public void StartPlay()
    {
        PetStateMachine.SetState(PetState.Playing);
    }
    public void StopPlay()
    {
        PetStateMachine.SetState(PetState.Idle);
    }
    public void StartSleep()
    {
        PetStateMachine.SetState(PetState.Sleeping);
    }
    public void StopSleep()
    {
        PetStateMachine.SetState(PetState.Idle);
    }
    public void CurePet()
    {
        immuneTickTimer = 120;
        PetFlagManager.AddFlag(PetFlag.Immune);
        PetFlagManager.RemoveFlag(PetFlag.Sick);
        PetAnimation.Instance.SetBoolParameter("IsSick", false);
    }
}
