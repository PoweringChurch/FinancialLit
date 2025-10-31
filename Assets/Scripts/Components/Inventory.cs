using UnityEngine;
using System.Collections.Generic;
public class Inventory : MonoBehaviour
{
    private Dictionary<string, InventoryItem> inventoryItems = new Dictionary<string, InventoryItem>();
    public Dictionary<string, InventoryItem> InventoryItems
    {
        get
        {
            return inventoryItems;
        }
    }
    public void AddItem(InventoryItem item, int amount)
    {
        if (inventoryItems.ContainsKey(item.displayName))
        {
            // item already exists in the inventory, just increase the count
            inventoryItems[item.displayName].Count += amount;
        }
        else
        {
            // new item, add it to the inventory
            inventoryItems.Add(item.displayName, item);
            inventoryItems[item.displayName].Count = amount;
        }
    }
    public void RemoveItem(string itemName, int amount)
    {
        if (inventoryItems.ContainsKey(itemName))
        {
            InventoryItem item = inventoryItems[itemName];
            item.Count -= amount;

            // remove the item completely if the count is 0
            if (item.Count <= 0)
            {
                inventoryItems.Remove(itemName);
            }
        }
        else
        {
            Debug.LogWarning($"Item {itemName} not found in inventory.");
        }
    }

    // get an item from display name
    public InventoryItem GetItem(string itemName)
    {
        if (inventoryItems.ContainsKey(itemName))
        {
            return inventoryItems[itemName];
        }
        return null;  // return null if the item doesn't exist
    }
    // check if an item exists in the inventory
    public bool HasItem(string itemName)
    {
        return inventoryItems.ContainsKey(itemName);
    }
     // Get all items with a non-zero count
    public List<InventoryItem> GetInventoryItemsToDisplay()
    {
        List<InventoryItem> itemsToDisplay = new List<InventoryItem>();
        foreach (var item in inventoryItems)
        {
            if (item.Value.Count > 0)
            {
                itemsToDisplay.Add(item.Value);
            }
        }
        return itemsToDisplay;
    }
}
