using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Collections;
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
            RawImage unavailableImage = targetButton.button.transform.Find("Unavailable").GetComponent<RawImage>();
            if (unavailableImage) unavailableImage.enabled = !enabled;
        }
        
    }
    [Serializable]
    public class UIResourcesUpdater
    {
        public GameObject spendingsPanel;

        public TextMeshProUGUI shampooText;
        public TextMeshProUGUI foodText;
        public TextMeshProUGUI moneyText;

        public TextMeshProUGUI foodspendings;
        public TextMeshProUGUI hygieneSpendings;
        public TextMeshProUGUI furnitureSpendings;
        public TextMeshProUGUI healthcareSpendings;

        public void Initialize()
        {
            UpdateText();
        }
        public void UpdateText()
        {
            shampooText.text = PlayerResources.Instance.Shampoo.ToString();
            foodText.text = PlayerResources.Instance.Food.ToString();
            moneyText.text = $"Balance: ${PlayerResources.Instance.Money:N2}";

            foodspendings.text = $"${PlayerResources.Instance.Spendings["Food"].ToString()}";
            hygieneSpendings.text = $"${PlayerResources.Instance.Spendings["Hygiene"].ToString()}";
            healthcareSpendings.text = $"${PlayerResources.Instance.Spendings["Healthcare"].ToString()}";
            furnitureSpendings.text = $"${PlayerResources.Instance.Spendings["Furniture"].ToString()}";
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
        public GameObject ynPanelTemplate;
        public Transform PopupsTransform;

        public void PopupInfo(string header, string body, string dismiss = "OK", Action action = null)
        {
            GameObject newInfoPanel = Instantiate(infoPanelTemplate, PopupsTransform);
            var tmps = newInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();

            // Header, Body, Dismiss button text
            tmps[0].text = header;
            tmps[1].text = body;
            tmps[2].text = dismiss;

            // Setup dismiss button
            Button dismissButton = newInfoPanel.GetComponentInChildren<Button>();
            dismissButton.onClick.AddListener(() => {
                Destroy(newInfoPanel);
                if (action != null) action.Invoke();
            });
        }
        public void PopupYN(string header, string body, Action onYes, Action onNo = null, string y = "Yes", string n = "No")
        {
            GameObject newYNPanel = Instantiate(ynPanelTemplate, PopupsTransform);
            var tmps = newYNPanel.GetComponentsInChildren<TextMeshProUGUI>();
            
            // Header, Body, Yes text, No text
            tmps[0].text = header;
            tmps[1].text = body;
            tmps[2].text = y;
            tmps[3].text = n;
            
            Button[] buttons = newYNPanel.GetComponentsInChildren<Button>();
            
            // Yes button
            buttons[0].onClick.AddListener(() => 
            {
                onYes?.Invoke();
                Destroy(newYNPanel);
            });
            
            // No button
            buttons[1].onClick.AddListener(() => 
            {
                onNo?.Invoke();
                Destroy(newYNPanel);
            });
        }

        public void AskTutorial()
        {
            PopupYN("Tutorial", 
            "Would you like to learn how to play?", 
            () => StartTutorial(), 
            null, 
            "Teach me!", 
            "Skip");
        }
        public void StartTutorial()
        {
            // Welcome
            PopupInfo("Welcome!", 
                "Let's learn how to take care of your new pet!", 
                "Next", 
                () => {
                    // Pet care basics
                    PopupInfo("Pet Care Basics", 
                        "Keep your pet healthy by managing four needs:\n\n" +
                        "• Hygiene - Bathe your pet regularly\n" +
                        "• Hunger - Feed your pet when hungry\n" +
                        "• Energy - Send them to sleep on a dog bed\n" +
                        "• Entertainment - Play at the park or with toys", 
                        "Got it!", 
                        () => {
                            // Health warning
                            PopupInfo("Stay Healthy", 
                                "If your pet's needs get too low, they might get sick and need a trip to the vet. Keep those stats up! \n\n" +
                                "You're able to view your pet's needs at any time by pressing on the mood display on the bottom left.", 
                                "Understood", 
                                () => {
                                    // Working from home
                                    PopupInfo("Money", 
                                        "You can work from home to earn money! Just click any monitor and press \"Go to work\". \n\n" +
                                        "You can also see how you spend your earnings by pressing on your balance in the top left!",
                                        "Nice!", 
                                        () => {
                                            // Placement mode
                                            PopupInfo("Furniture & Items", 
                                                "Want to add furniture and decor?\n\n" +
                                                "Expand the arrow on the bottom right and select the yellow button with tools to enter placement mode.\n\n" +
                                                "You've been given some basic furniture items to start building up your home with.", 
                                                "Cool!", 
                                                () => {
                                                    // Travel
                                                    PopupInfo("Explore & Shop", 
                                                        "Travel to different locations by expanding the arrow on the bottom right and selecting the blue map.\n\n" +
                                                        "Different areas offer different services - explore them all!", 
                                                        "Let's go!", 
                                                        () => {
                                                            // Final message
                                                            PopupInfo("You're Ready!", 
                                                                "That's everything you need to know. Have fun taking care of your pet!", 
                                                                "Enter game");
                                                        });
                                                });
                                        });
                                });
                        });
                });
        }
    }
    
    [Serializable]
    public class UISaveManager
    {
        public Transform slotGrid;
        public GameObject ingameOverlay;
        public GameObject ingameMenu;
        public GameObject savesScreen;
        public GameObject slotTemplate;
        public TextMeshProUGUI petNameInput;

        public Toggle debugToggle;

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
            PetMover.Instance.petTransform.gameObject.SetActive(true);
            AreaHandler.Instance.EnterHome();
            LoadThisSave(fileName);

            //enter game
            ingameOverlay.SetActive(true);
            savesScreen.SetActive(false);

            CameraHandler.Instance.ToggleGamecam(true);
            Instance.ItemUpdater.UpdateText();
            PetStateMachine.SetState(PetState.Idle);

            PetAnimation.Instance.SetBoolParameter("IsSitting",false);
            PetAnimation.Instance.SetBoolParameter("IsSick",false);
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
            if (debugToggle.isOn)
            {
                FurnitureData[] allData = FurnitureDatabase.GetAllData();
                foreach (FurnitureData data in allData)
                {
                    newData.PlayerInventory.AddItem(data, 1000);
                }
                newData.Money = 1000000;
                newData.Shampoo = 10000;
                newData.Food = 1000;

                Instance.PopupManager.PopupInfo(
                    "Hey!",
                    "Because debug mode is enabled, you start with a bunch of resources and every furniture item in the game, obtainable or not! This can be disabled in settings.",
                    "Sweet!"
                );
            }
            else
            {
                Instance.PopupManager.AskTutorial();
            }
            //give starter items
            string[] starterItems = { 
                "Pet Bed", "Small Bed", "Work Computer", "Food Bowl", 
                "Bathroom Vanity", "Box Bath", "Toy Train", "Couch", 
                "Toilet", "Rectangle Table" 
            };
            foreach (string itemName in starterItems)
            {
                FurnitureData data = FurnitureDatabase.GetData(itemName);
                if (data != null) newData.PlayerInventory.AddItem(data, 1);
            }
            //set as new data
            SaveHandler.Instance.currentPlayerData = newData;
            SaveHandler.Instance.LoadSaved(newData);
            SaveHandler.Instance.currentSaveFile = $"save_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";

            //upd external
            PetFlagManager.ClearFlags();
            PetStateMachine.SetState(PetState.Idle);
            PetBehaviour.Instance.ActiveBehaviour = Behaviour.Default;

            PetMover.Instance.petTransform.gameObject.SetActive(true);
            //upd ui
            Instance.ItemUpdater.UpdateText();
        }
        public void DeleteCurrentSave()
        {
            Instance.PopupManager.PopupYN(
            "Delete Save?",
            "Are you sure you want to delete this save? This cannot be undone.",
            onYes: () => 
            {
                ingameOverlay.SetActive(false);
                savesScreen.SetActive(true);

                CameraHandler.Instance.ToggleGamecam(false);
                
                SaveHandler.Instance.DeleteSave(SaveHandler.Instance.currentSaveFile);
                DisplaySaves();
                Debug.Log("Save deleted");
            },
            onNo: () => 
            {
                ingameMenu.SetActive(true);
                Debug.Log("Cancelled");
            }
        );
            
        }
    }
    
    [Serializable]
    public class UIFlagManager
    {

        [Serializable]
        public class FlagIcon
        {
            public GameObject gameObject; //object with rawimage
            public string Name;
            public string Effect; //Positive, Mixed, Negative
            public string Description;
            public PetFlag petFlag;
        }
        public Transform popupsContainer;
        public Transform flagContainerTransform; 
        public GameObject descriptionDisplayPrefab; //panel w/ three tmpro children, [0] = name, [1] = effect, [2] = desc
        public FlagIcon[] flagIcons;

        private GameObject currentDescription;
        private Dictionary<PetFlag, FlagIcon> flagIconMap;
        private FlagIcon currentDescriptionIcon;
        //icon should show description when hovered over by mouse
        public void Initialize()
        {
        flagIconMap = new Dictionary<PetFlag, FlagIcon>();
        foreach (var icon in flagIcons)
        {
            flagIconMap[icon.petFlag] = icon;
            icon.gameObject.SetActive(false);
            
            var eventTrigger = icon.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null) eventTrigger = icon.gameObject.AddComponent<EventTrigger>();
            
            var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnter.callback.AddListener((data) => ShowDescription(icon));
            eventTrigger.triggers.Add(pointerEnter);
            
            var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((data) => HideDescription());
            eventTrigger.triggers.Add(pointerExit);
            }
            
            PetFlagManager.OnFlagChanged += UpdateFlags;
            UpdateFlags();
        }
        void UpdateFlags()
        {
            foreach (var icon in flagIcons)
            {
                bool hasFlag = PetFlagManager.HasFlag(icon.petFlag);
                icon.gameObject.SetActive(hasFlag);
                
                if (!hasFlag && currentDescription != null && currentDescriptionIcon == icon)
                {
                    HideDescription();
                }
            }
        }
        void ShowDescription(FlagIcon icon)
        {
            HideDescription();
            currentDescriptionIcon = icon;
            currentDescription = Instantiate(descriptionDisplayPrefab, popupsContainer);
            var texts = currentDescription.GetComponentsInChildren<TMP_Text>();
            texts[0].text = icon.Name;
            texts[1].text = icon.Effect;
            texts[2].text = icon.Description;
        }
        void HideDescription()
        {
            if (currentDescription != null) Destroy(currentDescription);
        }
        
        void OnDestroy()
        {
            PetFlagManager.OnFlagChanged -= UpdateFlags;
        }
    }
    [Serializable]
    public class MenuAnimationManager
    {
        public Transform flag;

        public RawImage bgScrollerImage;
        public Vector2 scrollSpeed = new Vector2(0.1f, 0.1f);
        public void UpdateUI()
        {
            //rotate flag
            float angle = Mathf.Sin(Time.time * 0.5f) * 5f;
            flag.localRotation = Quaternion.Euler(0, 0, angle);

            //move bg scroller
            bgScrollerImage.uvRect = new Rect(
                bgScrollerImage.uvRect.position + scrollSpeed * Time.deltaTime,
                bgScrollerImage.uvRect.size
            );
        }
    }

    [Serializable]
    public class UIWorkManager
    {
        public GameObject workoverlayUI;
        public GameObject ingameOverlayUI;
        public TextMeshProUGUI completedOrders;
        public TextMeshProUGUI timerText;
        public GameObject ballItemUI;
        public GameObject brushItemUI;
        public GameObject treatItemUI;
        public GameObject shampooItemUI;        
        public Transform reqHolder;
        public RectTransform boxTransform;
        private Dictionary<Items, GameObject> itemUIMap;        
        public void Initialize()
        {            
            itemUIMap = new Dictionary<Items, GameObject>
            {
                { Items.Ball, ballItemUI },
                { Items.Brush, brushItemUI },
                { Items.Treat, treatItemUI },
                { Items.Shampoo, shampooItemUI }
            };
        }
        
        public void NextBox()
        {
            Instance.StartCoroutine(BoxTransitionAnimation());
        }
        private IEnumerator BoxTransitionAnimation()
        {
            float duration = 0.5f;
            float elapsed = 0f;
            
            // Get screen width for off-screen positions
            float screenWidth = Screen.width;
            Vector2 centerPos = boxTransform.anchoredPosition;
            Vector2 rightOffScreen = new Vector2(screenWidth, centerPos.y);
            Vector2 leftOffScreen = new Vector2(-screenWidth, centerPos.y);
            
            // Slide current box off to the right
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                boxTransform.anchoredPosition = Vector2.Lerp(centerPos, rightOffScreen, t);
                yield return null;
            }
            
            // Teleport box to left side (off screen)
            boxTransform.anchoredPosition = leftOffScreen;
            
            // Reset elapsed time for slide in
            elapsed = 0f;
            
            // Slide new box in from the left
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);
                boxTransform.anchoredPosition = Vector2.Lerp(leftOffScreen, centerPos, t);
                yield return null;
            }
            // Ensure it's exactly centered
            boxTransform.anchoredPosition = centerPos;
        }
        public void StartWorking()
        {
            workoverlayUI.SetActive(true);
            OrderHandler.Instance.BeginShift();
        }
        
        public void UpdateTimer(float timeRemaining)
        {
            if (timerText != null)
            {
                timerText.text = $"Time: {timeRemaining:F1}s";
            }
        }
        public void UpdateOrderDisplay(List<Items> currentOrder)
        {
            for (int i = reqHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(reqHolder.GetChild(i).gameObject);
            }
            foreach (Items item in currentOrder)
            {
                Instantiate(itemUIMap[item],reqHolder);
            }
        }
        public void UpdateCompletedOrders(int completed, int total)
        {
            if (completedOrders != null)
            {
                completedOrders.text = $"Orders: {completed}/{total}";
            }
        }
        public void CancelWork()
        {
            string header = "Stop working?";
            string body = "Are you sure you want to stop working? You will lose any earned money.";
            Instance.PopupManager.PopupYN(header,body, () =>
            {
                OrderHandler.Instance.CancelShift();
                CameraHandler.Instance.ToggleGamecam(true);
                ingameOverlayUI.SetActive(true);
                workoverlayUI.SetActive(false);
            }, () => {});
        }
        public void EnterWork()
        {
            string header = "Start working";
            string body = "Fulfill customer orders by dragging the requested items into the delivery box. Work fast for bonus earnings!";
            Instance.PopupManager.PopupYN(header,body, () =>
            {
                OrderHandler.Instance.BeginShift();
                CameraHandler.Instance.ToggleGamecam(false);
                workoverlayUI.SetActive(true);
                ingameOverlayUI.SetActive(false);
            }, null, "Start", "Nevermind");
        }
        public void EndShift(float totalEarned)
        {
            string body = $"Great work! You earned ${totalEarned:F2} for your hard work!";
            Instance.PopupManager.PopupInfo("Job well done!",body,"Yay!",() =>
            {
                CameraHandler.Instance.ToggleGamecam(true);
                ingameOverlayUI.SetActive(true);
                workoverlayUI.SetActive(false);
            });
        }
    }
    [Serializable]
    public class SettingsUIManager
    {
        public Toggle fullScreenToggle;
        public Toggle vsyncToggle;
        public TMP_Dropdown antiAliasingDropdown;
        public Toggle anisotropicFilteringToggle;
        public TMP_Dropdown textureQualityDropdown;
        public Toggle realtimeReflectionsToggle;

        public void Initialize()
        {
            fullScreenToggle.onValueChanged.AddListener(SetFullscreen);
            vsyncToggle.onValueChanged.AddListener(SetVsync);
            antiAliasingDropdown.onValueChanged.AddListener(SetAntiAliasing);
            anisotropicFilteringToggle.onValueChanged.AddListener(SetAnisotropicFiltering);
            textureQualityDropdown.onValueChanged.AddListener(SetTextureQuality);
            realtimeReflectionsToggle.onValueChanged.AddListener(SetRealtimeReflections);
        }
        //settings
        public void SetFullscreen(bool to)
        {
            Screen.fullScreen = to;
        }

        public void SetVsync(bool to)
        {
            QualitySettings.vSyncCount = to ? 1 : 0;
        }

        public void SetAntiAliasing(int level)
        {
            // Dropdown: 0=Off, 1=2x, 2=4x, 3=8x
            int[] aaLevels = { 0, 2, 4, 8 };
            QualitySettings.antiAliasing = aaLevels[level];
        }

        public void SetAnisotropicFiltering(bool enabled)
        {
            QualitySettings.anisotropicFiltering = enabled ? AnisotropicFiltering.Enable : AnisotropicFiltering.Disable;
        }

        public void SetTextureQuality(int quality)
        {
            // 0 = full res, 1 = half res, 2 = quarter res, 3 = eighth res
            QualitySettings.globalTextureMipmapLimit = quality;
        }

        public void SetRealtimeReflections(bool enabled)
        {
            QualitySettings.realtimeReflectionProbes = enabled;
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
    public UIFlagManager FlagManager = new();
    public UIWorkManager WorkManager = new();
    public MenuAnimationManager MenuAnimation = new();
    public SettingsUIManager SettingsManager = new();
    
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
        ButtonManager.Initialize();
        InventoryManager.Initialize();
        ItemUpdater.Initialize();
        PetUI.Initialize();
        SaveManagerUI.Initialize();
        FlagManager.Initialize();
        WorkManager.Initialize();
        SettingsManager.Initialize();
    }
    void Update()
    {
        PetUI.UpdateUI();
        MenuAnimation.UpdateUI();
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
    public void DeleteCurrentSave()
    {
        SaveManagerUI.DeleteCurrentSave();
    }
    public void CancelWorking()
    {
        WorkManager.CancelWork();
    }
    public void ToggleSpendingsPanel()
    {
        ItemUpdater.spendingsPanel.SetActive(!ItemUpdater.spendingsPanel.activeSelf);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}