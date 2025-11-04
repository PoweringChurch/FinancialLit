using UnityEngine;
using System;
using System.Collections.Generic;
public class Pet : MonoBehaviour
{
    public static Pet Instance;
    public string PetName {
        get {
            return petName;
        }
    }
    private string petName = "";
    private bool sleeping = false;
    private Dictionary<string, float> status = new Dictionary<string, float>();
    void Awake()
    {
        Instance = this;
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
    public void SetName(string newPetName)
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
    public void CleanPet(float amount)
    {
        status["dirtiness"] = Math.Max(0,status["dirtiness"]-amount);
    }
    public void PlayWithPet(float amount)
    {
        status["boredom"] = Math.Max(0,status["boredom"]-amount);
    }
    public void FeedPet(float amount)
    {
        status["hunger"] = Math.Max(0,status["hunger"]-amount);
    }
    public void CurePet(float amount)
    {
        status["sickness"] = Math.Max(0,status["sickness"]-amount);
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
