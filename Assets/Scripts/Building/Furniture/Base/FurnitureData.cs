using UnityEngine;

[CreateAssetMenu(fileName = "NewFurniture", menuName = "Inventory/Furniture")]
public class FurnitureData : ScriptableObject
{
    public string itemName;
    public GameObject prefab;  // The furniture prefab
    public Sprite icon;
}