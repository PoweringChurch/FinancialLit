// walls are to placed on a base floor grid
// a single wall is defined by two points
// a single wall cannot be intersecting another wall
// a single wall is double faced in that one side of the wall can have a different material than the other side

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
public class Wall
{
    public Vector3 p0;
    public Vector3 p1;
}

// class is modified from the furniture placer class
public class WallPlacement : MonoBehaviour
{
    public static WallPlacement Instance;

    public LayerMask placementLayerMask;
    public Camera gameCamera;

    public GameObject wallPrefab;
    public GameObject wallCornerPrefab;
    public Transform wallHolder;

    public Material invalidPlacementMaterial;
    public Material validPlacementMaterial;

    [HideInInspector] public bool onPlacement = false;

    private Ray _ray;
    private RaycastHit _hit;

    // grid placement
    private bool freemove = false;
    private const float cellSize = 2f;
    
    private Vector2 gridOffset = new(1f,1f);
    // keep track of the placed walls
    public List<Wall> placedWalls = new();
    // preview the wall
    public GameObject previewWall;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        onPlacement = false;
        // if player is hovering over placement
        _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(_ray, out _hit, 1000f, placementLayerMask))
        {
            // set active on first frame
            _currentPosition = ClampToNearest(_hit.point, cellSize);
            // preview the wall
            if (_positionA.HasValue)
            {
                if (!previewWall.activeSelf)
                    previewWall.SetActive(true);
                previewWall.transform.position = (_currentPosition+(Vector3)_positionA)/2;
                previewWall.transform.localScale = new Vector3(Vector3.Distance(_currentPosition,(Vector3)_positionA)/2, 1, 1);
                previewWall.transform.LookAt(_currentPosition);
                previewWall.transform.Rotate(0,-90,0);
            }
            else if (!_positionA.HasValue && previewWall.activeSelf)
                previewWall.SetActive(false);
            
            onPlacement = true;
        }
    }
    // store positionA for placement in the future
    private Vector3 _currentPosition;
    private Vector3? _positionA;
    private Vector3 cornerOffset = new(0,0.184f,0);
    private Vector3 offset = new(0,2.5f,0);
    public void TryPlaceWall()
    {
        // check if cursor over ui
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        // check if the layer is not over placement
        if (!onPlacement) return;
        // set position a's value if it does not have value
        if (_positionA.HasValue)
        {
            // create the new wall with the provided positions
            Wall newWall = new() { p0 = _currentPosition+offset, p1 = (Vector3)_positionA+offset };
            if (!IsWallValid(newWall))
                return;
            // create a wall gameobject based on the positions
            GameObject wallObj = Instantiate(wallPrefab, wallHolder);
            Vector3 wallDir = (newWall.p1 - newWall.p0).normalized;
            Quaternion wallRotation = Quaternion.LookRotation(wallDir) * Quaternion.Euler(0, 90, 0);

            wallObj.transform.position = (newWall.p0 + newWall.p1) / 2;
            wallObj.transform.rotation = wallRotation;
            wallObj.transform.localScale = new Vector3(Vector3.Distance(newWall.p0, newWall.p1) / 2, 1, 1);

            GameObject corner0 = Instantiate(wallCornerPrefab, wallHolder);
            corner0.transform.position = newWall.p0 + cornerOffset;
            corner0.transform.rotation = wallRotation;
            corner0.transform.Rotate(-90,0,-90);
            GameObject corner1 = Instantiate(wallCornerPrefab, wallHolder);
            corner1.transform.position = newWall.p1 + cornerOffset;
            corner1.transform.rotation = wallRotation;
            corner1.transform.Rotate(-90,0,-90);
            placedWalls.Add(newWall);
            

            // reset and refresh
            _positionA = null;
            CameraHandler.Instance.RefreshRenderers();
        }
        else
        {
            _positionA = _currentPosition;
        }
    }
    public void CancelPlacement()
    {
        _positionA = null;
    }
    private Vector3 ClampToNearest(Vector3 pos, float threshold)
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

        float s = threshold * 0.5f;
        v.x += s + gridOffset.x; // recenter in middle of cells
        v.y = 0f;
        v.z += s + gridOffset.y;

        return v;
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
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    public int Orientation(Vector3 p, Vector3 q, Vector3 r) {
        float val = (q.z - p.z) * (r.x - q.x) -
                  (q.x - p.x) * (r.z - q.z);
        // collinear
        if (val == 0) return 0;
        // clock or counterclock wise
        // 1 for clockwise, 2 for counterclockwise
        return (val > 0) ? 1 : 2;
    }
    public bool DoIntersect(Wall a, Wall b)
    {
        if (a.p0 == b.p0 || a.p0 == b.p1 || a.p1 == b.p0 || a.p1 == b.p1)
            return false;
        
        int o1 = Orientation(a.p0, a.p1, b.p0);
        int o2 = Orientation(a.p0, a.p1, b.p1);
        int o3 = Orientation(b.p0, b.p1, a.p0);
        int o4 = Orientation(b.p0, b.p1, a.p1);

        // general case
        if (o1 != o2 && o3 != o4)
            return true;

        
        // collinear special cases
        if (o1 == 0 && OnSegment(a.p0, b.p0, a.p1)) return true;
        if (o2 == 0 && OnSegment(a.p0, b.p1, a.p1)) return true;
        if (o3 == 0 && OnSegment(b.p0, a.p0, b.p1)) return true;
        if (o4 == 0 && OnSegment(b.p0, a.p1, b.p1)) return true;
        

        return false;
    }
    public bool IsWallValid(Wall newWall)
    {
        // wall must have nonzero length
        if (Vector3.Distance(newWall.p0, newWall.p1) < 0.001f)
            return false;

        foreach (Wall wall in placedWalls)
        {
            if (DoIntersect(newWall, wall))
                return false;
        }

        return true;
    }
}