using UnityEngine;

[CreateAssetMenu(fileName = "NewFurniture", menuName = "Inventory/Furniture")]
[System.Serializable]
public class FurnitureData : ScriptableObject
{
    public string itemName;
    public GameObject prefab;  // The furniture prefab
    public Sprite icon;
}