using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DraggableItem : MonoBehaviour
{
    public Items itemType;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private GameObject draggedCopy;
    private bool isDragging = false;
    private Camera mainCamera;
    
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        // Check for mouse button down to start drag
        if (Mouse.current.leftButton.wasPressedThisFrame && IsPointerOverUI())
        {
            StartDrag();
        }
        
        // Update drag position while dragging
        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            UpdateDrag();
        }
        
        // End drag when mouse button released
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            EndDrag();
        }
    }
    
    private bool IsPointerOverUI()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos, canvas.worldCamera);
    }
    
    private void StartDrag()
    {
        Debug.Log("Started dragging: " + gameObject.name);
        isDragging = true;
        
        // Create a duplicate
        draggedCopy = Instantiate(gameObject, canvas.transform);
        
        // Set up the copy
        RectTransform copyRect = draggedCopy.GetComponent<RectTransform>();
        copyRect.position = transform.position;
        
        // Add CanvasGroup to the copy for transparency
        CanvasGroup canvasGroup = draggedCopy.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = draggedCopy.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        // Remove the DraggableItem script from the copy so it doesn't interfere
        Destroy(draggedCopy.GetComponent<DraggableItem>());
    }
    
    private void UpdateDrag()
    {
        if (draggedCopy != null)
        {
            // Get mouse position and convert to canvas space
            Vector2 mousePos = Mouse.current.position.ReadValue();
            
            RectTransform copyRect = draggedCopy.GetComponent<RectTransform>();
            
            // Convert screen point to local point in canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePos,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            
            copyRect.localPosition = localPoint;
        }
    }
    
    private void EndDrag()
    {
        Debug.Log("Stopped dragging: " + gameObject.name);
        isDragging = false;
        
        // Check if dropped on a drop zone
        Vector2 mousePos = Mouse.current.position.ReadValue();
        DropZone dropZone = GetDropZoneAtPosition(mousePos);
        
        if (dropZone != null && draggedCopy != null)
        {
            // Notify the drop zone
            dropZone.HandleDrop(itemType);
        }
        
        // Always destroy the copy when letting go
        if (draggedCopy != null)
        {
            Destroy(draggedCopy);
            draggedCopy = null;
        }
    }
    
    private DropZone GetDropZoneAtPosition(Vector2 screenPos)
    {
        GameObject[] dropZoneObjects = GameObject.FindGameObjectsWithTag("DropZone");
        
        foreach (GameObject obj in dropZoneObjects)
        {
            DropZone zone = obj.GetComponent<DropZone>();
            if (zone != null)
            {
                RectTransform zoneRect = zone.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(zoneRect, screenPos, canvas.worldCamera))
                {
                    return zone;
                }
            }
        }
        return null;
    }
}