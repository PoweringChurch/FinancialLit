using System.Collections.Generic;
using System;
using UnityEngine;

public enum PetFlag {Sick, Immune, Content, WornOut, Playful, Loved, _ALL} // all enum is only meant to be used in ClearFlags and SetFlags

//immune = gained after vet appointment, prevents sickness
//sick = chance to gain based on total stats, half recovery from everything, requires vet visit to fix
//content = all stats over .7%, stats drain 10% slower
//worn out = gained from spending 50 ticks at park, sleeping recovers 15% more energy
//playful = gained randomly (0.4% per tick) if energy and entertainment are both over 0.6, pet moves faster and entertainment gain is increased by 10%
//loved = gained randomly (0.4% per tick) if hunger and hygiene are both over 0.6, pet moves faster and stats drain 5% slower

public  class PetFlagManager : MonoBehaviour
{
    private  List<PetFlag> currentFlags = new();

    public  List<PetFlag> CurrentFlags => currentFlags;
    
    public  event Action<PetFlag> OnFlagChanged;
    
    public  void AddFlag(PetFlag state)
    {
        if (!currentFlags.Contains(state))
        {
            currentFlags.Add(state);
            OnFlagChanged?.Invoke(state);
        }
    }
    
    public  void AddFlag(int stateInt)
    {
        AddFlag((PetFlag)stateInt);
    }
    
    public  void RemoveFlag(PetFlag state)
    {
        if (currentFlags.Remove(state))
            OnFlagChanged?.Invoke(state);
    }
    
    public  void RemoveFlag(int stateInt)
    {
        RemoveFlag((PetFlag)stateInt);
    }
    
    public  bool HasFlag(PetFlag state)
    {
        return currentFlags.Contains(state);
    }
    public  bool HasFlag(int stateInt)
    {
        return HasFlag((PetFlag)stateInt);
    }
    public  void ClearFlags()
    {
        currentFlags.Clear();
        OnFlagChanged?.Invoke(PetFlag._ALL);
    }
    public  void SetFlags(List<PetFlag> petFlags)
    {
        currentFlags = petFlags;
        OnFlagChanged?.Invoke(PetFlag._ALL);
    }
}