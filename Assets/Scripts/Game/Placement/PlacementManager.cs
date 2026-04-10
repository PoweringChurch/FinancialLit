using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    // shared
    public LayerMask placementLayerMask;
    private Ray _ray;
    private RaycastHit _hit;
    public bool onPlacement = false;

    // modules 
    public FurniturePlacement Furniture;
    public WallPlacement Wall;
    public enum Mode { None, Furniture, Wall }
    public Mode ActiveMode { get; private set; } = Mode.None;

    void Awake() => Instance = this;

    void Update()
    {
        onPlacement = false;
        _ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(_ray, out _hit, 1000f, placementLayerMask)) return;

        onPlacement = true;

        if (ActiveMode == Mode.Furniture) Furniture.Tick(_hit);
        else if (ActiveMode == Mode.Wall)  Wall.Tick(_hit);
    }

    public void SetMode(Mode mode)
    {
        if (ActiveMode == Mode.Furniture) Furniture.Cancel();
        if (ActiveMode == Mode.Wall)      Wall.Cancel();
        if (ActiveMode == Mode.None)      { Furniture.Cancel(); Wall.Cancel();}
        ActiveMode = mode;
    }

    public void TryPlace()
    {
        if (!onPlacement) return;
        if (ActiveMode == Mode.Furniture) Furniture.TryPlace(_hit);
        else if (ActiveMode == Mode.Wall)
        {
            if (!Wall.inDestroy)
                Wall.TryPlace();
            else
                Wall.DestroyWall();
        };
    }
    public void CancelPlace()
    {
        Furniture.Cancel();
        Wall.Cancel();
    }
    public void LoadHouseData(List<WallData> wallData, List<FurnitureObjectData> furnitureData)
    {
        Wall.LoadWalls(wallData);
        Furniture.LoadFurniture(furnitureData);
    }
    public List<FurnitureObjectData> GetPlacedFurniture() => Furniture.GetPlacedFurniture();
    public List<WallData> GetPlacedWalls() => Wall.GetPlacedWalls();

}