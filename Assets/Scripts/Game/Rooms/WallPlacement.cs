// walls are to placed on a base floor grid
// a single wall is defined by two points
// a single wall cannot be intersecting another wall
// a single wall is "double faced" in that one side of the wall can have a different material than the other side

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;

public class Wall
{
    public Vector3 A;
    public Vector3 B;
    // in the future, should store material of inner and outer sides
}

public class WallPlacement : MonoBehaviour
{

    public LayerMask placementLayerMask;
    public Camera gameCamera;
    [HideInInspector] public bool onPlacement = false;
    void Update()
    {
        onPlacement = false;
        // if player is hovering over placement
        _ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if ( && Physics.Raycast(_ray, out _hit, 1000f, placementLayerMask))
        {
            // set active on first frame
            if (freemove)
                _currentPosition = new Vector3(_hit.point.x,currentyoffset,_hit.point.z);
            else
                _currentPosition = _ClampToNearest(_hit.point, cellSize);
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
        if (!_positionA.HasValue)
        {
            _positionA = _currentPosition;
        }
        else
        {
            // create the new wall with the provided positions
            Wall newWall = new();
            newWall.A = _positionA;
            newWall.B = _currentPosition;
            return newWall;
        }
    }
}