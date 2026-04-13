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

    private float moveSpeed = 15f;
    private float currentZoom = 10f;
    private float minZoom = 2f;
    private float maxZoom = 20f;
    private float zoomSpeed = 75f;

    public Slider zoomSpeedMultiplier;
    public Slider camSpeedMultiplier;

    private Renderer[] wallRenderers;
    private Renderer[] hideableRenderers;

    void Awake()
    {
        Instance = this;
        RefreshRenderers();
    }
    // toggles the game cameras
    public void ToggleGamecam(bool state)
    {
        gameCamera.enabled = state;
        menuCamera.enabled = !state;
        scrollGameobj.SetActive(!state); 
    }
    // refresh the renderers for the walls and objects, allowing them to become transparent when zoomed in
    public void RefreshRenderers()
    {
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
    // move the camera around, called in the update function
    private Vector3 directions = new Vector3(1, 0, 1);
    private void MoveCamera()
    {
        Vector2 input = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        gameCamera.transform.position += (directions * input.y + gameCamera.transform.right * input.x) * Time.deltaTime * moveSpeed * camSpeedMultiplier.value;
        gameCamera.transform.position = new Vector3(Mathf.Clamp(gameCamera.transform.position.x,-30,10),20,Mathf.Clamp(gameCamera.transform.position.z,-30,10));
    }
    // zoom the camera out and in, called in the update function
    private void ZoomCamera()
    {
        bool isOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (isOverUi) return;
        moveSpeed = 20f * (currentZoom / 10f);
        currentZoom = Mathf.Clamp(currentZoom - InputSystem.actions.FindAction("Zoom").ReadValue<Vector2>().y * zoomSpeed * zoomSpeedMultiplier.value * Time.deltaTime, minZoom, maxZoom);
        if (InputSystem.actions.FindAction("ZoomInKey").IsPressed())
            currentZoom = Mathf.Clamp(currentZoom - 0.1f * zoomSpeed * zoomSpeedMultiplier.value * Time.deltaTime, minZoom, maxZoom);
        else if (InputSystem.actions.FindAction("ZoomOutKey").IsPressed())
            currentZoom = Mathf.Clamp(currentZoom + 0.1f * zoomSpeed * zoomSpeedMultiplier.value * Time.deltaTime, minZoom, maxZoom);
        gameCamera.orthographicSize = currentZoom;
    }
    float hideableMinDistance = 18;
    float minDistance = 6;
    float maxDistance = 20;
    float minAlpha = 0f;
    private void HideObjects()
    {
        float zoomScale = currentZoom / 10f;
        // hide walls
        foreach (Renderer renderer in wallRenderers)
        {
            if (renderer == null) continue;
            float distance = Vector3.Distance(gameCamera.transform.position, renderer.transform.position);
            float t = Mathf.InverseLerp(minDistance / zoomScale, maxDistance / zoomScale, distance); // get amount zoomed in
            float alpha = Mathf.Lerp(0.8f, minAlpha, t); // dither
            foreach (var mat in renderer.materials)
                mat.SetFloat("_Alpha", alpha);
        }
        
        // hide objects
        foreach (Renderer renderer in hideableRenderers)
        {
            if (renderer == null) continue;
            float distance = Vector3.Distance(gameCamera.transform.position, renderer.transform.position);
            renderer.enabled = distance >= hideableMinDistance / zoomScale;
        }
    }
    // helpers
    public bool GameCamEnabled()
    {
        return gameCamera.enabled;
    }
    // get the renderers from tags
    Renderer[] GetRenderersFromTags(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        System.Collections.Generic.List<Renderer> renderers = new System.Collections.Generic.List<Renderer>();
        
        foreach (GameObject obj in objects)
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>());
        
        return renderers.ToArray();
    }
}