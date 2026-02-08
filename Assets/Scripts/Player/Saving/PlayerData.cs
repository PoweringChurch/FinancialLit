using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnitureObjectData
{
    public Vector3 position;
    public Quaternion rotation;
    public string itemName; // to know which prefab to spawn

    // furniture-specific data
    public bool isFilled = false; // for food bowl
}

// class that is exported to json and loaded
[System.Serializable]
public class PlayerData
{
    // Pet stats
    public PetBreed Breed;
    public string PetName;
    public float Hygiene;
    public float Entertainment;
    public float Hunger;
    public float Energy;

    public Vector3 PetPosition;
    public Quaternion PetRotation;
    public string DisplayStatus;

    public List<PetFlag> PetFlags; //enum
    // furniture
    public List<FurnitureObjectData> PlacedFurniture;
    public Inventory PlayerInventory;

    // resources
    public float Balance;
    public int Shampoo;
    public int Food;

    //player stats
    public float TotalPlaytimeSeconds;
    public long LastSaveTimestamp;

    public bool IsNewSave;

    public bool VisitedPark;
    public bool VisitedVet;
    public bool VisitedSmartyPets;
    public bool VisitedFurnitureStore;

    // igt
    public int Minute;
    public int Day;
    public int Week;

    public PlayerData()
    {
        // Default values for new game
        PetName = "Pet";
        Hygiene = Entertainment = Hunger = Energy = 100f;
        DisplayStatus = "OKAY";
        PetFlags = new List<PetFlag>();
        PlacedFurniture = new List<FurnitureObjectData>();
        PlayerInventory = new Inventory();
        Minute = 480;
        Balance = 200f;
        Shampoo = 8;
        Food = 8;
        TotalPlaytimeSeconds = 0f;
        LastSaveTimestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        IsNewSave = true;

        VisitedPark = false;
        VisitedVet = false;
        VisitedSmartyPets = false;
        VisitedFurnitureStore = false;
    }
}