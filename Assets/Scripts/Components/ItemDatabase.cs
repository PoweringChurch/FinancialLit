using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField] private FurnitureData[] allFurniture;
    
    private static Dictionary<string, FurnitureData> itemLookup;
    private static ItemDatabase instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject); //if theres more than one instance of database then destroy
        }
    }
    
    private void InitializeDatabase()
    {
        itemLookup = new Dictionary<string, FurnitureData>();
        foreach (var item in allFurniture)
        {
            if (item != null)
            {
                itemLookup[item.itemName] = item;
            }
        }
        Debug.Log($"Item database initialized with {itemLookup.Count} items");

        //debugging
    }
    
    public static FurnitureData GetItem(string itemName)
    {
        if (itemLookup != null && itemLookup.TryGetValue(itemName, out var item))
        {
            return item;
        }
        Debug.LogWarning($"Item {itemName} not found in database");
        return null;
    }
    
    public static FurnitureData[] GetAllItems()
    {
        return instance != null ? instance.allFurniture : new FurnitureData[0];
    }
}