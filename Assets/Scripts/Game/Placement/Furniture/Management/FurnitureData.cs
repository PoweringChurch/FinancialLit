using UnityEngine;

[CreateAssetMenu(fileName = "NewFurniture", menuName = "Inventory/Furniture")]
[System.Serializable]
public class FurnitureData : ScriptableObject
{
    public string itemName;
    public GameObject prefab;  // the furniture prefab
    public Sprite icon;
}