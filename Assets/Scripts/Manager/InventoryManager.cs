using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    public bool debugMode = false;
    [SerializeField] private InventoryUI inventoryUI;
    
    private Inventory inventory = new Inventory();
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        inventoryUI.SetInventory(inventory);
        
    }
    void Start()
    {
        if (debugMode)
        {
            FurnitureData[] allItems = ItemDatabase.GetAllItems(); 
            foreach (FurnitureData data in allItems)
            {
                inventory.AddItem(data, 1000);
            }
        }
    }

    // Convenience methods that update UI automatically
    public void AddItem(FurnitureData itemData, int count)
    {
        inventory.AddItem(itemData, count);
        inventoryUI.UpdateInventoryUI();
    }
    
    public void RemoveItem(string itemName, int count)
    {
        inventory.RemoveItem(itemName, count);
        inventoryUI.UpdateInventoryItem(itemName);
    }
    
    // Direct access to inventory for more complex operations
    public Inventory GetInventory() => inventory;
}