using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using System;
using System.Collections.Generic;

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
        bool isOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool hasPlacement = PlayerStateManager.HasState(PlayerState.Placement);
        if (isOverUi) return;
        CloseMenu();
        if (hasPlacement) return;
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
        var availableActions = functionality.GetAvailableActions();
        if (availableActions.Count == 0)
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

        Vector2[] buttonPositions = CalculateRadialPositions(availableActions.Count, menuRadius);

        int index = 0;
        foreach (KeyValuePair<string, Action> pair in availableActions)
        {
            CreateButton(pair.Key, buttonPositions[index], pair.Value, currentMenu.transform, functionality.price);
            index++;
        }
    }
    
    void CreateButton(string actionName, Vector2 localPosition, Action action, Transform parent, float price)
    {
        // Instantiate button
        GameObject buttonObj = Instantiate(buttonPrefab, parent);
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        
        // Position button
        buttonRect.anchoredPosition = localPosition;
        
        // Set button text if it has one
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = actionName;
        if (actionName == "Purchase")
        {
            buttonText.text = $"Purchase ({price:f2})";
        }
        // Add click listener
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            // Capture action name to avoid closure iss
            string capturedAction = actionName;
            button.onClick.AddListener(() =>
            {
                action();
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