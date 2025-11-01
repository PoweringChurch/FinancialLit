using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    private readonly Dictionary<string, InventoryEntry> inventoryEntries = new Dictionary<string, InventoryEntry>();
    
    public void AddItem(FurnitureData itemData, int amount)
    {
        if (itemData == null)
        {
            Debug.LogError("Cannot add null item to inventory");
            return;
        }
        
        if (inventoryEntries.ContainsKey(itemData.itemName))
        {
            inventoryEntries[itemData.itemName].count += amount;
        }
        else
        {
            inventoryEntries.Add(itemData.itemName, new InventoryEntry(itemData, amount));
        }
    }
    
    public void RemoveItem(string itemName, int amount)
    {
        if (inventoryEntries.ContainsKey(itemName))
        {
            InventoryEntry entry = inventoryEntries[itemName];
            entry.count -= amount;
            
            if (entry.count < 0)
            {
                entry.count = 0;
            }
        }
        else
        {
            Debug.LogWarning($"Item {itemName} not found in inventory.");
        }
    }
    
    public InventoryEntry GetEntry(string itemName)
    {
        if (inventoryEntries.ContainsKey(itemName))
        {
            return inventoryEntries[itemName];
        }
        return null;
    }
    
    public int GetItemCount(string itemName)
    {
        if (inventoryEntries.ContainsKey(itemName))
        {
            return inventoryEntries[itemName].count;
        }
        return 0;
    }
    
    public bool HasItem(string itemName, int amount = 1)
    {
        return GetItemCount(itemName) >= amount;
    }
    
    public List<InventoryEntry> GetItemsToDisplay()
    {
        return inventoryEntries.Values
            .Where(entry => entry.count > 0)
            .ToList();
    }
    
    public Dictionary<string, InventoryEntry> GetAllEntries()
    {
        return new Dictionary<string, InventoryEntry>(inventoryEntries);
    }
}