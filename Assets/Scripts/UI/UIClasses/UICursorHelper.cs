using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class UICursorHelper
{
    private static Vector2 cursorHotspot = new(-1, -1);
    private static CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] Camera gameCamera;
    public Texture2D defaultCursor;
    public Texture2D followingCursor;
    public (Vector3, bool) CursorToVector3(float targetY)
    {
        bool overInteractableLayer = false;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        // Create ray from camera through mouse position
        Ray ray = gameCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out _, 1000, interactableLayer)) overInteractableLayer = true;
        // Calculate where ray intersects the target Y plane
        float t = (targetY - ray.origin.y) / ray.direction.y;
        Vector3 targetPos = ray.origin + ray.direction * t;
        return (targetPos, overInteractableLayer);
    }

    public void SetCursor(Texture2D newcursor)
    {
        Debug.Log(newcursor.name);
        Cursor.SetCursor(newcursor, cursorHotspot, cursorMode);
    }
}