using UnityEngine;
using UnityEngine.InputSystem;
public class CameraHandler : MonoBehaviour
{
    public static CameraHandler Instance;
    [SerializeField] private Camera gameCamera;
    private float moveSpeed = 20f;
    private float currentZoom = 10f;
    private float minZoom = 5f;
    private float maxZoom = 15f;
    private float zoomSpeed = 100f;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (gameCamera != null && gameCamera.enabled)
        {
            MoveCamera();
            ZoomCamera();
        }
    }

    private void MoveCamera()
    {
        Vector2 input = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        gameCamera.transform.position += (new Vector3(1, 0, 1) * input.y + gameCamera.transform.right * input.x) * Time.deltaTime * moveSpeed;
    }

    private void ZoomCamera()
    {
        currentZoom = Mathf.Clamp(currentZoom - InputSystem.actions.FindAction("Zoom").ReadValue<Vector2>().y * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        gameCamera.orthographicSize = currentZoom;
    }
}