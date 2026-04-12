// walls are to placed on a base floor grid
// a single wall is defined by two points
// a single wall cannot be intersecting another wall
// a single wall is double faced in that one side of the wall can have a different material than the other side

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class WallData
{
    public Vector3 p0;
    public Vector3 p1;
    public string innerMat;
    public string outerMat;
    public float sellVal; // saved in wall info, as its easier to determine sell price at wall creation
    public bool isDoor;
}
[Serializable]
public class WallPaint
{
    public string key;
    public Material material;
}
// class is modified from the furniture placer class
public class WallPlacement : MonoBehaviour
{   
    public LayerMask wallLayerMask;

    // shared
    public Transform wallHolder;
    public WallPaint[] wallPaints; // defined in the unity editor

    public enum Mode {Wall, Door, Paint, Destroy}
    [SerializeField] private Mode currentMode = Mode.Wall;
    public Mode CurrentMode
    {
        get => currentMode;
        set
        {
            OnModeChange(value);
            currentMode = value;
        }
    }

    // wall placement
    public GameObject wallPrefab;
    public GameObject wallCornerPrefab;
    private Dictionary<string, Material> wallMaterialDict = new();
    
    // door placement
    public GameObject doorFramePrefab;

    private Material validPlacementMaterial;
    private Material invalidPlacementMaterial;

    // grid placement
    private const float cellSize = 2f;
    
