using UnityEngine;
using UnityEngine.InputSystem;
public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance;
    [SerializeField] private Camera gameCamera;
    private float moveSpeed = 20f;
    private float currentZoom = 10f;
    private float minZoom = 2f;
    private float maxZoom = 25f;
    private float zoomSpeed = 100f;

    private Renderer[] wallRenderers;
    void Awake()
    {
        Instance = this;
         GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Wall");
        wallRenderers = new Renderer[wallObjects.Length];
        
        for (int i = 0; i < wallObjects.Length; i++)
        {
            wallRenderers[i] = wallObjects[i].GetComponent<Renderer>();
        }
    }
    void Update()
    {
        if (gameCamera != null && gameCamera.enabled)
        {
            MoveCamera();
            ZoomCamera();
            HideWalls();
        }
    }

    private void MoveCamera()
    {
        Vector2 input = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        gameCamera.transform.position += (new Vector3(1, 0, 1) * input.y + gameCamera.transform.right * input.x) * Time.deltaTime * moveSpeed;
    }

    private void ZoomCamera()
    {
        moveSpeed = 20f * (currentZoom / 10f);
        currentZoom = Mathf.Clamp(currentZoom - InputSystem.actions.FindAction("Zoom").ReadValue<Vector2>().y * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        gameCamera.orthographicSize = currentZoom;
    }

    float maxDistance = 40;
    float minDistance = 30;
    float minAlpha = 0f;

    private void HideWalls()
    {
        // Scale distances with zoom (smaller orthoSize = zoomed in = shorter distances)
        float zoomScale = currentZoom / 10f; // 10f is your reference zoom level
        
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
    }
}