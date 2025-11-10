using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum PetState {Sitting, Sleeping, Playing}
public static class PetStateManager
{
    private static List<PetState> currentStates = new();
    public static List<PetState> CurrentStates => currentStates;
    public static void AddState(PetState state)
    {
        if (!currentStates.Contains(state))
        {
            currentStates.Add(state);
        }
        else
        {
            Debug.LogWarning($"Status {state} is already active");
        }
    }
    
    public static void AddState(int stateInt)
    {
        AddState((PetState)stateInt);
    }
    
    public static void RemoveState(PetState state)
    {
        currentStates.Remove(state);
    }
    
    public static void RemoveState(int stateInt)
    {
        RemoveState((PetState)stateInt);
    }
    
    public static bool HasState(PetState state)
    {
        return currentStates.Contains(state);
    }

    public static bool HasState(int stateInt)
    {
        return HasState((PetState)stateInt);
    }
    public static void ClearStates()
    {
        currentStates.Clear();
    }
}
