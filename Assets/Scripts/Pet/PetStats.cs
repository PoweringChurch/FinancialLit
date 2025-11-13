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
    private float tickspeed = 1.2f; //tick once every tickspeed seconds

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

        status["energy"] = Math.Max(0, status["energy"] - tirednessRate);
        if (PetFlagManager.HasFlag(PetFlag.Sleeping))
            status["energy"] = Math.Clamp(status["energy"] + (sleepRecoveryRate + tirednessRate) * recoveryMultiplier, 0, 1);

        status["entertainment"] = Math.Max(0, status["entertainment"] - boredomRate);
        if (PetFlagManager.HasFlag(PetFlag.Playing))
            status["entertainment"] = Math.Clamp(status["entertainment"] + (entertainmentRecoveryRate + boredomRate) * recoveryMultiplier, 0, 1);
        
        status["hygiene"] = Math.Max(0, status["hygiene"] - dirtinessRate);
        if (PetFlagManager.HasFlag(PetFlag.Bathing))
            status["hygiene"] = Math.Clamp(status["hygiene"] + (dirtinessRate + hygieneRecoveryRate), 0, 1);
        
        status["hunger"] = Math.Max(0, status["hunger"] - hungerRate);

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
        if (0.7f < sickChance && hit < sickChance*0.01f && !PetFlagManager.HasFlag(PetFlag.Sick))
        {
            PetFlagManager.AddFlag(PetFlag.Sick);
            PetAnimation.Instance.SetBoolParameter("IsSick", true);
            UIHandler.Instance.PopupManager.PopupInfo(
                "Oh no!",
                "Your pet is sick! Recovery from eating, playing, and sleeping is halved. Visit the vet!");
        }
    }
    public void StartBathing()
    {
        PetFlagManager.AddFlag(PetFlag.Bathing);
        var emission = batheParticles.emission;
        emission.enabled = true;
    }
    public void StopBathing()
    {
        PetFlagManager.RemoveFlag(PetFlag.Bathing);
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
        PetFlagManager.AddFlag(PetFlag.Playing);
    }
    public void StopPlay()
    {
        PetFlagManager.RemoveFlag(PetFlag.Playing);
    }
    public void StartSleep()
    {
        PetFlagManager.AddFlag(PetFlag.Sleeping);
    }
    public void StopSleep()
    {
        PetFlagManager.RemoveFlag(PetFlag.Sleeping);
    }
    public void CurePet()
    {
        PetFlagManager.RemoveFlag(PetFlag.Sick);
        PetAnimation.Instance.SetBoolParameter("IsSick", false);
    }
}
