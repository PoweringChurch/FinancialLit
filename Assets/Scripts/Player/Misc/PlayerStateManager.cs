using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum PlayerState {Home, Shopping, Placement} //multiple statuses

public static class PlayerStateManager
{
    private static List<PlayerState> currentStates = new();
    public static List<PlayerState> CurrentStates => currentStates;
    public static void AddState(PlayerState state)
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
        AddState((PlayerState)stateInt);
    }
    
    public static void RemoveState(PlayerState state)
    {
        currentStates.Remove(state);
    }
    
    public static void RemoveState(int stateInt)
    {
        RemoveState((PlayerState)stateInt);
    }
    
    public static bool HasState(PlayerState state)
    {
        return currentStates.Contains(state);
    }
    
    public static bool HasState(int stateInt)
    {
        return HasState((PlayerState)stateInt);
    }
}
