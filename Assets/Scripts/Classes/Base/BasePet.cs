using UnityEngine;
using System;
using System.Collections.Generic;
public class BasePet : MonoBehaviour
{
    public string PetName {
        get {
            return petName;
        }
    }
    private string petName = "";
    private bool sleeping = false;
    private Dictionary<string, float> status = new Dictionary<string, float>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //0 = satiated 1 = attention needed
        status.Add("sickness",0f);
        status.Add("dirtiness",0f);
        status.Add("boredom",0f);
        status.Add("hunger",0f);
        status.Add("tiredness",0f);
    }
    // Update is called once per frame
    private float elapsed = 0;
    private float time = 0.1f;
    void Update()
    {
        if (sleeping)
        {
            status["tiredness"] = Math.Max(0,status["tiredness"]-0.001f);
        }
        //tick stats
        elapsed += Time.deltaTime;
        if (elapsed > time)
        {
            elapsed = 0;
            Step();
        }
    }
    void SetName(string newPetName)
    {
        petName = newPetName;
    }
    void Step()
    {
        status["tiredness"] = Math.Min(1,status["tiredness"]+0.001f);
        status["hunger"] = Math.Min(1,status["hunger"]+0.002f);
        status["boredom"] = Math.Min(1,status["boredom"]+0.003f);

        bool dirty = status["dirtiness"] > 0.8f;
        if (dirty)
        {
            status["sickness"] += 0.004f;
        }
    }
    void CleanPet()
    {
        status["dirtiness"] = Math.Max(0,status["dirtiness"]-0.1f);
    }
    void PlayWithPet()
    {
        status["boredom"] = Math.Max(0,status["boredom"]-0.15f);
    }
    void FeedPet(float hungerRecovery)
    {
        status["hunger"] = Math.Max(0,status["hunger"]-hungerRecovery);
    }
    void CurePet(float sicknessRecovery)
    {
        status["sickness"] = Math.Max(0,status["sickness"]-sicknessRecovery);
    }
    void StartSleep()
    {
        sleeping = true;
    }
    void StopSleep()
    {
        sleeping = false;
    }
}
