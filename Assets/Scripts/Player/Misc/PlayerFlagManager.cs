using System.Collections.Generic;

public enum PlayerState {Home, Shopping, Placement, SetFollow}

public static class PlayerFlagManager
{
    private static List<PlayerState> currentFlags = new();
    public static List<PlayerState> CurrentFlags => currentFlags;
    public static void AddFlag(PlayerState state)
    {
        if (!currentFlags.Contains(state))
        {
            currentFlags.Add(state);
        }
    }
    
    public static void AddFlag(int stateInt)
    {
        AddFlag((PlayerState)stateInt);
    }
    
    public static void RemoveFlag(PlayerState state)
    {
        currentFlags.Remove(state);
    }
    
    public static void RemoveFlag(int stateInt)
    {
        RemoveFlag((PlayerState)stateInt);
    }
    
    public static bool HasState(PlayerState state)
    {
        return currentFlags.Contains(state);
    }

    public static bool HasFlag(int stateInt)
    {
        return HasState((PlayerState)stateInt);
    }
    public static void ClearFlag()
    {
        currentFlags.Clear();
    }
}
