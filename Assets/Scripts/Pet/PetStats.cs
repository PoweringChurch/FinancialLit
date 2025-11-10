using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private string petName = "Foobar";
    private Dictionary<string, float> status = new Dictionary<string, float>();
    public Dictionary<string, float> Status => status;
    void Awake()
    {
        Instance = this;
        //adjust so 0 = needs attention and 1 = satisfied
        status.Add("hygiene", 0.8f);
        status.Add("entertainment", 0.8f);
        status.Add("hunger", 0.8f);
        status.Add("energy", 0.8f);
    }
    private float elapsed = 0;
    private float tickspeed = 1f; //tick once every tickspeed seconds
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
        status["energy"] = Math.Max(0, status["energy"] - tirednessRate);
        if (PetStateManager.HasState(PetState.Sleeping))
            status["energy"] = Math.Max(0, status["energy"] + sleepRecoveryRate + tirednessRate); //+rate so that i dont have to consider it
        
        status["entertainment"] = Math.Max(0, status["entertainment"] - boredomRate);
        if (PetStateManager.HasState(PetState.Playing))
            status["entertainment"] = Math.Max(0, status["entertainment"] + entertainmentRecoveryRate + boredomRate);
        
        status["hunger"] = Math.Max(0, status["hunger"] - hungerRate);
        status["hygiene"] = Math.Max(0, status["hygiene"] - dirtinessRate);
    }
    public void CleanPet(float amount)
    {
        status["hygiene"] = Math.Min(1, status["hygiene"] + amount);
    }
    public void FeedPet(float amount)
    {
        status["hunger"] = Math.Min(1, status["hunger"] + amount);
    }
    public void StartPlay()
    {
        PetStateManager.AddState(PetState.Playing);
    }
    public void StopPlay()
    {
        PetStateManager.RemoveState(PetState.Playing);
    }
    public void StartSleep()
    {
        PetStateManager.AddState(PetState.Sleeping);
    }
    public void StopSleep()
    {
        PetStateManager.RemoveState(PetState.Sleeping);
    }
}
