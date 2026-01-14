using UnityEngine;

//only used to make make handling animation easier later in the future
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
