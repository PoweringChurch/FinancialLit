using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class Interact : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform menuContainer;
    
    [Header("Settings")]
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float menuRadius = 150f;
    [SerializeField] private float raycastDistance = 1000f;
    
    private GameObject currentMenu;
    private Outline currentOutline;
    void Update()
    {
        // Check for right click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
        
        // Close menu on right click or escape
        if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseMenu();
        }
    }

    void HandleClick()
    {
        CloseMenu();
        // Don't interact if clicking on UI or in placement
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()
            || PlayerStates.HasStatus(PlayerStatus.Placement))
        {
            return;
        }
        // Get mouse position and create ray
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = gameCamera.ScreenPointToRay(mousePos);
        
        // Raycast to find interactable object
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, interactableLayer))
        {
            // Check if object has functionality component
            if (hit.transform.TryGetComponent(out BaseFunctionality functionality))
            {
                currentOutline = hit.transform.GetComponent<Outline>();
                currentOutline.enabled = true;
                ShowMenu(mousePos, functionality);
            }
        }
    }
    
    void ShowMenu(Vector2 screenPosition, BaseFunctionality functionality)
    {
        // Get available actions
        string[] actions = functionality.GetAvailableActions().ToArray();
        //intercept if shopping, swap to alternate mode
        if (PlayerStates.HasStatus(PlayerStatus.Shopping))
        {
            actions = functionality.GetShoppingActions().ToArray();
        }
        if (actions.Length == 0)
        {
            Debug.LogWarning("No actions available");
            return;
        }
        
        // Create menu container
        currentMenu = new GameObject("RadialMenu");
        currentMenu.transform.SetParent(menuContainer, false);
        
        // Convert screen position to canvas space
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : gameCamera,
            out Vector2 localPoint
        );
        
        RectTransform menuRect = currentMenu.AddComponent<RectTransform>();
        menuRect.anchoredPosition = localPoint;
        
        // Calculate button positions around circle
        Vector2[] buttonPositions = CalculateRadialPositions(actions.Length, menuRadius);
        // Create buttons
        for (int i = 0; i < actions.Length; i++)
        {
            CreateButton(actions[i], buttonPositions[i], functionality, currentMenu.transform);
        }
    }
    
    void CreateButton(string actionName, Vector2 localPosition, BaseFunctionality functionality, Transform parent)
    {
        // Instantiate button
        GameObject buttonObj = Instantiate(buttonPrefab, parent);
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        
        // Position button
        buttonRect.anchoredPosition = localPosition;
        
        // Set button text if it has one
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = actionName;
        }
        
        // Add click listener
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            // Capture action name to avoid closure issues
            string capturedAction = actionName;
            button.onClick.AddListener(() => 
            {
                functionality.InvokeAction(capturedAction);
                CloseMenu();
            });
        }
    }
    
    Vector2[] CalculateRadialPositions(int count, float radius)
    {
        Vector2[] positions = new Vector2[count];
        // Calculate angle between each button
        float angleStep = 360f / count;
        // Offset to start at top (optional - remove the -90f to start at right)
        float startAngle = -90f;
        
        for (int i = 0; i < count; i++)
        {
            float angleDegrees = startAngle + (i * angleStep);
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            
            float x = radius * Mathf.Cos(angleRadians);
            float y = radius * Mathf.Sin(angleRadians);
            
            positions[i] = new Vector2(x, y);
        }
        
        return positions;
    }
    
    void CloseMenu()
    {
        if (currentMenu != null)
        {
            Destroy(currentMenu);
            currentMenu = null;
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }
}