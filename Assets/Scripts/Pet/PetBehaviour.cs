using UnityEngine;
using UnityEngine.InputSystem;
public class PetBehaviour : MonoBehaviour
{
    [SerializeField] Camera gameCamera;
    public static PetBehaviour Instance;
    public bool followingCursor = false;
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (followingCursor)
        {
            var cursorPosition = CursorToVector3(2);
            PetMover.Instance.SetGoalPosition(cursorPosition);
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
}