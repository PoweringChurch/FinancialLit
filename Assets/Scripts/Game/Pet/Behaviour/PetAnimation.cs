using UnityEngine;

public class PetAnimation : MonoBehaviour
{
    [SerializeField] private Animator petAnimator;
    public void SetBoolParameter(string paramName, bool to)
    {
        petAnimator.SetBool(paramName, to);
    }
    public void SetTrigger(string paramName)
    {
        petAnimator.SetTrigger(paramName);
    }
}
