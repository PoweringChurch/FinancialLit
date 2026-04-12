using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FurnitureInventoryUI : MonoBehaviour
{
    public static FurnitureInventoryUI Instance;

    [SerializeField] private GameObject itemButtonTemplate;
    [SerializeField] private Transform contentTransform;

    private Inventory inventory;
    private readonly Dictionary<string, GameObject> inventoryItemUI = new();

    public void Awake()
    {
        Instance = this;
        if (itemButtonTemplate == null || contentTransform == null)
            Debug.LogError("InventoryManager components not fully linked in Inspector");
    }
    // set a new inventory
    public void SetInventory(Inventory newInventory)
    {
        inventory = newInventory;
    }
    // updates the inventorys ui
    public void UpdateInventoryUI()
    {
        // clear existing UI
        foreach (Transform child in contentTransform)
            Destroy(child.gameObject);
        inventoryItemUI.Clear();

        if (inventory == null)
            return;
        foreach (var entry in inventory.GetItemsToDisplay())
        {
            // shouldnt happen but better to check
            if (entry.data == null)
            {
                Debug.LogWarning("Entry item not found");
                continue;
            }
            // create a new inventory button based on the button template
            GameObject newTemplate = UnityEngine.Object.Instantiate(itemButtonTemplate, contentTransform);
            Button itemButton = newTemplate.GetComponent<Button>();
            // set count
            TextMeshProUGUI countText = newTemplate.transform.GetComponentInChildren<TextMeshProUGUI>();
            countText.text = $"{entry.count}";
            // set img
            Transform inner = newTemplate.transform.GetChild(0);
            var imgPreview = inner.GetChild(0).GetComponent<Image>();

            
            imgPreview.sprite = entry.data.icon;
            // add clicking functionality
            itemButton.onClick.AddListener(() => OnItemButtonClicked(entry));
            inventoryItemUI.Add(entry.itemName, newTemplate);
        }
    }
    // update the inventory item
    public void UpdateInventoryItem(string itemName)
    {
        if (inventory == null)
            return;

        InventoryEntry entry = inventory.GetEntry(itemName);

        if (entry == null || entry.count <= 0)
        {
            //if item is removed or count is zero, clean up the UI element
            if (inventoryItemUI.TryGetValue(itemName, out GameObject itemUI))
            {
                Destroy(itemUI);
                inventoryItemUI.Remove(itemName);
            }
            return;
        }

        // if the item UI doesn't exist but the item is in inventory, refresh the whole UI 
        if (!inventoryItemUI.TryGetValue(itemName, out GameObject existingItemUI))
        {
            UpdateInventoryUI();
            return;
        }

        // update the count text
        TextMeshProUGUI countText = existingItemUI.GetComponentInChildren<TextMeshProUGUI>();
        countText.text = $"{entry.count}";

        // ensure UI is active if count > 0
        existingItemUI.SetActive(true);
    }
    // called when the item button is clicked in the placement ui
    private void OnItemButtonClicked(InventoryEntry entry)
    {
        // shouldnt occur; safety
        if (entry.count <= 0)
        {
            Debug.LogWarning("Cannot select item with 0 count");
            return;
        }
        // doubly shouldnt occur, but safety
        if (!entry.data.prefab)
        {
            Debug.LogError($"Item {entry.itemName} has no prefab assigned");
            return;
        }
        // set the current selected furniture
        PlacementManager.Instance.Furniture.SetCurrentFurniture(entry.itemName);
    }
}