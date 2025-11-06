using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum PlayerState {Menu, Home, View}; //only one state
public enum PlayerStatus {Shopping, Placement} //multiple statuses

public static class PlayerStates
{
    private static PlayerState currentState;
    private static List<PlayerStatus> currentStatuses = new();
    
    public static PlayerState CurrentState => currentState;
    public static List<PlayerStatus> CurrentStatuses => currentStatuses;
    
    public static void SetState(PlayerState state)
    {
        currentState = state;
    }
    
    // Accept int and convert to enum
    public static void SetState(int stateInt)
    {
        SetState((PlayerState)stateInt);
    }
    
    public static void AddStatus(PlayerStatus status)
    {
        if (!currentStatuses.Contains(status))
        {
            currentStatuses.Add(status);
        }
        else
        {
            Debug.LogWarning($"Status {status} is already active!");
        }
    }
    
    public static void AddStatus(int statusInt)
    {
        AddStatus((PlayerStatus)statusInt);
    }
    
    public static void RemoveStatus(PlayerStatus status)
    {
        currentStatuses.Remove(status);
    }
    
    public static void RemoveStatus(int statusInt)
    {
        RemoveStatus((PlayerStatus)statusInt);
    }
    
    public static bool HasStatus(PlayerStatus status)
    {
        return currentStatuses.Contains(status);
    }
    
    public static bool HasStatus(int statusInt)
    {
        return HasStatus((PlayerStatus)statusInt);
    }
}