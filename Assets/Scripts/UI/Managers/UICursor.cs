using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class UICursor : MonoBehaviour
{
    public static UICursor Instance;

    private static Vector2 cursorHotspot = new(-1, -1);
    private static CursorMode cursorMode = CursorMode.Auto;

    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] Camera gameCamera;
    
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
    }
    // returns the position of the cursor to vector3
    public (Vector3, bool) CursorToVector3(float targetY)
    {
        bool overInteractableLayer = false;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        // create ray from camera through mouse position
        Ray ray = gameCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out _, 1000, interactableLayer)) overInteractableLayer = true;
        // calculate where ray intersects the target Y plane
        float t = (targetY - ray.origin.y) / ray.direction.y;
        Vector3 targetPos = ray.origin + ray.direction * t;
        return (targetPos, overInteractableLayer);
    }

    public void SetCursor(Texture2D newcursor)
    {
        Cursor.SetCursor(newcursor, cursorHotspot, cursorMode);
    }
}