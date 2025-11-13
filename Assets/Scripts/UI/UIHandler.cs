using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
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
                return;
            }

            if (targetButton.button) targetButton.button.enabled = enabled;
            RawImage unavailableImage = targetButton.button.GetComponentInChildren<RawImage>();
            if (unavailableImage) unavailableImage.enabled = !enabled;
        }
    }
    [Serializable]
    public class UIResourcesUpdater
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
            moneyText.text = $"Balance: ${PlayerResources.Instance.Money:N2}";
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
                Destroy(child.gameObject);
            }
            inventoryItemUI.Clear();
            if (inventory == null)
            {
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
                    tmpName.text = entry.itemName;
                }
                // Find and set Count
                Transform countText = newTemplate.transform.Find("Count");
                if (countText != null && countText.TryGetComponent<TextMeshProUGUI>(out var tmpCount))
                {
                    tmpCount.text = $"{entry.count}";
                }
                itemButton.onClick.AddListener(() => OnItemButtonClicked(entry));
                inventoryItemUI.Add(entry.itemName, newTemplate);
            }
            
            // Automatically resize content to fit all items
            RectTransform contentRect = contentTransform.GetComponent<RectTransform>();
            int itemCount = inventory.GetItemsToDisplay().Count;
            float buttonHeight = 80f;
            float spacing = 0f; // Adjust based on your layout spacing
            float newHeight = Mathf.Max(80f, 80f+(buttonHeight + spacing) * (1+itemCount%10));
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, newHeight);
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
                    Destroy(itemUI);
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
            //shouldnt even occur
            if (entry.count <= 0)
            {
                Debug.LogWarning("Cannot select item with 0 count");
                return;
            }
            if (entry.
            data.
            prefab == null)
            {
                Debug.LogError($"Item {entry.itemName} has no prefab assigned");
                return;
            }

            FurniturePlacer.Instance.SetCurrentFurniture(entry.itemName);
        }
    }
    [Serializable]
    public class UIPetManager
    {
        public GameObject StatusPanel;
        [SerializeField]
        private Image DisplayImage;
        [SerializeField]
        private TextMeshProUGUI DisplayText;
        [SerializeField]
        private Image HungerFill;
        [SerializeField]
        private Image HygieneFill;
        [SerializeField]
        private Image EntertainmentFill;
        [SerializeField]
        private Image EnergyFill;

        private float currentHygiene;
        private float currentHunger;
        private float currentEntertainment;
        private float currentEnergy;
        private Color currentDisplayColor;
        private float lerpSpeed = 3f;
        public void Initialize()
        {
            currentHygiene = PetStats.Instance.Status["hygiene"];
            currentHunger = PetStats.Instance.Status["hunger"];
            currentEntertainment = PetStats.Instance.Status["entertainment"];
            currentEnergy = PetStats.Instance.Status["energy"];
            UpdateUI();
        }
        public void UpdateUI()
        {
            var hygiene = PetStats.Instance.Status["hygiene"];
            var hunger = PetStats.Instance.Status["hunger"];
            var entertainment = PetStats.Instance.Status["entertainment"];
            var energy = PetStats.Instance.Status["energy"];

            currentHygiene = Mathf.Lerp(currentHygiene, hygiene, Time.deltaTime * lerpSpeed);
            currentHunger = Mathf.Lerp(currentHunger, hunger, Time.deltaTime * lerpSpeed);
            currentEntertainment = Mathf.Lerp(currentEntertainment, entertainment, Time.deltaTime * lerpSpeed);
            currentEnergy = Mathf.Lerp(currentEnergy, energy, Time.deltaTime * lerpSpeed);

            HygieneFill.fillAmount = currentHygiene;
            HungerFill.fillAmount = currentHunger;
            EntertainmentFill.fillAmount = currentEntertainment;
            EnergyFill.fillAmount = currentEnergy;

            HungerFill.color = Color.Lerp(Color.red, Color.green, currentHunger);
            EnergyFill.color = Color.Lerp(Color.red, Color.green, currentEnergy);
            HygieneFill.color = Color.Lerp(Color.red, Color.green, currentHygiene);
            EntertainmentFill.color = Color.Lerp(Color.red, Color.green, currentEntertainment);

            var total = entertainment + hygiene + energy + hunger;
            string displaytext = "OKAY";

            if (total > 3.5) displaytext = "FINE";
            if (total > 3.7) displaytext = "GREAT";
            if (total > 3.9) displaytext = "WONDERFUL";
            if (entertainment < 0.5) displaytext = "BORED";
            if (entertainment < 0.2) displaytext = "SAD";
            if (hygiene < 0.3) displaytext = "DIRTY";
            if (hunger < 0.5) displaytext = "HUNGRY";
            if (hunger < 0.3) displaytext = "STARVING";
            if (energy < 0.4) displaytext = "TIRED";
            if (energy < 0.2) displaytext = "EXHAUSTED";
            if (total < 0.8) displaytext = "CRITICAL";

            var colorDict = new Dictionary<string, Color>
            {
                ["OKAY"] = new Color(0.5f, 0.6f, 0.5f),
                ["FINE"] = new Color(0.5f, 0.8f, 0.6f),
                ["GREAT"] = new Color(0.5f, 0.9f, 0.6f),
                ["WONDERFUL"] = new Color(0.3f, 0.9f, 0.5f),
                ["BORED"] = new Color(0.6f, 0.6f, 0.8f),
                ["SAD"] = new Color(0.4f, 0.4f, 0.7f),
                ["DIRTY"] = new Color(0.6f, 0.5f, 0.3f),
                ["HUNGRY"] = new Color(0.6f, 0.4f, 0.2f),
                ["STARVING"] = new Color(0.9f, 0.3f, 0f),
                ["TIRED"] = new Color(0.7f, 0.7f, 0.5f),
                ["EXHAUSTED"] = new Color(0.3f, 0.3f, 0.3f),
                ["CRITICAL"] = Color.darkRed
            };

            Color targetColor = colorDict[displaytext];
            currentDisplayColor = Color.Lerp(currentDisplayColor, targetColor, Time.deltaTime * lerpSpeed);
            DisplayImage.color = currentDisplayColor;
            DisplayText.text = displaytext;
        }
    }
    [Serializable]
    public class UICursorHelper
    {
        private static Vector2 cursorHotspot = Vector2.zero;
        private static CursorMode cursorMode = CursorMode.Auto;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] Camera gameCamera;
        public Texture2D defaultCursor;
        public Texture2D followingCursor;
        public (Vector3, bool) CursorToVector3(float targetY)
        {
            bool overInteractableLayer = false;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            // Create ray from camera through mouse position
            Ray ray = gameCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out _, 1000, interactableLayer)) overInteractableLayer = true;
            // Calculate where ray intersects the target Y plane
            float t = (targetY - ray.origin.y) / ray.direction.y;
            Vector3 targetPos = ray.origin + ray.direction * t;
            return (targetPos, overInteractableLayer);
        }

        public void SetCursor(Texture2D newcursor)
        {
            Cursor.SetCursor(newcursor, cursorHotspot, cursorMode);
        }
    }
    [Serializable]
    public class UIPopupManager
    {
        public GameObject infoPanelTemplate;
        public Transform PopupsTransform;

        public void PopupInfo(string header, string body, string dismiss = "OK")
        {
            GameObject newInfoPanel = Instantiate(infoPanelTemplate, PopupsTransform);
            var tmps = newInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();

            // Header, Body, Dismiss button text
            tmps[0].text = header;
            tmps[1].text = body;
            tmps[2].text = dismiss;

            // Setup dismiss button
            Button dismissButton = newInfoPanel.GetComponentInChildren<Button>();
            dismissButton.onClick.AddListener(() => Destroy(newInfoPanel));
        }
    }
    
    [Serializable]
    public class UISaveManager
    {
        public Transform slotGrid;
        public GameObject ingameOverlay;
        public GameObject savesScreen;
        public GameObject slotTemplate;
        public TextMeshProUGUI petNameInput;

        public bool debugMode = false;

        public void Initialize()
        {
            DisplaySaves();
        }
        public void DisplaySaves()
        {
            // Clear existing slots
            foreach (Transform child in slotGrid)
            {
                Destroy(child.gameObject);
            }
            // Get all .json files in the save directory
            string savePath = Application.persistentDataPath;
            string[] saveFiles = System.IO.Directory.GetFiles(savePath, "*.json");
            // Sort by last modified time (most recent first)
            Array.Sort(saveFiles, (a, b) =>
                System.IO.File.GetLastWriteTime(b).CompareTo(System.IO.File.GetLastWriteTime(a))
            );
            // Create a slot for each save file
            foreach (string filePath in saveFiles)
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                // Try to load the save data to display info
                try
                {
                    string json = System.IO.File.ReadAllText(filePath);
                    PlayerData saveData = JsonUtility.FromJson<PlayerData>(json);

                    // Create slot UI
                    GameObject slot = Instantiate(slotTemplate, slotGrid);

                    // Fill out slot info (adjust based on your template structure)
                    var texts = slot.GetComponentsInChildren<TextMeshProUGUI>();
                    texts[0].text = saveData.PetName; // Slot name/pet name
                    texts[1].text = FormatPlaytime(saveData.TotalPlaytimeSeconds); // Playtime
                    texts[2].text = FormatTimestamp(saveData.LastSaveTimestamp); // Last saved
                    texts[3].text = saveData.DisplayStatus; // Status
                    texts[4].text = $"${saveData.Money:N2}"; //money

                    // Add button to load this save
                    Button loadButton = slot.GetComponentInChildren<Button>();
                    string capturedFileName = fileName; // Capture in local variable
                    loadButton.onClick.AddListener(() => OnLoadClick(fileName));
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to load save file {fileName}: {e.Message}");
                }
            }
        }
        private void OnLoadClick(string fileName)
        {
            LoadThisSave(fileName);
            ingameOverlay.SetActive(true);
            savesScreen.SetActive(false);
            CameraHandler.Instance.ToggleGamecam(true);
        }
        string FormatPlaytime(float seconds)
        {
            int hours = (int)(seconds / 3600);
            int minutes = (int)(seconds % 3600 / 60);
            return $"{hours}h {minutes}m";
        }
        string FormatTimestamp(long timestamp)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            return dateTime.ToString("MMM dd, HH:mm");
        }
        void LoadThisSave(string fileName)
        {
            var plrData = SaveHandler.Instance.LoadGameFromFile(fileName);
            SaveHandler.Instance.LoadSaved(plrData);
        }
        public void NewSave()
        {
            if (petNameInput.text == "")
            {
                return; //cant have empty name
            }
            PlayerData newData = new()
            {
                PetName = petNameInput.text
            };
            if (debugMode)
            {
                FurnitureData[] allItems = FurnitureDatabase.GetAllItems();
                foreach (FurnitureData data in allItems)
                {
                    newData.PlayerInventory.AddItem(data, 1000);
                }
                newData.Money = 1000000;
                newData.Shampoo = 10000;
                newData.Food = 1000;

                UIHandler.Instance.PopupManager.PopupInfo(
                    "Hey!",
                    "Because debug mode is enabled, you start with a bunch of resources and every furniture item in the game, obtainable or not! This can be disabled in settings.",
                    "Sweet!"
                );
            }
            SaveHandler.Instance.currentPlayerData = newData;
            SaveHandler.Instance.LoadSaved(newData);
            SaveHandler.Instance.currentSaveFile = $"save_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";
        }
    }
    [Header("Manager Settings")]
    public UIButtonManager ButtonManager = new();
    public UIInventoryManager InventoryManager = new();
    public UIResourcesUpdater ItemUpdater = new();
    public UIPetManager PetUI = new();
    public UICursorHelper CursorHelper = new();
    public UIPopupManager PopupManager = new();
    public UISaveManager SaveManagerUI = new();
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
        SaveManagerUI.Initialize();
    }
    void Update()
    {
        PetUI.UpdateUI();
    }
    //intermediarys
    public void OpenBuilder()
    {
        InventoryManager.UpdateInventoryUI();
    }
    public void ToggleStatusPanel()
    {
        PetUI.StatusPanel.SetActive(!PetUI.StatusPanel.activeSelf);
    }
    public void NewSave()
    {
        SaveManagerUI.NewSave();
    }
    public void SaveGame()
    {
        SaveHandler.Instance.SaveGame();
        SaveManagerUI.DisplaySaves();
    }
    //helpers
}