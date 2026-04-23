using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum PetBreed {Corgi, Cur, Pug}
public class SaveHandler : MonoBehaviour
{
    public static SaveHandler Instance;
    public Transform homeFurnitureTransform;
    public PlayerData currentPlayerData = new();
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
        currentPlayerData.Breed = PetHelper.petStats.breed;
        currentPlayerData.PetName = PetHelper.petStats.petName;

        currentPlayerData.Hygiene = PetHelper.petStats.Status["hygiene"];
        currentPlayerData.Hunger = PetHelper.petStats.Status["hunger"];
        currentPlayerData.Entertainment = PetHelper.petStats.Status["entertainment"];
        currentPlayerData.Energy = PetHelper.petStats.Status["energy"];

        currentPlayerData.PetPosition = PetHelper.petMover.petTransform.position;
        currentPlayerData.PetRotation = PetHelper.petMover.petTransform.rotation;

        currentPlayerData.PetFlags = PetHelper.petFlagManager.CurrentFlags;
        
        // save house
        currentPlayerData.PlacedFurniture = PlacementManager.Instance.Furniture.GetPlacedFurniture();
        currentPlayerData.PlacedWalls = PlacementManager.Instance.Wall.GetAllWalls().ToArray();
        currentPlayerData.PlacedFloors = PlacementManager.Instance.Floor.GetAllFloors();
        currentPlayerData.PlayerInventory = InventoryHelper.Instance.GetInventory();
        // resources
        currentPlayerData.Balance = FinancialSpending.Instance.Balance;
        currentPlayerData.Food = PlayerResources.Instance.Food;
        currentPlayerData.Shampoo = PlayerResources.Instance.Shampoo;

        // igt
        currentPlayerData.Minute = GameTime.Instance.Minute;
        currentPlayerData.Day = GameTime.Instance.Day;
        currentPlayerData.Week = GameTime.Instance.Week;

        // player stats
        float currentSessionTime = Time.time - sessionStartTime;
        currentPlayerData.TotalPlaytimeSeconds += currentSessionTime;
        currentPlayerData.LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        sessionStartTime = Time.time; //reset for next save
        // save to file
        string json = JsonUtility.ToJson(currentPlayerData, true); // true here pretty print
        string savePath = Application.persistentDataPath + "/" + currentSaveFile;
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
    }

    public GameObject corgiPrefab;
    public GameObject curPrefab;
    public GameObject pugPrefab;

    public Transform gameSpace;
    public void LoadSaveData(PlayerData playerData)
    {
        playerData.IsNewSave = false;
        // if there is a pet
        if (PetHelper.CurrentActivePet != null)
            Destroy(PetHelper.CurrentActivePet); // destroy it and
        GameObject dog = null;
        // load the new pet
        switch (playerData.Breed)
        {
            case PetBreed.Corgi:
                dog = Instantiate(corgiPrefab,gameSpace);
                break;
            case PetBreed.Cur:
                dog = Instantiate(curPrefab,gameSpace);
                break;
            case PetBreed.Pug:
                dog = Instantiate(pugPrefab,gameSpace);
                break;
        }
        if (!dog) return;
        PetHelper.CurrentActivePet = dog;
        // pet stats
        PetHelper.petStats.petName = playerData.PetName;
        PetHelper.petStats.breed = playerData.Breed;

        PetHelper.petStats.Status["hygiene"] = playerData.Hygiene;
        PetHelper.petStats.Status["hunger"] = playerData.Hunger;
        PetHelper.petStats.Status["entertainment"] = playerData.Entertainment;
        PetHelper.petStats.Status["energy"] = playerData.Energy;

        PetHelper.petMover.agent.Warp(playerData.PetPosition);
        PetHelper.petMover.petTransform.rotation = playerData.PetRotation;
        // pet flags
        PetHelper.petFlagManager.SetFlags(playerData.PetFlags);
        // enter home
        AreaHandler.Instance.EnterHome();
        // load house
        PlacementManager.Instance.LoadHouseData(playerData.PlacedWalls, playerData.PlacedFurniture, playerData.PlacedFloors);
        // inventory
        InventoryHelper.Instance.SetInventory(playerData.PlayerInventory);
        InventoryHelper.Instance.Rebuild(); // Rebuild FurnitureData references

        // igt
        GameTime.Instance.SetTime(playerData.Minute,playerData.Day,playerData.Week);
        // resources
        FinancialSpending.Instance.SetBalance(playerData.Balance);
        PlayerResources.Instance.SetFood(playerData.Food);
        PlayerResources.Instance.SetShampoo(playerData.Shampoo);

        sessionStartTime = Time.time; // reset when loading
    }
    // delete a save by file name
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
            
            // if deleted the currently active save, reset it (should happen but just in case)
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
    // load player data from a file name
    public PlayerData PlayerDataFromFile(string fileName)
    {
        string savePath = Application.persistentDataPath + "/"+ fileName;
        // ensure the fle exists
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
