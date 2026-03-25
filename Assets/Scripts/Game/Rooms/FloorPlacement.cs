
/*
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class Floor
{
    // a floor only technically needs to be defined by two vectors
    public Vector3 pos;
    public Vector3 size;
}
// class is modified from WallPlacement
public class FloorPlacement : MonoBehaviour
{
    public Material invalidPlacementMaterial;
    public Material validPlacementMaterial;
    public Transform 
    [HideInInspector] public bool onPlacement = false;

    public GameObject floorPrefab;

    private Ray _ray;
    private RaycastHit _hit;

    // grid placement
    private bool freemove = false;
    private const float cellSize = 2f;
    
    private Vector2 gridOffset = new(1f,1f);
    // keep track of the placed floors
    public List<Floor> placedFloors = new();
    // preview the floor
    public GameObject previewFloor;

    private Vector3 _currentPosition;
    private Vector3? _positionA;

    void Update()
    {
        onPlacement = false;
        // if player is hovering over placement
        _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(_ray, out _hit, 1000f, placementLayerMask))
        {
            // set active on first frame
            _currentPosition = ClampToNearest(_hit.point, cellSize);
            // preview the floor
            if (_positionA.HasValue)
            {
                if (!previewFloor.activeSelf)
                    previewFloor.SetActive(true);
                
                previewFloor.transform.position = (_currentPosition+(Vector3)_positionA)/2;
                previewFloor.transform.localScale = new Vector3(Vector3.Distance(_currentPosition,(Vector3)_positionA)/2, 1, Vector3.Distance(_currentPosition,(Vector3)_positionA)/2);
            }
            else if (!_positionA.HasValue && previewFloor.activeSelf)
                previewFloor.SetActive(false);
            
            onPlacement = true;
        }
    }

    public void TryPlaceFloor()
    {
        // check if cursor over ui
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        // check if the layer is not over placement
        if (!onPlacement) return;
        // set position a's value if it does not have value
        if (_positionA.HasValue)
        {
            // create the new floor with the provided positions
            Floor newFloor = new() { 
                pos = (_currentPosition+_positionA)/2, 
                size = new Vector3(Vector3.Distance(_currentPosition,(Vector3)_positionA)/2, 1, Vector3.Distance(_currentPosition,(Vector3)_positionA)/2) };
            if (!IsFloorValid(newFloor))
                return;
            // create a floor gameobject based on the positions
            GameObject floorObj = Instantiate(floorPrefab, wallHolder);

            floorObj.transform.position = newFloor.pos;
            floorObj.transform.localScale = newFloor.size;

            placedFloors.Add(newFloor);

            // reset and refresh
            _positionA = null;
        }
        else
        {
            _positionA = _currentPosition;
        }
    }
    
    private bool IsFloorValid(Floor floor)
    {
        return true; // temporary, add checks later
    }
}
*/