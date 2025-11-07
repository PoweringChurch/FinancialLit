using UnityEngine;

public class InventoryHelper : MonoBehaviour
{
    public static InventoryHelper Instance { get; private set; }
    
    public bool debugMode = false;    
    private Inventory inventory = new Inventory();
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        if (debugMode)
        {
            FurnitureData[] allItems = FurnitureDatabase.GetAllItems();
            foreach (FurnitureData data in allItems)
            {
                inventory.AddItem(data, 1000);
            }
        }
        UIHandler.Instance.InventoryManager.SetInventory(inventory);
    }
    // Helpers
    public void AddItem(FurnitureData itemData, int count)
    {
        inventory.AddItem(itemData, count);
        UIHandler.Instance.InventoryManager.UpdateInventoryUI();
    }

    public void RemoveItem(string itemName, int count)
    {
        inventory.RemoveItem(itemName, count);
        UIHandler.Instance.InventoryManager.UpdateInventoryItem(itemName);
    }

    public Inventory GetInventory() => inventory;
}