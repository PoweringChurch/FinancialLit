using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
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

        inventory.AddItem(ItemDatabase.GetItem("Desk"), 10);
        inventory.AddItem(ItemDatabase.GetItem("Toilet"), 10);
        inventory.AddItem(ItemDatabase.GetItem("FoodBowl"), 10);
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