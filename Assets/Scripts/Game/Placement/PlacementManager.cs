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

    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    // modules 
    public FurniturePlacement Furniture;
    public WallPlacement Wall;
    public FloorPlacement Floor;
    
    public enum Mode { None, Furniture, Wall, Floor }
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
        else if (ActiveMode == Mode.Floor) Floor.Tick(_hit);
    }
    public void SetMode(int mode) => SetMode((Mode)mode);
    public void SetMode(Mode mode)
    {
        Furniture.Cancel(); 
        Wall.Cancel();
        ActiveMode = mode;
        switch (mode)
        {
            case Mode.Wall:
                Wall.SetMode(WallPlacement.Mode.Wall); 
                UICursor.Instance.SetCursor(UICursor.Instance.wallCursor);
                break;
            case Mode.Floor:
                Floor.SetMode(FloorPlacement.Mode.Floor);
                UICursor.Instance.SetCursor(UICursor.Instance.floorCursor);
                break;
            default:
                UICursor.Instance.SetCursor(UICursor.Instance.defaultCursor);
                break;
        }
    }
    public void TryPlace()
    {
        if (ActiveMode == Mode.Furniture) 
            Furniture.TryPlace(placementLayerMask);
        else if (ActiveMode == Mode.Wall)
        {
            if (Wall.CurrentMode == WallPlacement.Mode.Wall || Wall.CurrentMode == WallPlacement.Mode.Door )
                Wall.TryPlace();
            else if (Wall.CurrentMode == WallPlacement.Mode.Destroy)
                Wall.DestroyWall();
            else if (Wall.CurrentMode == WallPlacement.Mode.Paint)
                Wall.PaintWall();
        }
        else if (ActiveMode == Mode.Floor)
        {
            if (Floor.CurrentMode == FloorPlacement.Mode.Floor)
            {
                Floor.TryPlace();
            }
            else if (Floor.CurrentMode == FloorPlacement.Mode.Destroy)
                Floor.DestroyFloor();
        }
    }
    public void CancelPlace()
    {
        Furniture.Cancel();
        Wall.Cancel();
        Floor.Cancel();
    }
    public void LoadHouseData(WallData[] wallData, FurnitureObjectData[] furnitureData, FloorData[] floorData)
    {
        Floor.LoadFloors(floorData);
        Wall.LoadWalls(wallData);
        Furniture.LoadFurniture(furnitureData);
    }
}