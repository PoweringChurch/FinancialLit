using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemButtonTemplate;
    [SerializeField] private Transform contentTransform;
    
    private Inventory inventory;
    private readonly Dictionary<string, GameObject> inventoryItemUI = new Dictionary<string, GameObject>();
    
    public void SetInventory(Inventory newInventory)
    {
        inventory = newInventory;
    }
    
    public void UpdateInventoryUI()
    {
        // Clear existing UI
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        inventoryItemUI.Clear();
        
        // Create UI for each item
        foreach (var entry in inventory.GetItemsToDisplay())
        {
            GameObject newTemplate = Instantiate(itemButtonTemplate, contentTransform);
            Button itemButton = newTemplate.GetComponent<Button>();
            
            Transform displayNameText = newTemplate.transform.Find("DisplayName");
            Transform countText = newTemplate.transform.Find("Count");
            
            if (displayNameText != null)
            {
                displayNameText.GetComponent<TextMeshProUGUI>().text = entry.itemData.itemName;
            }
            
            if (countText != null)
            {
                countText.GetComponent<TextMeshProUGUI>().text = $"{entry.count}";
            }
            
            // Capture entry in closure
            itemButton.onClick.AddListener(() => OnItemButtonClicked(entry));
            
            inventoryItemUI.Add(entry.itemData.itemName, newTemplate);
        }
    }
    
    public void UpdateInventoryItem(string itemName)
    {
        if (!inventoryItemUI.ContainsKey(itemName))
        {
            Debug.LogWarning($"UI for item {itemName} not found. Refreshing entire UI.");
            UpdateInventoryUI();
            return;
        }
        
        InventoryEntry entry = inventory.GetEntry(itemName);
        if (entry == null)
        {
            Debug.LogWarning($"Item {itemName} not found in inventory.");
            return;
        }
        
        GameObject itemUI = inventoryItemUI[itemName];
        Transform countText = itemUI.transform.Find("Count");
        
        if (countText != null)
        {
            countText.GetComponent<TextMeshProUGUI>().text = $"{entry.count}";
        }
        
        // Hide button if count is 0
        if (entry.count <= 0)
        {
            itemUI.SetActive(false);
        }
        else
        {
            itemUI.SetActive(true);
        }
    }
    
    private void OnItemButtonClicked(InventoryEntry entry)
    {
        if (entry.count <= 0)
        {
            Debug.LogWarning("Cannot place item with 0 count");
            return;
        }
        
        if (entry.itemData.prefab == null)
        {
            Debug.LogError($"Item {entry.itemData.itemName} has no prefab assigned!");
            return;
        }
        FurniturePlacer.Instance.SetCurrentFurniture(entry.itemData.itemName);
    }
}