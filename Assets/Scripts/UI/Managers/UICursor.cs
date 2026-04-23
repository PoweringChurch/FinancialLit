using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class CursorUtils : MonoBehaviour
{
    public static CursorUtils Instance;

    private static Vector2 cursorHotspot = new(8, 8);
    private static CursorMode cursorMode = CursorMode.Auto;
    private static LayerMask interactableLayer;
    
    public Texture2D defaultCursor;
    public Texture2D followingCursor;
    public Texture2D destroyCursor;
    public Texture2D paintCursor;
    public Texture2D wallCursor;
    public Texture2D doorCursor;
    public Texture2D floorCursor;
    public Texture2D scrapeCursor;


    public void Awake()
    {
        Instance = this;
        interactableLayer = (LayerMask) 6;
    }
    // returns the position of the cursor to vector3
    public static (Vector3, bool) CursorToVector3(float targetY)
    {
        bool overInteractableLayer = false;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        // create ray from camera through mouse position
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out _, 1000, interactableLayer)) 
            overInteractableLayer = true;
        // calculate where ray intersects the target Y plane
        float t = (targetY - ray.origin.y) / ray.direction.y;
        Vector3 targetPos = ray.origin + ray.direction * t;
        return (targetPos, overInteractableLayer);
    }

    public void SetCursor(Texture2D newcursor) =>
        Cursor.SetCursor(newcursor, cursorHotspot, cursorMode);
}