using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum PetFlag {Sitting, Sleeping, Playing, Stinky, Sick, Bathing}
public static class PetFlagManager
{
    private static List<PetFlag> currentFlags = new();
    public static List<PetFlag> CurrentFlags => currentFlags;
    public static void AddFlag(PetFlag state)
    {
        if (!currentFlags.Contains(state))
        {
            currentFlags.Add(state);
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
        currentFlags.Remove(state);
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
    }
    public static void SetFlags(List<PetFlag> petFlags)
    {
        currentFlags = petFlags;
    }
}
