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
            Debug.Log("Set current active pet");
        }
    }

    public static PetAnimation petAnimation;
    public static PetBehaviour petBehaviour;
    public static PetFunctionality petFunctionality;
    public static PetMover petMover;
    public static PetStateMachine petStateMachine;

    public static PetFlagManager petFlagManager;
    public static PetStats petStats;
}