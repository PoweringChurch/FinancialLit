using UnityEngine;
using System;
using System.Collections.Generic;
public class Pet : MonoBehaviour
{
    public static Pet Instance;
    public string PetName
    {
        get
        {
            return petName;
        }
    }
    [Header("Decay Rates")]
    [SerializeField] private float tirednessRate = 0.001f;
    [SerializeField] private float hungerRate = 0.002f;
    [SerializeField] private float boredomRate = 0.0025f;
    [SerializeField] private float dirtinessRate = 0.0015f;
    [SerializeField] private float sleepRecoveryRate = 0.001f;
    private string petName = "";
    private bool sleeping = false;
    private Dictionary<string, float> status = new Dictionary<string, float>();
    public Dictionary<string, float> Status => status;
    void Awake()
    {
        Instance = this;
        //adjust so 0 = needs attention and 1 = satisfied
        status.Add("hygiene",1f);
        status.Add("entertainment",1f);
        status.Add("hunger",1f);
        status.Add("energy",1f);
    }
    private float elapsed = 0;
    private float time = 1f;
    void Update()
    {
        if (sleeping)
        {
            status["energy"] = Math.Max(0,status["energy"]+sleepRecoveryRate);
        }
        //tick stats
        elapsed += Time.deltaTime;
        if (elapsed > time)
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
        status["energy"] = Math.Max(0,status["energy"]-tirednessRate);
        status["hunger"] = Math.Max(0,status["hunger"]-hungerRate);
        status["entertainment"] = Math.Max(0, status["entertainment"] - boredomRate);
        status["hygiene"] = Math.Max(0, status["hygiene"] - dirtinessRate);
        bool dirty = status["hygiene"] < 0.2f;
        if (dirty)
        {
            //add chance for pet to get sick
        }
    }
    public void CleanPet(float amount)
    {
        status["hygiene"] = Math.Min(1,status["hygiene"]+amount);
    }
    public void PlayWithPet(float amount)
    {
        status["entertainment"] = Math.Min(1,status["entertainment"]+amount);
    }
    public void FeedPet(float amount)
    {
        status["hunger"] = Math.Min(1,status["hunger"]+amount);
    }
    public void StartSleep()
    {
        sleeping = true;
    }
    public void StopSleep()
    {
        sleeping = false;
    }
}
