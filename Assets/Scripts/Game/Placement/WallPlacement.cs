// walls are to placed on a base floor grid
// a single wall is defined by two points
// a single wall cannot be intersecting another wall
// a single wall is double faced in that one side of the wall can have a different material than the other side

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using Unity.Mathematics;

[System.Serializable]
public class WallData
{
    public Vector3 p0;
    public Vector3 p1;
    public int innerMat;
    public int outerMat;
    public float sellVal; // saved in wall info, as its easier to determine sell price at wall creation
}
// class is modified from the furniture placer class
public class WallPlacement : MonoBehaviour
{   

    public LayerMask wallLayerMask;

    public bool inDestroy = false;
    public GameObject wallPrefab;
    public GameObject wallCornerPrefab;
    public Transform wallHolder;

    public Material[] wallMaterials; // defined in the unity editor
    private Material validPlacementMaterial;
    private Material invalidPlacementMaterial;

    // grid placement
    private const float cellSize = 2f;
    
    private Vector2 gridOffset = new(1f,1f);
    // keep track of the placed walls
    public List<WallData> currentWalls = new(); 
    // preview the wall
    public Transform previewWall;
    public Renderer previewWallRenderer;
    WallData previewWallData = new();
    void Awake()
    {
        validPlacementMaterial = Resources.Load<Material>("Materials/Furniture/ValidPlacement");
        invalidPlacementMaterial = Resources.Load<Material>("Materials/Furniture/InvalidPlacement");
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
    public void SetWalls(List<WallData> to)
    {
        currentWalls = to;
    }
    public void DestroyWall()
    {
        var wallRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(wallRay, out RaycastHit hitWall, 1000f, wallLayerMask))
        {
            var wallComponent = hitWall.transform.gameObject.GetComponent<WallComponent>();
            currentWalls.Remove(wallComponent.wallData);
            FinancialSpending.Instance.Earn(wallComponent.wallData.sellVal);
            Destroy(hitWall.transform.parent.gameObject);
        }
    }
    public void EnterDestroy()
    {
        inDestroy = true;
        _positionA = null;
        UICursor.Instance.SetCursor(UICursor.Instance.destroyCursor);
    }
    public void ExitDestroy()
    {
        inDestroy = false;
        _positionA = null;
        UICursor.Instance.SetCursor(UICursor.Instance.defaultCursor);
    }
    public void LoadWalls(List<WallData> wallData)
    {
        currentWalls = wallData;
        // clear existing walls
        for (int i = wallHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(wallHolder.GetChild(i).gameObject);
        }
        // spawn saved walls
        foreach (var wall in currentWalls)
        {
            // create wall
            GameObject wallObj = Instantiate(wallPrefab, wallHolder);
            Transform wallModelTransform = wallObj.transform.Find("WallModel");
            WallComponent wallComponent = wallModelTransform.GetComponent<WallComponent>();
            wallComponent.wallData = wall;
            
            Vector3 wallDir = (wall.p1 - wall.p0).normalized;
            Quaternion wallRotation = Quaternion.LookRotation(wallDir) * Quaternion.Euler(0, 90, 0);

            wallObj.transform.position = (wall.p0 + wall.p1) / 2;
            wallModelTransform.rotation = wallRotation;
            wallModelTransform.localScale = new Vector3(Vector3.Distance(wall.p0, wall.p1) / 2, 1, 1);

            // create corners
            GameObject corner0 = Instantiate(wallCornerPrefab, wallObj.transform);
            corner0.transform.position = wall.p0 + cornerOffset;
            corner0.transform.rotation = wallRotation;
            corner0.transform.Rotate(-90,0,-90);

            GameObject corner1 = Instantiate(wallCornerPrefab, wallObj.transform);
            corner1.transform.position = wall.p1 + cornerOffset;
            corner1.transform.rotation = wallRotation;
            corner1.transform.Rotate(-90,0,-90);

            Renderer wallRenderer = wallModelTransform.gameObject.GetComponentInChildren<Renderer>();
            Renderer corner0Renderer = corner0.GetComponent<Renderer>();
            Renderer corner1Renderer = corner1.GetComponent<Renderer>();

            // wall
            wallRenderer.materials[0] = wallMaterials[wall.innerMat];
            wallRenderer.materials[1] = wallMaterials[wall.outerMat];
            // corner 0
            corner0Renderer.materials[0] = wallMaterials[wall.innerMat];
            corner0Renderer.materials[1] = wallMaterials[wall.outerMat];
            // corner 1
            corner1Renderer.materials[0] = wallMaterials[wall.innerMat];
            corner1Renderer.materials[1] = wallMaterials[wall.outerMat];
        }
        CameraHandler.Instance.RefreshRenderers(); //refresh
    }
    public List<WallData> GetPlacedWalls() => currentWalls;
    
    // store positionA for placement in the future
    private Vector3 _currentPosition;
    private Vector3? _positionA;
    private Vector3 cornerOffset = new(0,0.184f,0);
    private Vector3 offset = new(0,2.5f,0);

    float basecost = 20;

    public void TryPlace()
    {
        // check if cursor over ui
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        // set position a's value if it does not have value
        if (_positionA.HasValue)
        {
            // create the new wall with the provided positions
            WallData newWall = new() { p0 = _currentPosition+offset, p1 = (Vector3)_positionA+offset };
            if (!IsWallValid(newWall))
                return;
            float cost = basecost*Vector3.Distance((Vector3)_positionA, _currentPosition);
            if (!FinancialSpending.Instance.CanAfford(cost))
                return;
            FinancialSpending.Instance.Spend(cost);
            
            // create a wall gameobject based on the positions
            GameObject wallObj = Instantiate(wallPrefab, wallHolder);
            Transform wallModelTransform = wallObj.transform.Find("WallModel");
            Vector3 wallDir = (newWall.p1 - newWall.p0).normalized;
            Quaternion wallRotation = Quaternion.LookRotation(wallDir) * Quaternion.Euler(0, 90, 0);

            wallObj.transform.position = (newWall.p0 + newWall.p1) / 2;
            wallModelTransform.rotation = wallRotation;
            wallModelTransform.localScale = new Vector3(Vector3.Distance(newWall.p0, newWall.p1) / 2, 1, 1);

            GameObject corner0 = Instantiate(wallCornerPrefab, wallObj.transform);
            corner0.transform.position = newWall.p0 + cornerOffset;
            corner0.transform.rotation = wallRotation;
            corner0.transform.Rotate(-90,0,-90);

            GameObject corner1 = Instantiate(wallCornerPrefab, wallObj.transform);
            corner1.transform.position = newWall.p1 + cornerOffset;
            corner1.transform.rotation = wallRotation;
            corner1.transform.Rotate(-90,0,-90);
            currentWalls.Add(newWall);
            
            newWall.innerMat = 2;
            newWall.outerMat = 1;

            newWall.sellVal = cost*0.8f;

            WallComponent wallComponent = wallModelTransform.GetComponent<WallComponent>();
            wallComponent.wallData = newWall;
            // reset and refresh
            _positionA = null;
            CameraHandler.Instance.RefreshRenderers();
        }
        else
        {
            _positionA = _currentPosition;
        }
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

        foreach (WallData wall in currentWalls)
        {
            if (DoIntersect(newWall, wall))
                return false;
        }

        return true;
    }
}