using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private GameObject scrollGameobj;
    private float moveSpeed = 20f;
    private float currentZoom = 10f;
    private float minZoom = 2f;
    private float maxZoom = 25f;
    private float zoomSpeed = 100f;

    public Slider zoomSpeedMultiplier;
    public Slider camSpeedMultiplier;

    private Renderer[] wallRenderers;
    private Renderer[] hideableRenderers;

    void Awake()
    {
        Instance = this;
        RefreshRenderers();
    }
    public void ToggleGamecam(bool state)
    {
        gameCamera.enabled = state;
        menuCamera.enabled = !state;
        scrollGameobj.SetActive(!state); 
    }
    public void RefreshRenderers()
    {
        Debug.Log("refreshed");
        wallRenderers = GetRenderersFromTags("Wall");
        hideableRenderers = GetRenderersFromTags("Hideable");
    }
    void Update()
    {
        if (gameCamera != null && gameCamera.enabled)
        {
            MoveCamera();
            ZoomCamera();
            HideObjects();
        }
    }
    private void MoveCamera()
    {
        Vector2 input = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        gameCamera.transform.position += (new Vector3(1, 0, 1) * input.y + gameCamera.transform.right * input.x) * Time.deltaTime * moveSpeed * camSpeedMultiplier.value;
    }
    private void ZoomCamera()
    {
        bool isOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (isOverUi) return;
        moveSpeed = 20f * (currentZoom / 10f);
        currentZoom = Mathf.Clamp(currentZoom - InputSystem.actions.FindAction("Zoom").ReadValue<Vector2>().y * zoomSpeed * zoomSpeedMultiplier.value * Time.deltaTime, minZoom, maxZoom);
        gameCamera.orthographicSize = currentZoom;
    }
    float maxDistance = 40;
    float minDistance = 25;

    float hideableMinDistance = 18;
    float minAlpha = 0.05f;
    private void HideObjects()
    {
        float zoomScale = currentZoom / 10f;

        //fade walls
        foreach (Renderer renderer in wallRenderers)
        {
            if (renderer == null) continue;
            float distance = Vector3.Distance(gameCamera.transform.position, renderer.transform.position);
            float t = Mathf.InverseLerp(minDistance / zoomScale, maxDistance / zoomScale, distance);
            float alpha = Mathf.Lerp(minAlpha, 1f, t);

            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }

        //hide objects
        foreach (Renderer renderer in hideableRenderers)
        {
            if (renderer == null) continue;
            float distance = Vector3.Distance(gameCamera.transform.position, renderer.transform.position);
            renderer.enabled = distance >= hideableMinDistance / zoomScale;
        }
    }
    //helpers
    Renderer[] GetRenderersFromTags(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        System.Collections.Generic.List<Renderer> renderers = new System.Collections.Generic.List<Renderer>();
        
        foreach (GameObject obj in objects)
        {
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>());
        }
        
        return renderers.ToArray();
    }
}