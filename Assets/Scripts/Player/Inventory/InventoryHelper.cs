using UnityEngine;

public class InventoryHelper : MonoBehaviour
{
    public static InventoryHelper Instance { get; private set; }
    private Inventory inventory = new();
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
    public void SetInventory(Inventory newInventory)
    {
        inventory = newInventory;
        inventory.Initialize();
        UIHandler.Instance.InventoryManager.SetInventory(inventory);
        UIHandler.Instance.ItemUpdater.UpdateText();
    }
    public void Rebuild()
    {
        inventory.Initialize();
    }
    public Inventory GetInventory() => inventory;
}