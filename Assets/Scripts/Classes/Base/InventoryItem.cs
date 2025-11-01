using System;
using UnityEngine;
[Serializable]
public class InventoryItem
{
    public GameObject furnitureGameObject; // Reference to the object in the scene
    public string itemName;             // Display name of the item
    private int count;                     // Private backing field for count

    public int Count
    {
        get { return count; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Count cannot be negative");
            count = value;
        }
    }

    // Constructor to initialize inventory items
    public InventoryItem(string name, GameObject gameObject)
    {
        itemName = name;
        furnitureGameObject = gameObject;
        furnitureGameObject.GetComponent<PlacementHandler>().itemName = name;
    }
}