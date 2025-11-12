using UnityEngine;

public class FurnitureSetup : MonoBehaviour
{
    [Header("Box Collider")]
    public Vector3 colliderSize = Vector3.one;
    public Vector3 colliderCenter = Vector3.zero;
    
    [Header("Outline")]
    public Color outlineColor = Color.sandyBrown;
    [Header("Functionality")]
    public bool hasFunctionality = true;
    public float price = 10f;
    public BaseFunctionality baseFunctionality;
}