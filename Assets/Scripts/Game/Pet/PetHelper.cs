using System;
using UnityEngine;

public static class PetHelper
{
    static GameObject currentActivePet;
    public static GameObject CurrentActivePet
    {
        get
        {
            return currentActivePet;
        }
        set
        {
            OnPetLoad?.Invoke(value);
            currentActivePet = value;
            Debug.Log("Set current active pet");
            // set the variables
            if (currentActivePet != null)
            {
                petAnimation = currentActivePet.GetComponent<PetAnimation>();
                petBehaviour = currentActivePet.GetComponentInChildren<PetBehaviour>();
                petFunctionality = currentActivePet.GetComponent<PetFunctionality>();
                petMover = currentActivePet.GetComponent<PetMover>();
                petStateMachine = currentActivePet.GetComponent<PetStateMachine>();
                petFlagManager = currentActivePet.GetComponent<PetFlagManager>();
                petStats = currentActivePet.GetComponent<PetStats>();
                if (petAnimation == null) Debug.LogWarning("No PetAnimation component found on current active pet");
                if (petBehaviour == null) Debug.LogWarning("No PetBehaviour component found on current active pet");
                if (petFunctionality == null) Debug.LogWarning("No PetFunctionality component found on current active pet");
                if (petMover == null) Debug.LogWarning("No PetMover component found on current active pet");
                if (petStateMachine == null) Debug.LogWarning("No PetStateMachine component found on current active pet");
                if (petFlagManager == null) Debug.LogWarning("No PetFlagManager component found on current active pet");
                if (petStats == null) Debug.LogWarning("No PetStats component found on current active pet");
            }
            else
            {
                Debug.LogWarning("Trying to set pet components, but currentActivePet is null");
            }
        }
    }
    
    // for easy access
    public static event Action<GameObject> OnPetLoad;

    public static PetAnimation petAnimation;
    public static PetBehaviour petBehaviour;
    public static PetFunctionality petFunctionality;
    public static PetMover petMover;
    public static PetStateMachine petStateMachine;
    public static PetFlagManager petFlagManager;
    public static PetStats petStats;
}