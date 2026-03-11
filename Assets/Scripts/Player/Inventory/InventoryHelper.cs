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
        UIInventory.Instance.SetInventory(inventory);
    }
    // adds an item to the inventory, additionally updating ui
    public void AddItem(FurnitureData itemData, int count)
    {
        inventory.AddItem(itemData, count);
        UIInventory.Instance.UpdateInventoryItem(itemData.itemName);
    }
    // removes an item to the inventory, additionally updating ui
    public void RemoveItem(string itemName, int count)
    {
        inventory.RemoveItem(itemName, count);
        UIInventory.Instance.UpdateInventoryItem(itemName);
    }
    // sets the inventory to another inventory, additionally updating ui
    public void SetInventory(Inventory newInventory)
    {
        inventory = newInventory;
        inventory.Initialize();
        UIInventory.Instance.SetInventory(inventory);
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    // rebuilds the inventory
    public void Rebuild()
    {
        inventory.Initialize();
    }
    // passes through the inventory
    public Inventory GetInventory() => inventory;
}