using UnityEngine;
using UnityEngine.InputSystem;
public class PetBehaviour : MonoBehaviour
{
    private static Vector2 cursorHotspot = Vector2.zero;
    private static CursorMode cursorMode = CursorMode.Auto;

    [SerializeField] Camera gameCamera;
    public static PetBehaviour Instance;

    public Texture2D defaultCursor;
    public Texture2D followingCursor;
    
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (PetStateManager.HasState(PetState.Following))
        {
            var cursorPosition = CursorToVector3(2);
            PetMover.Instance.SetGoalPosition(cursorPosition);
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                PetStateManager.RemoveState(PetState.Following);
                var currentPosition = PetMover.Instance.petModel.transform.position;
                SetCursor(defaultCursor);
                PetMover.Instance.SetGoalPosition(currentPosition);
            }
        }
    }
    //helpers
    Vector3 CursorToVector3(float targetY)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        // Create ray from camera through mouse position
        Ray ray = gameCamera.ScreenPointToRay(mousePos);
        // Calculate where ray intersects the target Y plane
        float t = (targetY - ray.origin.y) / ray.direction.y;
        Vector3 targetPos = ray.origin + ray.direction * t;
        return targetPos;
    }
    public void SetCursor(Texture2D newcursor)
    {
        Cursor.SetCursor(newcursor, cursorHotspot, cursorMode);
    }
}