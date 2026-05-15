using System;
using UnityEngine;
public enum PetState {Idle, Sitting, Sleeping, Playing, Bathing }
public class PetStateMachine : MonoBehaviour
{
    private PetState currentState = PetState.Sitting;
    public  PetState CurrentState 
    {
        get => currentState;
        set 
        {
            if (currentState == value) return;
            OnStateChanged?.Invoke(currentState, value);
            currentState = value;
        }
    }
    public  event Action<PetState, PetState> OnStateChanged;
}