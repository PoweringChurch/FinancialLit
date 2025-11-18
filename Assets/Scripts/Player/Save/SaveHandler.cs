using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    public static SaveHandler Instance;
    public Transform homeFurnitureTransform;
    public PlayerData currentPlayerData = new();
    public const string DEFAULT_SAVE_FILE = "default.json";
    public string currentSaveFile = "default.json";
    private float sessionStartTime;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        sessionStartTime = Time.time;
    }
    public void SaveGame()
    {
        currentPlayerData.IsNewSave = false;
        //pet stats
        currentPlayerData.PetName = PetStats.Instance.PetName;
        currentPlayerData.Hygiene = PetStats.Instance.Status["hygiene"];
        currentPlayerData.Hunger = PetStats.Instance.Status["hunger"];
        currentPlayerData.Entertainment = PetStats.Instance.Status["entertainment"];
        currentPlayerData.Energy = PetStats.Instance.Status["energy"];

        string displayStatus = "OKAY";

        float hygiene = currentPlayerData.Hygiene;
        float hunger = currentPlayerData.Hunger;
        float entertainment = currentPlayerData.Entertainment;
        float energy = currentPlayerData.Energy;

        float total = hygiene + hunger + entertainment + energy;

        if (total > 3.5) displayStatus = "FINE";
        if (total > 3.7) displayStatus = "GREAT";
        if (total > 3.9) displayStatus = "WONDERFUL";
        if (entertainment < 0.5) displayStatus = "BORED";
        if (entertainment < 0.2) displayStatus = "SAD";
        if (hygiene < 0.3) displayStatus = "DIRTY";
        if (hunger < 0.5) displayStatus = "HUNGRY";
        if (hunger < 0.3) displayStatus = "STARVING";
        if (energy < 0.4) displayStatus = "TIRED";
        if (energy < 0.2) displayStatus = "EXHAUSTED";
        if (total < 0.8) displayStatus = "CRITICAL";

        currentPlayerData.DisplayStatus = displayStatus;

        currentPlayerData.PetPosition = PetMover.Instance.petTransform.position;
        currentPlayerData.PetRotation = PetMover.Instance.petTransform.rotation;

        currentPlayerData.PetFlags = PetFlagManager.CurrentFlags;
        
        // furniture

        List<FurnitureObjectData> placedFurnitureData = new();
        for (int i = 0; i < homeFurnitureTransform.childCount; i++)
        {
            var childTransform = homeFurnitureTransform.GetChild(i);
            var placementHandler = childTransform.GetComponent<PlacementHandler>();
            if (placementHandler == null) continue; // Skip if no PlacementHandler

            FurnitureObjectData newFurnitureObjData = new();
            newFurnitureObjData.position = childTransform.position;
            newFurnitureObjData.rotation = childTransform.rotation;
            newFurnitureObjData.itemName = placementHandler.itemName;

            var childFunctionality = childTransform.GetComponent<BaseFunctionality>();
            if (childFunctionality is FeedingFunctionality feedingFunctionality)
            {
                newFurnitureObjData.isFilled = feedingFunctionality.filled;
            }
            placedFurnitureData.Add(newFurnitureObjData);
        }
        currentPlayerData.PlacedFurniture = placedFurnitureData;

        currentPlayerData.PlayerInventory = InventoryHelper.Instance.GetInventory();

        // resources
        currentPlayerData.Money = PlayerResources.Instance.Money;
        currentPlayerData.Food = PlayerResources.Instance.Food;
        currentPlayerData.Shampoo = PlayerResources.Instance.Shampoo;

        // player stats
        float currentSessionTime = Time.time - sessionStartTime;
        currentPlayerData.TotalPlaytimeSeconds += currentSessionTime;
        currentPlayerData.LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        sessionStartTime = Time.time; //reset for next save
        // save to file
        string json = JsonUtility.ToJson(currentPlayerData, true); // true = pretty print
        string savePath = Application.persistentDataPath + "/" + currentSaveFile;
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
    }
    public void LoadSaved(PlayerData playerData)
    {
        playerData.IsNewSave = false;
        // Pet stats
        PetStats.Instance.SetName(playerData.PetName);
        PetStats.Instance.Status["hygiene"] = playerData.Hygiene;
        PetStats.Instance.Status["hunger"] = playerData.Hunger;
        PetStats.Instance.Status["entertainment"] = playerData.Entertainment;
        PetStats.Instance.Status["energy"] = playerData.Energy;

        PetMover.Instance.agent.Warp(playerData.PetPosition);
        PetMover.Instance.petTransform.rotation = playerData.PetRotation;
        // Pet flags
        PetFlagManager.SetFlags(playerData.PetFlags);

        // Clear existing furniture
        for (int i = homeFurnitureTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(homeFurnitureTransform.GetChild(i).gameObject);
        }

        // Spawn saved furniture
        foreach (var furnitureData in playerData.PlacedFurniture)
        {
            FurnitureData furnitureItem = FurnitureDatabase.GetData(furnitureData.itemName);
            if (furnitureItem == null)
            {
                continue;
            }

            GameObject spawnedFurniture = Instantiate(furnitureItem.prefab, homeFurnitureTransform);
            spawnedFurniture.transform.SetPositionAndRotation(furnitureData.position, furnitureData.rotation);

            // Restore furniture-specific data
            var functionality = spawnedFurniture.GetComponent<BaseFunctionality>();
            if (functionality is FeedingFunctionality feedingFunctionality)
            {
                feedingFunctionality.SetFilled(furnitureData.isFilled);
            }
        }

        // Inventory
        InventoryHelper.Instance.SetInventory(playerData.PlayerInventory);
        InventoryHelper.Instance.Rebuild(); // Rebuild FurnitureData references

        // Resources
        PlayerResources.Instance.SetMoney(playerData.Money);
        PlayerResources.Instance.SetFood(playerData.Food);
        PlayerResources.Instance.SetShampoo(playerData.Shampoo);

        sessionStartTime = Time.time; // reset when loading
    }
    public bool DeleteSave(string fileName)
    {
        string savePath = Application.persistentDataPath + "/" + fileName;
        
        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"Save file {fileName} not found, cannot delete");
            return false;
        }
        
        try
        {
            File.Delete(savePath);
            Debug.Log($"Deleted save file: {savePath}");
            
            // If we deleted the currently active save, reset it
            if (currentSaveFile == fileName)
            {
                currentSaveFile = "default.json";
                currentPlayerData = new PlayerData();
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete save file: {e.Message}");
            return false;
        }
    }
    public PlayerData LoadGameFromFile(string fileName)
    {
        string savePath = Application.persistentDataPath + "/"+ fileName;

        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Save file not found");
            return null;
        }
        currentSaveFile = fileName;
        string json = File.ReadAllText(savePath);

        PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);
        return loadedData;
    }
}
