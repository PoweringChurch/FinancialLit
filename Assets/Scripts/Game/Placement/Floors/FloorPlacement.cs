using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class FloorData
{
    public Vector3 position;
    public float length;
    public float width;
    public string floorMaterial;
    public float sellVal;
}

[Serializable]
public class FloorMaterial
{
    public string key;
    public Sprite previewImage;
    public Material material;
}

public class FloorPlacement : MonoBehaviour
{
    public LayerMask floorLayerMask;
    public Transform floorHolder;
    public GameObject floorPrefab;
    
    public Transform floorPreview;
    public Renderer floorRenderer;

    public string selectedMaterial;
    public FloorMaterial[] floorMaterials;
    public float baseCost = 5f;

    private Dictionary<string, Material> floorMaterialDict = new();
    private Vector2 gridOffset = new(1f, 1f);
    
    public enum Mode { Floor, Destroy }
    [SerializeField] private Mode currentMode = Mode.Floor;
    public Mode CurrentMode
    {
        get => currentMode;
        set { OnModeChange(value); currentMode = value; }
    }

    public void OnModeChange(Mode newMode)
    {
        switch (newMode)
        {
            case Mode.Floor:
                UICursor.Instance.SetCursor(UICursor.Instance.floorCursor);
                floorPreview.gameObject.SetActive(true);
                break;
            case Mode.Destroy:
                UICursor.Instance.SetCursor(UICursor.Instance.scrapeCursor);
                floorPreview.gameObject.SetActive(false);
                _positionA = null;
                break;
        }
    }

    public void SetMode(Mode to) => CurrentMode = to;
    public void SetMode(int to) => CurrentMode = (Mode)to;

    private void Awake()
    {
        foreach (FloorMaterial mat in floorMaterials)
            floorMaterialDict.Add(mat.key, mat.material);
    }

    public Vector3 _currentPosition;
    public Vector3? _positionA;
    private const float cellSize = 2f;
    private const float minArea = 1f;

    public void Tick(RaycastHit hit)
    {
        _currentPosition = SnapToGrid(hit.point);
        if (currentMode == Mode.Floor)
        {
            if (_positionA.HasValue)
            {
                floorPreview.gameObject.SetActive(true);
                // show a preview rect between positionA and current
                Vector3 center = (_positionA.Value + _currentPosition) / 2f;
                float rawLength = _currentPosition.x - _positionA.Value.x;
                float rawWidth = _currentPosition.z - _positionA.Value.z;
                float length = Mathf.Abs(rawLength);
                float width  = Mathf.Abs(rawWidth);

                if (length <= 0 || width <= 0)
                {
                    if (length <= 0)
                    {
                        float snapDir = rawLength >= 0 ? cellSize : -cellSize;
                        _currentPosition.x = _positionA.Value.x + snapDir;
                        length = cellSize;
                    }
                    if (width <= 0)
                    {
                        float snapDir = rawWidth >= 0 ? cellSize : -cellSize;
                        _currentPosition.z = _positionA.Value.z + snapDir;
                        width = cellSize;
                    }
                    center = (_positionA.Value + _currentPosition) / 2f;
                }
                floorPreview.position = center;
                floorPreview.localScale = new Vector3(length, 1f, width);

                Vector3 p0 = _positionA.Value;
                Vector3 p1 = _currentPosition;
                // no need to check if area < min cause it wont even render if so
                if (IsOverlapping(p0, p1)) 
                {
                    floorRenderer.material = PlacementManager.Instance.invalidPlacementMaterial; 
                    return;
                }
                    floorRenderer.material = PlacementManager.Instance.validPlacementMaterial; 
            }
            else
            {
                // snap preview to a single cell at cursor
                floorPreview.gameObject.SetActive(false);
            }
        }
    }
    public void Cancel() => _positionA = null;
    public void LoadFloors(FloorData[] floorData)
    {
        // clear existing floors
        for (int i = floorHolder.childCount - 1; i >= 0; i--)
            Destroy(floorHolder.GetChild(i).gameObject);
        if (floorData == null)
            return;
        // spawn saved walls
        foreach (var floor in floorData)
            SpawnFloor(floor);
    }
    public void TryPlace()
    {
        if (_positionA.HasValue)
        {
            Vector3 p0 = _positionA.Value;
            Vector3 p1 = _currentPosition;

            float length = Mathf.Abs(p1.x - p0.x);
            float width  = Mathf.Abs(p1.z - p0.z);

            if (length * width < minArea) return;
            if (IsOverlapping(p0, p1)) return;

            float cost = baseCost * length * width;
            if (!FinancialSpending.Instance.CanAfford(cost)) return;
            FinancialSpending.Instance.Spend(cost);

            FloorData newFloor = new()
            {
                position      = new Vector3(Mathf.Min(p0.x, p1.x), p0.y, Mathf.Min(p0.z, p1.z)),
                length        = length,
                width         = width,
                floorMaterial = selectedMaterial,
                sellVal       = cost * 0.8f
            };

            SpawnFloor(newFloor);
            _positionA = null;
            floorPreview.localScale = new Vector3(cellSize, 1f, cellSize);
        }
        else
        {
            _positionA = _currentPosition;
        }
    }
    private void SpawnFloor(FloorData floor)
    {
        Vector3 center = new(
            floor.position.x + floor.length / 2f,
            floor.position.y,
            floor.position.z + floor.width  / 2f
        );

        GameObject floorObj = Instantiate(floorPrefab, floorHolder);
        floorObj.transform.position   = center;
        floorObj.transform.localScale = new Vector3(floor.length, 1f, floor.width);

        FloorComponent floorComponent = floorObj.GetComponent<FloorComponent>();
        floorComponent.floorData = floor;

        if (floorMaterialDict.TryGetValue(floor.floorMaterial, out Material mat))
        {
            Renderer r    = floorObj.GetComponentInChildren<Renderer>();
            Material[] ms = r.materials;
            ms[0]         = new Material(mat);
            ms[0].mainTextureScale = new Vector2(floor.length/2f, floor.width/2f); 
            r.materials   = ms;
        }
    }

    public void DestroyFloor()
    {
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, floorLayerMask))
        {
            FloorComponent fc = hit.transform.GetComponentInParent<FloorComponent>();
            if (fc == null) return;
            FinancialSpending.Instance.Earn(fc.floorData.sellVal);
            Destroy(fc.gameObject);
            CameraHandler.Instance.RefreshRenderers();
        }
    }

    private bool IsOverlapping(Vector3 p0, Vector3 p1)
    {
        Rect incoming = MakeRect(p0, p1);
        foreach (FloorComponent fc in floorHolder.GetComponentsInChildren<FloorComponent>())
        {
            FloorData d = fc.floorData;
            Rect existing = new(d.position.x, d.position.z, d.length, d.width);
            if (incoming.Overlaps(existing)) return true;
        }
        return false;
    }
    private Rect MakeRect(Vector3 p0, Vector3 p1) => new(
        Mathf.Min(p0.x, p1.x),
        Mathf.Min(p0.z, p1.z),
        Mathf.Abs(p1.x - p0.x),
        Mathf.Abs(p1.z - p0.z)
    );
    private Vector3 SnapToGrid(Vector3 pos) => new(
        Mathf.Round(pos.x / cellSize) * cellSize,
        0.5f,
        Mathf.Round(pos.z / cellSize) * cellSize
    );
    public FloorData[] GetAllFloors() =>
        floorHolder.GetComponentsInChildren<FloorComponent>()
                   .Select(fc => fc.floorData)
                   .ToArray();
}