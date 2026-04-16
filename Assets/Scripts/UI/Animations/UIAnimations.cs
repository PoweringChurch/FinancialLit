using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

// base class of all ui animation classes
public class UIAnimation : MonoBehaviour
{
    protected float inDuration = 0.2f;
    protected float outDuration = 0.2f;
    public void Awake()
    {
        print("awake");
    }
    public void Update()
    {
    }
}