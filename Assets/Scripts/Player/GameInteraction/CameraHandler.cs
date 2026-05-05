using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PrimeTween;
public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance;
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
    
    private Vector3 origin = Vector3.zero;
    private float bounds = 20;

    void Awake()
    {
        Instance = this;
        RefreshRenderers();
    }
    // toggles the scroller bg
    public void ToggleScrollerBG(bool state)
        => scrollGameobj.SetActive(!state); 
    public void SetCameraOrigin(Vector3 to, float newBounds)
    {
        Camera.main.transform.position = new Vector3(to.x, 20, to.z);
        origin = to;
        bounds = newBounds;
    }
    public void RotateCamera(float by)
    {
        float startAngle = 0f;
        float endAngle = 45f;

        Tween.Custom(startAngle, endAngle, duration: 0.5f, ease: Ease.OutExpo, onValueChange: angle =>
        {
            Camera.main.transform.RotateAround(origin, Vector3.up, angle - startAngle);
            startAngle = angle;
        });
    }
    // refresh the renderers for the walls and objects, allowing them to become transparent when zoomed in
    public void RefreshRenderers()
    {
        wallRenderers = GetRenderersFromTags("Wall");
        hideableRenderers = GetRenderersFromTags("Hideable");
    }
    void Update()
    {
        if (Camera.main != null && Camera.main.enabled)
        {
            MoveCamera();
            ZoomCamera();
            HideObjects();
        }
    }
    // move the camera around, called in the update function
    private void MoveCamera()
    {
        var directions = new Vector3 (
            Mathf.Clamp(Camera.main.transform.forward.x, -1, 1), 
            0, 
            Mathf.Clamp(Camera.main.transform.forward.z, -1, 1));
        Vector2 input = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        Camera.main.transform.position += (directions * input.y + Camera.main.transform.right * input.x) * Time.deltaTime * moveSpeed * camSpeedMultiplier.value;
        Camera.main.transform.position = new Vector3(
            Mathf.Clamp(Camera.main.transform.position.x,origin.x-bounds,origin.x+bounds),
            20,
            Mathf.Clamp(Camera.main.transform.position.z,origin.z-bounds,origin.z+bounds));
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
        Camera.main.orthographicSize = currentZoom;
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
            float distance = Vector3.Distance(Camera.main.transform.position, renderer.transform.position);
            float t = Mathf.InverseLerp(minDistance / zoomScale, maxDistance / zoomScale, distance); // get amount zoomed in
            float alpha = Mathf.Lerp(0.8f, minAlpha, t); // dither
            foreach (var mat in renderer.materials)
                mat.SetFloat("_Alpha", alpha);
        }
        
        // hide objects
        foreach (Renderer renderer in hideableRenderers)
        {
            if (renderer == null) continue;
            float distance = Vector3.Distance(Camera.main.transform.position, renderer.transform.position);
            renderer.enabled = distance >= hideableMinDistance / zoomScale;
        }
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