using System;
public enum PetState {Idle, Sitting, Sleeping, Playing, Bathing }
public  class PetStateMachine
{
    private  PetState currentState = PetState.Sitting;
    public  PetState CurrentState => currentState;
    
    public  event Action<PetState, PetState> OnStateChanged;
    
    public  void SetState(PetState newState)
    {
        if (currentState == newState) return;
        
        PetState oldState = currentState;
        currentState = newState;
        OnStateChanged?.Invoke(oldState, newState);
    }
    public  bool IsInState(PetState state) => currentState == state;
}