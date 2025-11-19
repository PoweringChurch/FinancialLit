using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableItem : MonoBehaviour
{
    public static AudioClip correctSound;
    public static AudioClip incorrectSound;

    public bool template = false;
    public AudioClip correctSFX;
    public AudioClip incorrectSFX;

    public Items itemType;
    private Canvas canvas;
    private RectTransform rectTransform;
    private GameObject draggedCopy;
    private bool isDragging = false;
    
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        if (template)
        {
            incorrectSound = incorrectSFX;
            correctSound = correctSFX;
        }
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
        isDragging = false;
        
        // Check if dropped on a drop zone
        Vector2 mousePos = Mouse.current.position.ReadValue();
        GameObject dropZone = GetDropZoneAtPosition(mousePos);
        
        if (dropZone != null && draggedCopy != null)
        {
            bool isCorrect = OrderHandler.Instance.CheckItem(itemType);
            if (isCorrect)
                UISFXPlayer.Instance.Play(correctSound);
            else
                UISFXPlayer.Instance.Play(incorrectSound);
        }
        // Always destroy the copy when letting go
        if (draggedCopy != null)
        {
            Destroy(draggedCopy);
            draggedCopy = null;
        }
    }
    //really could just return a bool but i dont want to just in case right
    private GameObject GetDropZoneAtPosition(Vector2 screenPos)
    {
        GameObject[] dropZoneObjects = GameObject.FindGameObjectsWithTag("DropZone");
        
        foreach (GameObject obj in dropZoneObjects)
        {
            RectTransform zoneRect = obj.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(zoneRect, screenPos, canvas.worldCamera))
            {
                return obj;
            }
        }
        return null;
    }
}