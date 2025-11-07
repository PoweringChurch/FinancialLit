using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;

    [Serializable]
    public class UIButtonManager
    {
        [Serializable]
        public class ToggleableButton
        {
            public string name;
            public Button button;
        }

        [SerializeField] private List<ToggleableButton> buttons;
        private Dictionary<string, ToggleableButton> buttonDict;

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
            RawImage unavailableImage = targetButton.button.GetComponentInChildren<RawImage>();
            if (unavailableImage) unavailableImage.enabled = !enabled;
        }
    }
    [Serializable]
    public class UIItemUpdater
    {
        public TextMeshProUGUI shampooText;
        public TextMeshProUGUI foodText;
        public TextMeshProUGUI moneyText;
        public void Initialize()
        {
            UpdateText();
        }
        public void UpdateText()
        {
            shampooText.text = PlayerResources.Instance.Shampoo.ToString();
            foodText.text = PlayerResources.Instance.Food.ToString();
            moneyText.text = $"Balance: ${PlayerResources.Instance.Money:f2}";
        }
    }
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
                Debug.LogError("Inventory not set on UIHandler");
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
                Debug.LogError($"Item {entry.itemData.itemName} has no prefab assigned");
                return;
            }

            FurniturePlacer.Instance.SetCurrentFurniture(entry.itemData.itemName);
        }
    }
    [Serializable]
    public class UIPetManager
    {
        [SerializeField]
        private Image HungerFill;
        [SerializeField]
        private Image HygieneFill;
        [SerializeField]
        private Image EntertainmentFill;
        [SerializeField]
        private Image EnergyFill;
        [SerializeField]
        private GameObject warningPrefab;
        [SerializeField]
        private Transform warningContent;

        private Dictionary<string, GameObject> activeWarnings = new();
        public void Initialize()
        {
            UpdateUI();
        }
        public void UpdateUI()
        {
            var hygiene = PetStats.Instance.Status["hygiene"];
            var hunger = PetStats.Instance.Status["hunger"];
            var entertainment = PetStats.Instance.Status["entertainment"];
            var energy = PetStats.Instance.Status["energy"];
            HygieneFill.fillAmount = hygiene;
            HungerFill.fillAmount = hunger;
            EntertainmentFill.fillAmount = entertainment;
            EnergyFill.fillAmount = energy;

            if (hunger < 0.2f) TryAddWarning($"{PetStats.Instance.PetName} is hungry!"); else ResolveWarning($"{PetStats.Instance.PetName} is hungry!");
            if (hygiene < 0.2f) TryAddWarning($"{PetStats.Instance.PetName} is dirty!"); else ResolveWarning($"{PetStats.Instance.PetName} is dirty!");
            if (entertainment < 0.2f) TryAddWarning($"{PetStats.Instance.PetName} is bored!"); else ResolveWarning($"{PetStats.Instance.PetName} is bored!");
            if (energy < 0.2f) TryAddWarning($"{PetStats.Instance.PetName} is tired!"); else ResolveWarning($"{PetStats.Instance.PetName} is tired!");
        }
        public void AddWarning(string message)
        {
            var newWarningMessage = Instantiate(warningPrefab, warningContent);
            TextMeshProUGUI messagetext = newWarningMessage.GetComponentInChildren<TextMeshProUGUI>();
            messagetext.text = message;
            activeWarnings[message] = newWarningMessage;
        }
        public void TryAddWarning(string message) //checks if warning already exists before adding
        {
            if (!activeWarnings.ContainsKey(message))
            {
                AddWarning(message);
            }
        }
        public void ResolveWarning(string message)
        {
            if (!activeWarnings.ContainsKey(message)) return;
            Destroy(activeWarnings[message]);
            activeWarnings.Remove(message);
        }
        public void ClearWarnings()
        {
            foreach (var warning in activeWarnings.Values) Destroy(warning);
            activeWarnings.Clear();
        }
    }


    [Header("Manager Settings")]
    public UIButtonManager ButtonManager = new();
    public UIInventoryManager InventoryManager = new();
    public UIItemUpdater ItemUpdater = new();
    public UIPetManager PetUI = new();
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ButtonManager.Initialize();
        InventoryManager.Initialize();
        ItemUpdater.Initialize();
        PetUI.Initialize();
    }

    //intermediarys
    public void OpenBuilder()
    {
        InventoryManager.UpdateInventoryUI();
    }
}
