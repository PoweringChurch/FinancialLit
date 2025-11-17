using UnityEngine;
using System.Collections.Generic;
using System;

public enum PetFlag {Sick, Immune, Content, WornOut, Playful, Loved}
//immune = gained after vet appointment, prevents sickness
//sick = chance to gain based on total stats, half recovery from everything, requires vet visit to fix
//content = all stats over .7%, stats drain 10% slower
//worn out = gained from spending 50 ticks at park (dont worry about implementing), sleeping recovers 15% more energy
//playful = gained randomly (0.4% per tick) if energy and entertainment are both over 0.6, pet moves faster (ill implement) and entertainment gain is increased by 10%
//loved = gained randomly (0.4% per tick) if hunger and hygiene are both over 0.6, pet moves faster and stats drain 5% slower
public static class PetFlagManager
{
    private static List<PetFlag> currentFlags = new();
    public static List<PetFlag> CurrentFlags => currentFlags;
    
    public static event Action OnFlagChanged;
    
    public static void AddFlag(PetFlag state)
    {
        if (!currentFlags.Contains(state))
        {
            currentFlags.Add(state);
            OnFlagChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Status {state} is already active");
        }
    }
    
    public static void AddFlag(int stateInt)
    {
        AddFlag((PetFlag)stateInt);
    }
    
    public static void RemoveFlag(PetFlag state)
    {
        if (currentFlags.Remove(state))
        {
            OnFlagChanged?.Invoke();
        }
    }
    
    public static void RemoveFlag(int stateInt)
    {
        RemoveFlag((PetFlag)stateInt);
    }
    
    public static bool HasFlag(PetFlag state)
    {
        return currentFlags.Contains(state);
    }
    public static bool HasFlag(int stateInt)
    {
        return HasFlag((PetFlag)stateInt);
    }
    public static void ClearFlags()
    {
        currentFlags.Clear();
        OnFlagChanged?.Invoke();
    }
    public static void SetFlags(List<PetFlag> petFlags)
    {
        currentFlags = petFlags;
        OnFlagChanged?.Invoke();
    }
}