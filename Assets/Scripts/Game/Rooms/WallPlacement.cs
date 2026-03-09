// walls are to placed on a base floor grid
// a single wall is defined by two points
// a single wall cannot be intersecting another wall
// a single wall is "double faced" in that one side of the wall can have a different material than the other side

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class Wall
{
    public Vector3 A;
    public Vector3 B;
    // in the future, should store material of inner and outer sides
}

public class WallPlacement : MonoBehaviour
{
    public static WallPlacement Instance;

    public LayerMask placementLayerMask;
    public Camera gameCamera;

    public GameObject wallPrefab;
    public Transform wallHolder;

    [HideInInspector] public bool onPlacement = false;

    private Ray _ray;
    private RaycastHit _hit;

    private bool freemove = false;
    private const float cellSize = 0.25f;
    private Vector2 gridOffset = new();

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
            if (freemove)
                _currentPosition = new Vector3(_hit.point.x,0,_hit.point.z);
            else
                _currentPosition = ClampToNearest(_hit.point, cellSize);
            onPlacement = true;
        }
    }
    // store positionA for placement in the future
    private Vector3 _currentPosition;
    private Vector3? _positionA;
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
            GameObject newWall = Instantiate(wallPrefab, wallHolder);
            newWall.transform.position = (_currentPosition+(Vector3)_positionA)/2;
            newWall.transform.localScale = new Vector3(Vector3.Distance(_currentPosition,(Vector3)_positionA)/2, 1,1);
            newWall.transform.LookAt(_currentPosition);
            newWall.transform.Rotate(0,-90,0);
            _positionA = null;
        }
        else
        {
            _positionA = _currentPosition;
        }
    }
    private Vector3 ClampToNearest(Vector3 pos, float threshold)
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

        float s = threshold * 0.5f;
        v.x += s + gridOffset.x; // recenter in middle of cells
        v.z += s + gridOffset.y;

        return v;
    }
}