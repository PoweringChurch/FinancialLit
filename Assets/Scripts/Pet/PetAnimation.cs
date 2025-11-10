using System.Collections.Generic;
using UnityEngine;

public class PetAnimation : MonoBehaviour
{
    public static PetAnimation Instance;
    [SerializeField] private Animator petAnimator;
    void Awake()
    {
        Instance = this;
    }
    public void SetBoolParameter(string paramName, bool to)
    {
        Debug.Log(paramName + " " + to);
        petAnimator.SetBool(paramName, to);
    }
    public void SetTrigger(string paramName)
    {
        Debug.Log(paramName);
        petAnimator.SetTrigger(paramName);
    }
}
