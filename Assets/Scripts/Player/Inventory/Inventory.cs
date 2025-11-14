using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class Inventory
{
    [SerializeField] private List<InventoryEntry> inventoryEntries = new();    
    [NonSerialized] private Dictionary<string, InventoryEntry> inventoryDict = new();
    
    // Call this after deserialization to rebuild the dictionary
    public void Initialize()
    {
        inventoryDict.Clear();
        foreach (var entry in inventoryEntries)
        {
            entry.data = FurnitureDatabase.GetData(entry.itemName);
            inventoryDict[entry.itemName] = entry;
        }
    }
    public void AddItem(FurnitureData itemData, int amount)
    {
        if (itemData == null)
        {
            Debug.LogError("Cannot add null item to inventory");
            return;
        }
        
        if (inventoryDict.ContainsKey(itemData.itemName))
        {
            inventoryDict[itemData.itemName].count += amount;
        }
        else
        {
            var newEntry = new InventoryEntry(itemData, amount);
            inventoryEntries.Add(newEntry);
            inventoryDict.Add(itemData.itemName, newEntry);
        }
    }
    
    public void RemoveItem(string itemName, int amount)
    {
        if (inventoryDict.ContainsKey(itemName))
        {
            InventoryEntry entry = inventoryDict[itemName];
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
        if (inventoryDict.ContainsKey(itemName))
        {
            return inventoryDict[itemName];
        }
        return null;
    }
    
    public int GetItemCount(string itemName)
    {
        if (inventoryDict.ContainsKey(itemName))
        {
            return inventoryDict[itemName].count;
        }
        return 0;
    }
    
    public bool HasItem(string itemName, int amount = 1)
    {
        return GetItemCount(itemName) >= amount;
    }
    
    public List<InventoryEntry> GetItemsToDisplay()
    {
        return inventoryEntries
            .Where(entry => entry.count > 0)
            .ToList();
    }
    
    public Dictionary<string, InventoryEntry> GetAllEntries()
    {
        return new Dictionary<string, InventoryEntry>(inventoryDict);
    }
}