using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class Inventory
{
    [SerializeField] private List<InventoryEntry> inventoryEntries = new();    
    [NonSerialized] private Dictionary<string, InventoryEntry> inventoryDict = new();
    
    // called this after deserialization to rebuild dictionary
    public void Initialize()
    {
        inventoryDict.Clear();
        foreach (var entry in inventoryEntries)
        {
            entry.data = FurnitureDatabase.GetData(entry.itemName);
            inventoryDict[entry.itemName] = entry;
        }
    }
    // adds an item to the inventory
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
    // removes an item from the inventory
    public void RemoveItem(string itemName, int amount)
    {
        if (inventoryDict.ContainsKey(itemName))
        {
            InventoryEntry entry = inventoryDict[itemName];
            entry.count -= amount;
            
            if (entry.count < 0)
                entry.count = 0;
        }
        else
            Debug.LogWarning($"Item {itemName} not found in inventory");
    }

    // adds an entry to the inventory
    public InventoryEntry GetEntry(string itemName)
    {
        if (inventoryDict.ContainsKey(itemName))
            return inventoryDict[itemName];
        return null;
    }
    // get the amount of the item that the user has
    public int GetItemCount(string itemName)
    {
        // check if the player even has the passed item
        if (inventoryDict.ContainsKey(itemName))
            return inventoryDict[itemName].count;
        return 0; // if not, return 0 (as the player has none)
    }
    // bool used to check if the player has a certain amount of an item; default amount is 1
    public bool HasItem(string itemName, int amount = 1)
    {
        return GetItemCount(itemName) >= amount;
    }
    // get the items to display in the inventory, uses linq functions
    public List<InventoryEntry> GetItemsToDisplay()
    {
        return inventoryEntries
            .Where(entry => entry.count > 0)
            .ToList();
    }
    // get all the entries that the user has
    public Dictionary<string, InventoryEntry> GetAllEntries()
    {
        return new Dictionary<string, InventoryEntry>(inventoryDict);
    }
}