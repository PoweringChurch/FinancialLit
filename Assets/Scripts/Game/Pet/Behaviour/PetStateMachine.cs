using System;
using UnityEngine;
public enum PetState {Idle, Sitting, Sleeping, Playing, Bathing }
public class PetStateMachine : MonoBehaviour
{
    private PetState currentState = PetState.Sitting;
    public  PetState CurrentState => currentState;
    
    public  event Action<PetState, PetState> OnStateChanged;
    
    // might remove and just make CurrentState a property
    public void SetState(PetState newState)
    {
        if (currentState == newState) return;
        
        PetState oldState = currentState;
        currentState = newState;
        OnStateChanged?.Invoke(oldState, newState);
    }
    // dont think this is ever used, might also remove
    public bool IsInState(PetState state) => currentState == state;
}