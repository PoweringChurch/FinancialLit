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
        petAnimator.SetBool(paramName, to);
    }
    public void SetTrigger(string paramName)
    {
        petAnimator.SetTrigger(paramName);
    }
}
