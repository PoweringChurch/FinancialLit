using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UISave : MonoBehaviour
{
    public static UISave Instance;

    public Transform slotGrid;
    public GameObject ingameOverlay;
    public GameObject ingameMenu;
    public GameObject savesScreen;
    public GameObject slotTemplate;
    public TextMeshProUGUI petNameInput;

    public Toggle debugToggle;

    public void Awake()
    {
        Instance = this;
        DisplaySaves();
    }

    public void DisplaySaves()
    {
        // Clear existing slots
        foreach (Transform child in slotGrid)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
        //get all .json files in the save directory
        string savePath = Application.persistentDataPath;
        string[] saveFiles = System.IO.Directory.GetFiles(savePath, "*.json");
        //sort by last modified time
        Array.Sort(saveFiles, (a, b) =>
            System.IO.File.GetLastWriteTime(b).CompareTo(System.IO.File.GetLastWriteTime(a))
        );
        //create a slot for each save file
        foreach (string filePath in saveFiles)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            // Try to load the save data to display info
            try
            {
                string json = System.IO.File.ReadAllText(filePath);
                PlayerData saveData = JsonUtility.FromJson<PlayerData>(json);

                // create slot UI
                GameObject slot = UnityEngine.Object.Instantiate(slotTemplate, slotGrid);

                var texts = slot.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = saveData.PetName; // pet name
                texts[1].text = FormatPlaytime(saveData.TotalPlaytimeSeconds); // playtime
                texts[2].text = FormatTimestamp(saveData.LastSaveTimestamp); // Last saved
                texts[3].text = saveData.DisplayStatus; // Status
                texts[4].text = $"${saveData.Money:N2}"; //money

                // add button to load this save
                Button loadButton = slot.GetComponentInChildren<Button>();
                string capturedFileName = fileName; // capture in local variable
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
        PetHelper.petMover.petTransform.gameObject.SetActive(true);
        AreaHandler.Instance.EnterHome();
        LoadThisSave(fileName);
        
        //enter game
        ingameOverlay.SetActive(true);
        savesScreen.SetActive(false);

        CameraHandler.Instance.ToggleGamecam(true);
        UIResourcesUpdater.Instance.UpdateText();

        PetHelper.petStateMachine.SetState(PetState.Idle);

        PetHelper.petAnimation.SetBoolParameter("IsSitting",false);
        PetHelper.petAnimation.SetBoolParameter("IsSick",false);

        UIInventory.Instance.UpdateInventoryUI();
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

            UIPopups.Instance.PopupInfo(
                "Hey!",
                "Because debug mode is enabled, you start with a bunch of resources and every furniture item in the game, obtainable or not! This can be disabled in settings.",
                "Sweet!"
            );
        }
        else
        {
            TutorialManager.Instance.AskTutorial();
        }
        //give starter items
        string[] starterItems = { 
            "Pet Bed", "Small Bed", "Old Monitor", "Food Bowl", 
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

        PetHelper.petFlagManager.ClearFlags();
        PetHelper.petStateMachine.SetState(PetState.Idle);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;

        PetHelper.petMover.petTransform.gameObject.SetActive(true);
        //upd ui
        UIResourcesUpdater.Instance.UpdateText();
        UIInventory.Instance.UpdateInventoryUI();
    }
    public void DeleteCurrentSave()
    {
        UIPopups.Instance.PopupYN(
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