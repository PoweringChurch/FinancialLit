using UnityEngine;

[CreateAssetMenu(fileName = "NewFurniture", menuName = "Inventory/Furniture")]
[System.Serializable]
public class FurnitureData : ScriptableObject
{
    public string itemName;
    public GameObject prefab;  // the furniture prefab
    public Sprite icon;
    // really not a necessary function, as i removed most debugging feature involving this class. will keep for potential future use though
    public override string ToString()
    {
        return $"{itemName}, prefab {prefab.name}";
    }
}