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
    // helpers
    public void AddItem(FurnitureData itemData, int count)
    {
        inventory.AddItem(itemData, count);
        UIInventory.Instance.UpdateInventoryItem(itemData.itemName);
    }

    public void RemoveItem(string itemName, int count)
    {
        inventory.RemoveItem(itemName, count);
        UIInventory.Instance.UpdateInventoryItem(itemName);
    }
    public void SetInventory(Inventory newInventory)
    {
        inventory = newInventory;
        inventory.Initialize();
        UIInventory.Instance.SetInventory(inventory);
        UIOverlay.Instance.UpdateResourcesAndBal();
    }
    public void Rebuild()
    {
        inventory.Initialize();
    }
    public Inventory GetInventory() => inventory;
}