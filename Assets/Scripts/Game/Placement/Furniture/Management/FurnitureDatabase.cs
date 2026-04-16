using UnityEngine;
using System.Collections.Generic;

public class FurnitureDatabase : MonoBehaviour
{
    [SerializeField] private FurnitureData[] allFurniture;
    
    private static Dictionary<string, FurnitureData> itemLookup;
    private static FurnitureDatabase instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeDatabase();
        }
        else
            Destroy(gameObject); //if theres more than one instance of database then destroy
    }
    // initalized the database, constructing the item lookup
    private void InitializeDatabase()
    {
        itemLookup = new Dictionary<string, FurnitureData>(); 
        foreach (var item in allFurniture)
        {
            if (item != null)
                itemLookup[item.itemName] = item;
        }
        Debug.Log($"Item database initialized with {itemLookup.Count} items");
    }
    
    // returns data from item name
    public static FurnitureData GetData(string itemName)
    {
        if (itemLookup != null && itemLookup.TryGetValue(itemName, out var item))
            return item;
        Debug.LogWarning($"Item {itemName} not found in database");
        return null;
    }
    
    // returns array of all furniture data
    public static FurnitureData[] GetAllData()
    {
        // really not a needed check, but here for safety; might remove to prevent any confusion
        return instance != null ? instance.allFurniture : new FurnitureData[0];
    }
}