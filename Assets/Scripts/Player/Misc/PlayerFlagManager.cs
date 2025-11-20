using System.Collections.Generic;

public enum PlayerFlag {Tutorial,Home, Shopping, Placement, SetFollow}

public static class PlayerFlagManager
{
    private static List<PlayerFlag> currentFlags = new();
    public static List<PlayerFlag> CurrentFlags => currentFlags;
    public static void AddFlag(PlayerFlag state)
    {
        if (!currentFlags.Contains(state))
        {
            currentFlags.Add(state);
        }
    }
    
    public static void AddFlag(int stateInt)
    {
        AddFlag((PlayerFlag)stateInt);
    }
    
    public static void RemoveFlag(PlayerFlag state)
    {
        currentFlags.Remove(state);
    }
    
    public static void RemoveFlag(int stateInt)
    {
        RemoveFlag((PlayerFlag)stateInt);
    }
    
    public static bool HasState(PlayerFlag state)
    {
        return currentFlags.Contains(state);
    }

    public static bool HasFlag(int stateInt)
    {
        return HasState((PlayerFlag)stateInt);
    }
    public static void ClearFlag()
    {
        currentFlags.Clear();
    }
}
