using UnityEngine;
using System;
[Serializable]
public class InventoryEntry
{
    public FurnitureData itemData;  // Reference to the definition
    public int count;
    
    public InventoryEntry(FurnitureData data, int initialCount = 0)
    {
        itemData = data;
        count = initialCount;
    }
}