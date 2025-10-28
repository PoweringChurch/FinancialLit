using UnityEngine;

public class Furniture : MonoBehaviour
{
    public string DisplayName;
    public float sellValue;
    public float buyValue;
    private int furnitureId;
    
    public int FurnitureID {
        get {
            return furnitureId;
        }
    }

}
