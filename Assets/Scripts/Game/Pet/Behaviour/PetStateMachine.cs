using System;
public enum PetState {Idle, Sitting, Sleeping, Playing, Bathing }
public static class PetStateMachine
{
    private static PetState currentState = PetState.Sitting;
    public static PetState CurrentState => currentState;
    
    public static event Action<PetState, PetState> OnStateChanged;
    
    public static void SetState(PetState newState)
    {
        if (currentState == newState) return;
        
        PetState oldState = currentState;
        currentState = newState;
        OnStateChanged?.Invoke(oldState, newState);
    }
    public static bool IsInState(PetState state) => currentState == state;
}