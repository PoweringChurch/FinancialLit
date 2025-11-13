using UnityEngine;
using System;
[Serializable]
public class InventoryEntry
{
    public string itemName;
    public int count;
    
    [NonSerialized]
    public FurnitureData data; // Runtime reference, rebuilt after loading
    
    public InventoryEntry(FurnitureData itemData, int amount)
    {
        itemName = itemData.itemName;
        data = itemData;
        count = amount;
    }
}