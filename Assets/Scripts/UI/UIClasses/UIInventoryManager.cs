using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIInventoryManager
{
    [SerializeField] private GameObject itemButtonTemplate;
    [SerializeField] private Transform contentTransform;

    private Inventory inventory;
    private readonly Dictionary<string, GameObject> inventoryItemUI = new();

    public void Initialize()
    {
        if (itemButtonTemplate == null || contentTransform == null)
        {
            Debug.LogError("InventoryManager components not fully linked in Inspector");
        }
    }

    public void SetInventory(Inventory newInventory)
    {
        inventory = newInventory;
    }

    public void UpdateInventoryUI()
    {
        // Clear existing UI
        foreach (Transform child in contentTransform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
        inventoryItemUI.Clear();
        if (inventory == null)
        {
            return;
        }
        foreach (var entry in inventory.GetItemsToDisplay())
        {
            GameObject newTemplate = UnityEngine.Object.Instantiate(itemButtonTemplate, contentTransform);
            Button itemButton = newTemplate.GetComponent<Button>();
            //set count
            TextMeshProUGUI countText = newTemplate.transform.GetComponentInChildren<TextMeshProUGUI>();
            countText.text = $"{entry.count}";
            //set img
            Transform inner = newTemplate.transform.GetChild(1);
            var imgPreview = inner.GetChild(0).GetComponent<Image>();
            imgPreview.sprite = entry.data.icon;
            //add clicking functionality
            itemButton.onClick.AddListener(() => OnItemButtonClicked(entry));
            inventoryItemUI.Add(entry.itemName, newTemplate);
        }
    }

    public void UpdateInventoryItem(string itemName)
    {
        if (inventory == null)
        {
            return;
        }

        InventoryEntry entry = inventory.GetEntry(itemName);

        if (entry == null || entry.count <= 0)
        {
            // If item is removed or count is zero, clean up the UI element
            if (inventoryItemUI.TryGetValue(itemName, out GameObject itemUI))
            {
                UnityEngine.Object.Destroy(itemUI);
                inventoryItemUI.Remove(itemName);
            }
            return;
        }

        // If the item UI doesn't exist but the item is in inventory, refresh the whole UI 
        if (!inventoryItemUI.TryGetValue(itemName, out GameObject existingItemUI))
        {
            UpdateInventoryUI();
            return;
        }

        // Update the count text
        TextMeshProUGUI countText = existingItemUI.GetComponentInChildren<TextMeshProUGUI>();
        countText.text = $"{entry.count}";

        // Ensure UI is active if count > 0
        existingItemUI.SetActive(true);
    }

    private void OnItemButtonClicked(InventoryEntry entry)
    {
        //shouldnt even occur
        if (entry.count <= 0)
        {
            Debug.LogWarning("Cannot select item with 0 count");
            return;
        }
        if (!entry.data.prefab)
        {
            Debug.LogError($"Item {entry.itemName} has no prefab assigned");
            return;
        }

        FurniturePlacer.Instance.SetCurrentFurniture(entry.itemName);
    }
}