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

        foreach (KeyValuePair<string, float> entry in status)
        {
            Debug.Log($"Status: {entry.Key}, Value: {entry.Value}");
        }
    }
    // Update is called once per frame
    private float elapsed = 0;
    private float time = 0.1f;
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > time)
        {
            elapsed = 0;
            Step();
            Debug.Log("\n Stepped \n");
            foreach (KeyValuePair<string, float> entry in status)
            {
                Debug.Log($"Status: {entry.Key}, Value: {entry.Value}");
            }
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
}
