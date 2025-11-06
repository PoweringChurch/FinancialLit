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
    [SerializeField] private float tirednessRate;
    [SerializeField] private float hungerRate;
    [SerializeField] private float boredomRate;
    [SerializeField] private float dirtinessRate;
    [SerializeField] private float sleepRecoveryRate;

    private string petName = "Foobar";
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
    private float tickspeed = 1.5f; //tick once every tickspeed seconds
    void Update()
    {
        if (sleeping)
        {
            status["energy"] = Math.Max(0,status["energy"]+sleepRecoveryRate);
        }
        //tick stats
        elapsed += Time.deltaTime;
        if (elapsed > tickspeed)
        {
            elapsed = 0;
            Step();
            UIHandler.Instance.PetUI.UpdateUI();
        }
    }
    public void SetName(string newPetName)
    {
        petName = newPetName;
    }
    float calculateAffliction(float x, float w)
    {
        return (float)(Math.Max(0,Math.Log(-x+1)+0.1))*w;
    }
    void Step()
    {
        status["energy"] = Math.Max(0,status["energy"]-tirednessRate);
        status["hunger"] = Math.Max(0,status["hunger"]-hungerRate);
        status["entertainment"] = Math.Max(0, status["entertainment"] - boredomRate);
        status["hygiene"] = Math.Max(0, status["hygiene"] - dirtinessRate);

        var dirtyAffliction = calculateAffliction(status["hygiene"], 1.1f);
        var hungerAffliction = calculateAffliction(status["hunger"], 0.9f);
        var boredAffliction = calculateAffliction(status["entertainment"], 0.8f);
        var tiredAffliction = calculateAffliction(status["energy"], 1.2f);

        var total = dirtyAffliction + hungerAffliction + boredAffliction + tiredAffliction;
        if (total > 0.3f)
        {
            //get sick
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
