using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Note: This file assumes the existence of 'Inventory', 'InventoryEntry', 
// 'ItemData', and 'FurniturePlacer' classes in your project.

public class UIHandler : MonoBehaviour
{
    // --- 1. Singleton Instance ---
    public static UIHandler Instance;

    // --- 2. Inner Class for Button Toggling (UIButtonManager) ---
    
    [Serializable]
    public class UIButtonManager
    {
        [Serializable]
        public class ToggleableButton
        {
            public string name;
            public Button button;
            public RawImage unavailableImage;
        }

        [SerializeField] private List<ToggleableButton> buttons;
        private Dictionary<string, ToggleableButton> buttonDict;

        // Initialization method, called by UIHandler.Awake
        public void Initialize()
        {
            buttonDict = new Dictionary<string, ToggleableButton>();
            if (buttons != null)
            {
                foreach (var btn in buttons)
                {
                    if (!string.IsNullOrEmpty(btn.name))
                    {
                        buttonDict[btn.name] = btn;
                    }
                }
            }
        }

        public void DisableButton(string buttonName) => SetButtonState(buttonName, false);
        public void EnableButton(string buttonName) => SetButtonState(buttonName, true);

        private void SetButtonState(string buttonName, bool enabled)
        {
            if (!buttonDict.TryGetValue(buttonName, out ToggleableButton targetButton))
            {
                Debug.LogWarning($"Button '{buttonName}' not found in UIButtonManager!");
                return;
            }

            if (targetButton.button) targetButton.button.enabled = enabled;
            if (targetButton.unavailableImage) targetButton.unavailableImage.enabled = !enabled;
        }
    }


    // --- 3. Inner Class for Inventory UI (UIInventoryManager) ---
    
    [Serializable]
    public class UIInventoryManager
    {
        // Serialized fields required for Unity Inspector links
        [SerializeField] private GameObject itemButtonTemplate;
        [SerializeField] private Transform contentTransform;

        // Private operational data
        private Inventory inventory; 
        private readonly Dictionary<string, GameObject> inventoryItemUI = new Dictionary<string, GameObject>();

        // Initialization method (can be left empty but included for consistency)
        public void Initialize() 
        {
            // Perform any initial setup here, e.g., checking for required components.
            if (itemButtonTemplate == null || contentTransform == null)
            {
                Debug.LogError("InventoryManager components not fully linked in Inspector!");
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
                Destroy(child.gameObject);
            }
            inventoryItemUI.Clear();

            if (inventory == null)
            {
                Debug.LogError("Inventory not set on UIHandler!");
                return;
            }
            
            foreach (var entry in inventory.GetItemsToDisplay()) 
            {
                GameObject newTemplate = Instantiate(itemButtonTemplate, contentTransform);
                Button itemButton = newTemplate.GetComponent<Button>();

                // Find and set Display Name
                Transform displayNameText = newTemplate.transform.Find("DisplayName");
                if (displayNameText != null && displayNameText.TryGetComponent<TextMeshProUGUI>(out var tmpName))
                {
                    tmpName.text = entry.itemData.itemName;
                }

                // Find and set Count
                Transform countText = newTemplate.transform.Find("Count");
                if (countText != null && countText.TryGetComponent<TextMeshProUGUI>(out var tmpCount))
                {
                    tmpCount.text = $"{entry.count}";
                }

                itemButton.onClick.AddListener(() => OnItemButtonClicked(entry));
                inventoryItemUI.Add(entry.itemData.itemName, newTemplate);
            }
        }

        public void UpdateInventoryItem(string itemName)
        {
            if (inventory == null)
            {
                Debug.LogError("Inventory not set on UIHandler!");
                return;
            }

            InventoryEntry entry = inventory.GetEntry(itemName);

            if (entry == null || entry.count <= 0)
            {
                // If item is removed or count is zero, clean up the UI element
                if (inventoryItemUI.TryGetValue(itemName, out GameObject itemUI))
                {
                    Destroy(itemUI);
                    inventoryItemUI.Remove(itemName);
                }
                return;
            }

            // If the item UI doesn't exist but the item is in inventory, refresh the whole UI 
            if (!inventoryItemUI.TryGetValue(itemName, out GameObject existingItemUI))
            {
                Debug.LogWarning($"UI for item {itemName} not found. Refreshing entire UI.");
                UpdateInventoryUI();
                return;
            }

            // Update the count text
            Transform countText = existingItemUI.transform.Find("Count");
            if (countText != null && countText.TryGetComponent<TextMeshProUGUI>(out var tmpCount))
            {
                tmpCount.text = $"{entry.count}";
            }
            
            // Ensure UI is active if count > 0
            existingItemUI.SetActive(true);
        }

        private void OnItemButtonClicked(InventoryEntry entry)
        {
            if (entry.count <= 0)
            {
                Debug.LogWarning("Cannot select item with 0 count");
                return;
            }

            if (entry.itemData.prefab == null)
            {
                Debug.LogError($"Item {entry.itemData.itemName} has no prefab assigned!");
                return;
            }
            
            // Requires the 'FurniturePlacer' class to be accessible globally
            FurniturePlacer.Instance.SetCurrentFurniture(entry.itemData.itemName); 
        }
    }

    // --- 4. Main UIHandler Serialized Managers ---

    [Header("Manager Settings")]
    [Tooltip("Configuration for toggling button interactivity and visuals.")]
    [SerializeField]
    public UIButtonManager ButtonManager = new UIButtonManager();

    [Tooltip("Configuration for dynamic inventory display and item buttons.")]
    [SerializeField]
    public UIInventoryManager InventoryManager = new UIInventoryManager();


    // --- 5. MonoBehaviour Lifecycle ---
    private void Awake()
    {
        // Singleton Initialization
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: if your UI handler persists across scenes

        // Initialize all nested manager logic
        ButtonManager.Initialize();
        InventoryManager.Initialize();
    }
}
