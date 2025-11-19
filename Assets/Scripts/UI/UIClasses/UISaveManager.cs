using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            UnityEngine.Object.Destroy(child.gameObject);
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
                GameObject slot = UnityEngine.Object.Instantiate(slotTemplate, slotGrid);

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
        UIHandler.Instance.ItemUpdater.UpdateText();

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
        var plrData = SaveHandler.Instance.PlayerDataFromFile(fileName);
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

            UIHandler.Instance.PopupManager.PopupInfo(
                "Hey!",
                "Because debug mode is enabled, you start with a bunch of resources and every furniture item in the game, obtainable or not! This can be disabled in settings.",
                "Sweet!"
            );
        }
        else
        {
            UIHandler.Instance.PopupManager.AskTutorial();
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
        UIHandler.Instance.ItemUpdater.UpdateText();
    }
    public void DeleteCurrentSave()
    {
        UIHandler.Instance.PopupManager.PopupYN(
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