using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// handles saving and ui
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
        // clear existing slots
        foreach (Transform child in slotGrid)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
        // get all .json files in the save directory
        string savePath = Application.persistentDataPath;
        string[] saveFiles = System.IO.Directory.GetFiles(savePath, "*.json");
        // sort by last modified time
        Array.Sort(saveFiles, (a, b) =>
            System.IO.File.GetLastWriteTime(b).CompareTo(System.IO.File.GetLastWriteTime(a))
        );
        // create a slot for each save file
        foreach (string filePath in saveFiles)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            // try to load the save data to display info
            try
            {
                string json = System.IO.File.ReadAllText(filePath); // <-- but this works?
                PlayerData saveData = JsonUtility.FromJson<PlayerData>(json);

                // create slot UI
                GameObject slotobj = UnityEngine.Object.Instantiate(slotTemplate, slotGrid);
                var saveslotUi = slotobj.GetComponent<SaveSlotUI>();
                saveslotUi.nametxt.text = saveData.PetName;
                saveslotUi.playtimetxt.text = FormatPlaytime(saveData.TotalPlaytimeSeconds);
                saveslotUi.lastsavedtxt.text = FormatTimestamp(saveData.LastSaveTimestamp);

                // add button to load this save
                Button loadButton = slotobj.GetComponentInChildren<Button>();
                string capturedFileName = fileName; // capture
                loadButton.onClick.AddListener(() => OnLoadClick(fileName));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load save file {fileName}: {e.Message}");
            }
        }
    }
    // called when the load button is clicked
    private void OnLoadClick(string fileName)
    {
        var plrData = SaveHandler.Instance.PlayerDataFromFile(fileName);
        SaveHandler.Instance.LoadSaveData(plrData);
        AreaHandler.Instance.EnterHome();
        
        // enter game
        ingameOverlay.SetActive(true);
        savesScreen.SetActive(false);
        
        CameraHandler.Instance.ToggleScrollerBG(true);
        UIOverlay.Instance.UpdateResourcesAndBal();

        PetHelper.petStateMachine.SetState(PetState.Idle);

        PetHelper.petAnimation.SetBoolParameter("IsSitting",false);
        PetHelper.petAnimation.SetBoolParameter("IsSick",false);

        FurnitureInventoryUI.Instance.UpdateInventoryUI();
    }
    // formats the play time into hours and minutes
    string FormatPlaytime(float seconds)
    {
        int hours = (int)(seconds / 3600);
        int minutes = (int)(seconds % 3600 / 60);
        return $"{hours}h {minutes}m";
    }
    // formats the time stamp
    string FormatTimestamp(long timestamp)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
        return dateTime.ToString("MMM dd, HH:mm");
    }
    PetBreed selectedBreed = PetBreed.Corgi;

    // called externally from a button
    public void SelectBreed(int newBreed)
    {
        selectedBreed = (PetBreed)newBreed;
    }
    // create a new save
    public void NewSave()
    {
        if (petNameInput.text == "")
            return; // cant have empty name
        TextAsset jsonFile = Resources.Load<TextAsset>("Other/defaultsave"); // the file is in Resources/Other/defaultsave.json
        if (debugToggle.isOn)
        {
            PlayerData debugData = new();

            debugData.PetName = petNameInput.text;
            debugData.Breed = selectedBreed;

            FurnitureData[] allData = FurnitureDatabase.GetAllData();
            foreach (FurnitureData data in allData)
                debugData.PlayerInventory.AddItem(data, 1000);
            
            debugData.Balance = 100000;
            debugData.Shampoo = 1000;
            debugData.Food = 1000;

            UIPopups.Instance.PopupInfo(
                "Hey!",
                "Because debug mode is enabled, you start with a bunch of resources and every furniture item in the game, obtainable or not! This can be disabled in settings.",
                "OK"
            );
            // set as new data
            SaveHandler.Instance.currentPlayerData = debugData;
            SaveHandler.Instance.LoadSaveData(debugData);
            SaveHandler.Instance.currentSaveFile = $"save_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";

            PetHelper.petFlagManager.ClearFlags();
            PetHelper.petStateMachine.SetState(PetState.Idle);
            PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;

            // update ui
            UIOverlay.Instance.UpdateResourcesAndBal();
            FurnitureInventoryUI.Instance.UpdateInventoryUI();
            return;
        }
        PlayerData newData = JsonUtility.FromJson<PlayerData>(jsonFile.text);
        newData.PetName = petNameInput.text;
        newData.Breed = selectedBreed;
        newData.Minute = 600;
        TutorialManager.Instance.AskTutorial();
        // give starter items
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
        // set as new data
        SaveHandler.Instance.currentPlayerData = newData;
        SaveHandler.Instance.LoadSaveData(newData);
        SaveHandler.Instance.currentSaveFile = $"save_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";

        PetHelper.petFlagManager.ClearFlags();
        PetHelper.petStateMachine.SetState(PetState.Idle);
        PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
        // update ui
        UIOverlay.Instance.UpdateResourcesAndBal();
        FurnitureInventoryUI.Instance.UpdateInventoryUI();
    }
    // deletes the current save
    public void DeleteCurrentSave()
    {
        UIPopups.Instance.PopupYN(
        "Delete Save?",
        "Are you sure you want to delete this save? This cannot be undone.",
        onYes: () => 
        {
            ingameOverlay.SetActive(false);
            savesScreen.SetActive(true);

            CameraHandler.Instance.ToggleScrollerBG(false);
            
            SaveHandler.Instance.DeleteSave(SaveHandler.Instance.currentSaveFile);
            UIPopups.Instance.CloseAllPopups();
            DisplaySaves();
            Debug.Log("Save deleted");
        },
        onNo: () => 
        {
            ingameMenu.SetActive(true);
        }
    );
        
    }
}