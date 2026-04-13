using System.Collections.Generic;
using UnityEngine;

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

    public List<PetFlag> PetFlags; // enum

    // furniture
    public FurnitureObjectData[] PlacedFurniture;
    public WallData[] PlacedWalls;
    public FloorData[] PlacedFloors;
    public Inventory PlayerInventory;
    // rooms
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
        PlayerInventory = new();
        PetFlags = new();
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