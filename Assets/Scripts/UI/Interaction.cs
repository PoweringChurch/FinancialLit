using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;

public class Interaction : MonoBehaviour
{
    public static Interaction Instance;
    [Header("References")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject hoveringNamePrefab;
    [SerializeField] private Transform menuContainer;
    
    [Header("Settings")]
    [SerializeField] private LayerMask interactableLayer; //furniture and pet
    [SerializeField] private float menuRadius = 120f;
    [SerializeField] private float raycastDistance = 1000f;

    private GameObject currentMenu;
    private GameObject currentHoveringName;
    private Outline currentOutline;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (currentMenu || FurniturePlacer.Instance._objectPrefab) {
            if (currentHoveringName)
            {
                Destroy(currentHoveringName);
                currentHoveringName = null;
            }
            return;
        }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = gameCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, interactableLayer)
        && hit.transform.TryGetComponent(out BaseFunctionality functionality))
        {
            if (!currentHoveringName)
            {
                currentHoveringName = Instantiate(hoveringNamePrefab, menuContainer);
            }
            var nameText = currentHoveringName.GetComponent<TextMeshPro>();
            if (functionality is PetFunctionality)
                nameText.text = PetStats.Instance.PetName;
            else
            {
                var handler = functionality.GetComponent<PlacementHandler>();
                nameText.text = handler.itemName;
            }
            currentHoveringName.transform.position = functionality.transform.position+new Vector3(0,1f,0)-(Camera.main.transform.forward*2);
            currentHoveringName.transform.rotation = Camera.main.transform.rotation;
        }
        else {
            Destroy(currentHoveringName);
            currentHoveringName = null;
        }
    }
    //not yet implemented
    private int currentHitIndex = 0;
    private RaycastHit[] hitBuffer = new RaycastHit[10]; // Adjust size as needed
    private Vector2 lastClickPos;
    private float clickPositionThreshold = 5f; // Pixels

    public void HandleClick()
    {
        bool isOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool hasPlacement = PlayerFlagManager.HasState(PlayerFlag.Placement);

        if (isOverUi && IsPointerOverActionMenu()) return; //because we are over an action
        CloseMenu();
        if (hasPlacement) return; //because we are in placement, so we dont want to bring up action menu while placing something
        if (isOverUi) return; //because we are not in an action menu
        
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = gameCamera.ScreenPointToRay(mousePos);
        
        bool isSamePosition = Vector2.Distance(mousePos, lastClickPos) < clickPositionThreshold;
        if (!isSamePosition)
        {
            currentHitIndex = 0; // Reset index for new position
        }
        lastClickPos = mousePos;
        
        int hitCount = Physics.RaycastNonAlloc(ray, hitBuffer, raycastDistance, interactableLayer);
        
        if (hitCount > 0)
        {
            var validHits = new List<RaycastHit>();
            for (int i = 0; i < hitCount; i++)
            {
                if (hitBuffer[i].transform.TryGetComponent(out BaseFunctionality _))
                {
                    validHits.Add(hitBuffer[i]);
                }
            }
            
            if (validHits.Count > 0)
            {
                // Sort by distance (closest first)
                validHits.Sort((a, b) => a.distance.CompareTo(b.distance));
                
                // Cycle through valid hits
                currentHitIndex = currentHitIndex % validHits.Count;
                RaycastHit selectedHit = validHits[currentHitIndex];
                
                // Increment for next click
                currentHitIndex++;
                
                // Get functionality component
                selectedHit.transform.TryGetComponent(out BaseFunctionality functionality);

                currentOutline = selectedHit.transform.GetComponentInChildren<Outline>();
                if (currentOutline != null)
                {
                    currentOutline.enabled = true;
                }
                ShowMenu(mousePos, functionality);
            }
        }
    }
    private bool IsPointerOverActionMenu()
    {
        if (currentMenu == null) return false;
        
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(currentMenu.transform) || 
                result.gameObject == currentMenu)
            {
                return true;
            }
        }
        
        return false;
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
        if (actionName == "Buy")
        {
            buttonText.text = $"Buy (${price:f2})";
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
        // Offset to start at top (remove the -90f to start at right)
        float startAngle = 90f; //-90f;
        
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
    
    public void CloseMenu()
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