    private Vector2 gridOffset = new(1f,1f);
    // preview the wall
    public Transform previewWall;
    public Renderer previewWallRenderer;
    WallData previewWallData = new();
    void Awake()
    {
        validPlacementMaterial = Resources.Load<Material>("Materials/Furniture/ValidPlacement");
        invalidPlacementMaterial = Resources.Load<Material>("Materials/Furniture/InvalidPlacement");
        Init();
    }
    void Init()
    {
        foreach (WallPaint wallPaint in wallPaints)
            {
                wallMaterialDict.Add(wallPaint.key, wallPaint.material); // build dictionary
            }
    }
    public void Tick(RaycastHit hit)
    {
        // set active on first frame
        _currentPosition = PlacementUtils.ClampToNearest(hit.point, cellSize, gridOffset);
        // preview the wall
        if (_positionA.HasValue)
        {
            if (!previewWall.gameObject.activeSelf)
                previewWall.gameObject.SetActive(true);
            previewWallRenderer.material = validPlacementMaterial;
            previewWallData.p0 = _currentPosition;
            previewWallData.p1 = (Vector3)_positionA;

            if (!IsWallValid(previewWallData))
                previewWallRenderer.material = invalidPlacementMaterial;
            previewWall.position = (_currentPosition+(Vector3)_positionA)/2;
            previewWall.localScale = new Vector3(Vector3.Distance(_currentPosition,(Vector3)_positionA)/2, 1, 1);
            previewWall.LookAt(_currentPosition);
            previewWall.Rotate(0,-90,0);
        }
        else if (!_positionA.HasValue && previewWall.gameObject.activeSelf)
            previewWall.gameObject.SetActive(false);
    }
    public void DestroyWall()
    {
        var wallRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(wallRay, out RaycastHit hitWall, 1000f, wallLayerMask))
        {
            var wallComponent = hitWall.transform.gameObject.GetComponent<WallComponent>();
            FinancialSpending.Instance.Earn(wallComponent.wallData.sellVal);
            Destroy(hitWall.transform.parent.gameObject);
        }
    }
    public void OnModeChange(Mode newMode)
    {
        _positionA = null;
        switch (newMode)
        {
            case Mode.Wall:
                UICursor.Instance.SetCursor(UICursor.Instance.wallCursor);
                break;
            case Mode.Destroy:
                UICursor.Instance.SetCursor(UICursor.Instance.destroyCursor);
                break;
            case Mode.Paint:
                UICursor.Instance.SetCursor(UICursor.Instance.paintCursor);
                break;
            case Mode.Door:
                UICursor.Instance.SetCursor(UICursor.Instance.doorCursor);
                break;
        };
    }
    public void SetMode(Mode to) => CurrentMode = to;
    public void SetMode(int to) => CurrentMode = (Mode)to;
    public void LoadWalls(WallData[] wallData)
    {
        // clear existing walls
        for (int i = wallHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(wallHolder.GetChild(i).gameObject);
        }
        if (wallData == null)
            return;
        // spawn saved walls
        foreach (var wall in wallData)
        {
            if (wall.isDoor)
                SpawnDoor(wall, wall.sellVal*1.2f);
            else
                SpawnWall(wall, wall.sellVal*1.2f);
        }
        CameraHandler.Instance.RefreshRenderers(); //refresh
    }
    public IEnumerable<WallData> GetAllWalls() => 
        wallHolder.GetComponentsInChildren<WallComponent>()
                .Select(w => w.wallData);
    
    // store positionA for placement in the future
    private Vector3 _currentPosition;
    private Vector3? _positionA;
    private Vector3 cornerOffset = new(0,0.184f,0);
    private Vector3 offset = new(0,2.5f,0);

    float baseCost = 20;
    public void TryPlace()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (_positionA.HasValue)
        {
            WallData newWall = new() 
            { 
                p0 = _currentPosition + offset, 
                p1 = (Vector3)_positionA + offset,
                isDoor = currentMode == Mode.Door,
                innerMat = "default", outerMat = "default"
            };

            if (!IsWallValid(newWall)) return;

            float cost = baseCost * Vector3.Distance((Vector3)_positionA, _currentPosition);
            if (!FinancialSpending.Instance.CanAfford(cost)) return;
            FinancialSpending.Instance.Spend(cost);

            if (newWall.isDoor)
                SpawnDoor(newWall, cost);
            else
                SpawnWall(newWall, cost);

            _positionA = null;
            CameraHandler.Instance.RefreshRenderers();
        }
        else
        {
            _positionA = _currentPosition;
        }
    }
    private void SpawnDoor(WallData wall, float cost = -1)
    {
        Vector3 mid = (wall.p0 + wall.p1) / 2;
        Vector3 wallDir = (wall.p1 - wall.p0).normalized;
        Quaternion wallRotation = Quaternion.LookRotation(wallDir) * Quaternion.Euler(0, 90, 0);

        // place door frame centered on midpoint, fixed width
        GameObject doorObj = Instantiate(doorFramePrefab, wallHolder);
        Transform doorModelTransform = doorObj.transform.Find("DoorFrameModel");
        var doorRenderer = doorModelTransform.GetComponentInChildren<Renderer>();
        doorObj.transform.position = mid;
        doorObj.transform.rotation = wallRotation;

        // fill wall segments on either side of the door
        Vector3 doorEdgeLeft  = mid - wallDir; // * (wall.doorWidth / 2)
        Vector3 doorEdgeRight = mid + wallDir; // * (wall.doorWidth / 2)

        if (wall.innerMat == null)
            wall.innerMat = "default";
        if (wall.outerMat == null)
            wall.outerMat = "default";
        
        Material[] mats = doorRenderer.materials;
        mats[0] = wallMaterialDict[wall.innerMat];
        mats[1] = wallMaterialDict[wall.outerMat];
        doorRenderer.materials = mats;

        if (Vector3.Distance(wall.p0, doorEdgeLeft) > 0.1f)
        {
            WallData leftWall = new() { p0 = wall.p0, p1 = doorEdgeLeft, innerMat = wall.innerMat, outerMat = wall.outerMat };
            SpawnWall(leftWall, 0); // cost already paid
        }
        if (Vector3.Distance(wall.p1, doorEdgeRight) > 0.1f)
        {
            WallData rightWall = new() { p0 = doorEdgeRight, p1 = wall.p1, innerMat = wall.innerMat, outerMat = wall.outerMat };
            SpawnWall(rightWall, 0);
        }
        if (cost != -1)
            wall.sellVal = cost * 0.8f;
        doorModelTransform.GetComponent<WallComponent>().wallData = wall;
    }
    private void SpawnWall(WallData wall, float cost = -1)
    {
        GameObject wallObj = Instantiate(wallPrefab, wallHolder);

        if (wall.innerMat == null)
            wall.innerMat = "default";
        if (wall.outerMat == null)
            wall.outerMat = "default";
        
        Transform wallModelTransform = wallObj.transform.Find("WallModel");

        Vector3 wallDir = (wall.p1 - wall.p0).normalized;
        Quaternion wallRotation = Quaternion.LookRotation(wallDir) * Quaternion.Euler(0, 90, 0);

        wallObj.transform.position = (wall.p0 + wall.p1) / 2;
        wallModelTransform.rotation = wallRotation;
        wallModelTransform.localScale = new Vector3(Vector3.Distance(wall.p0, wall.p1) / 2, 1, 1);

        SpawnCorners(wallObj, wall, wallRotation);

        var wallRenderer = wallModelTransform.GetComponentInChildren<Renderer>();
        var cornerRenderers = wallObj.GetComponentsInChildren<Renderer>();

        Material[] mats = wallRenderer.materials;
        mats[0] = wallMaterialDict[wall.innerMat];
        mats[1] = wallMaterialDict[wall.outerMat];
        wallRenderer.materials = mats;

        foreach (var cornerRenderer in cornerRenderers)
        {
            if (cornerRenderer == wallRenderer) continue;
            Material[] cornerMats = cornerRenderer.materials;
            cornerMats[0] = wallMaterialDict[wall.innerMat];
            cornerMats[1] = wallMaterialDict[wall.outerMat];
            cornerRenderer.materials = cornerMats;
        }

        if (cost != -1)
            wall.sellVal = cost * 0.8f;
        wallModelTransform.GetComponent<WallComponent>().wallData = wall;
    }
    public string selectedPaint = "purple";
    public void PaintWall()
    {
        if (selectedPaint == null) return;
        var wallRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(wallRay, out RaycastHit hitWall, 1000f, wallLayerMask))
        {
            var wallComponent = hitWall.transform.gameObject.GetComponent<WallComponent>();
            var wallRenderer = hitWall.transform.GetComponentInChildren<Renderer>();
            var cornerRenderers = hitWall.transform.parent.GetComponentsInChildren<Renderer>();

            Vector3 wallOutward = hitWall.transform.forward;
            Vector3 toCamera = Camera.main.transform.position - hitWall.point;
            bool isOuter = Vector3.Dot(wallOutward, toCamera) > 0;

            int wallMatIndex = isOuter ? 1 : 0;

            if (isOuter) wallComponent.wallData.outerMat = selectedPaint;
            else wallComponent.wallData.innerMat = selectedPaint;

            Material[] mats = wallRenderer.materials;
            mats[wallMatIndex] = wallMaterialDict[selectedPaint];
            wallRenderer.materials = mats;

            foreach (var cornerRenderer in cornerRenderers)
            {
                if (cornerRenderer == wallRenderer) continue;
                Material[] cornerMats = cornerRenderer.materials;
                cornerMats[wallMatIndex] = wallMaterialDict[selectedPaint];
                cornerRenderer.materials = cornerMats;
            }
        }
    }
    private void SpawnCorners(GameObject parent, WallData wall, Quaternion wallRotation)
    {
        GameObject c0 = Instantiate(wallCornerPrefab, parent.transform);
        c0.transform.position = wall.p0 + cornerOffset;
        c0.transform.rotation = wallRotation;
        c0.transform.Rotate(-90, 0, -90);

        var c0Renderer = c0.GetComponent<Renderer>();
        Material[] c0Mats = c0Renderer.materials;
        c0Mats[1] = wallMaterialDict[wall.innerMat];
        c0Mats[0] = wallMaterialDict[wall.outerMat];
        c0Renderer.materials = c0Mats;

        GameObject c1 = Instantiate(wallCornerPrefab, parent.transform);
        c1.transform.position = wall.p1 + cornerOffset;
        c1.transform.rotation = wallRotation;
        c1.transform.Rotate(-90, 0, -90);

        var c1Renderer = c0.GetComponent<Renderer>();
        Material[] c1Mats = c1Renderer.materials;
        c1Mats[1] = wallMaterialDict[wall.innerMat];
        c1Mats[0] = wallMaterialDict[wall.outerMat];
        c1Renderer.materials = c0Mats;
    }
    public void Cancel()
    {
        _positionA = null;
    }
    // approach from https://www.geeksforgeeks.org/dsa/check-if-two-given-line-segments-intersect/
    private bool OnSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        return (q.x <= Math.Max(p.x, r.x) && 
                q.x >= Math.Min(p.x, r.x) &&
                q.z <= Math.Max(p.z, r.z) && 
                q.z >= Math.Min(p.z, r.z));
    }
    // function to find orientation of ordered triplet (p, q, r)
    // 0 --> p, q and r are collinear
    // 1 --> clockwise
    // 2 --> counterclockwise
    public int Orientation(Vector3 p, Vector3 q, Vector3 r) {
        float val = (q.z - p.z) * (r.x - q.x) -
                  (q.x - p.x) * (r.z - q.z);
        // collinear
        if (val == 0) return 0;
        // clock or counterclock wise
        // 1 for cw, 2 for ccw
        return (val > 0) ? 1 : 2;
    }
    public bool DoIntersect(WallData a, WallData b)
    {
        // allows for walls to be placed on corners
        if (a.p0 == b.p0 || a.p0 == b.p1 || a.p1 == b.p0 || a.p1 == b.p1)
            return false;
        
        int o1 = Orientation(a.p0, a.p1, b.p0);
        int o2 = Orientation(a.p0, a.p1, b.p1);
        int o3 = Orientation(b.p0, b.p1, a.p0);
        int o4 = Orientation(b.p0, b.p1, a.p1);

        // general case
        /* IF p1, q1, and p2 orientations (o1) DOES NOT EQUAL p1, q1, and q2 orientations (o2) 
        AND p2, q2, and p1 orientations (o3) DOES NOT EQUAL p2, q2, and q1 orientations (o4)
        THEN the lines intersect */
        if (o1 != o2 && o3 != o4)
            return true;

        // collinear special cases
        if (o1 == 0 && OnSegment(a.p0, b.p0, a.p1)) return true;
        if (o2 == 0 && OnSegment(a.p0, b.p1, a.p1)) return true;
        if (o3 == 0 && OnSegment(b.p0, a.p0, b.p1)) return true;
        if (o4 == 0 && OnSegment(b.p0, a.p1, b.p1)) return true;

        return false;
    }
    public bool IsWallValid(WallData newWall)
    {
        // wall must have nonzero length
        if (Vector3.Distance(newWall.p0, newWall.p1) < 0.001f)
            return false;

        foreach (WallData wall in GetAllWalls())
        {
            if (DoIntersect(newWall, wall))
                return false;
        }

        return true;
    }
}