using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class PlayerInputHandler : MonoBehaviour
{
    private InputAction interact;
    private InputAction cancel;
    private InputAction rotate;
    void Awake()
    {
        interact = InputSystem.actions.FindAction("Interact");
        cancel = InputSystem.actions.FindAction("Cancel");
        rotate = InputSystem.actions.FindAction("Rotate");
    }
    void Update()
    {
        HandleFurniturePlacer();
        HandleInteraction();
        HandleMisc();
    }
    void HandleFurniturePlacer()
    {
        bool isOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (cancel.WasPressedThisFrame())
        {
            FurniturePlacer.Instance.CancelPlacement();
        }
        if (rotate.WasPressedThisFrame())
        {
            FurniturePlacer.Instance.RotateFurniture();
        }
        if (interact.WasPressedThisFrame() && FurniturePlacer.Instance.onPlacement)
        {
            FurniturePlacer.Instance.Place();
        }
    }
    void HandleMisc()
    {
        //setting follow
        var (goalPosition,overInteractableLayer) = UIHandler.Instance.CursorHelper.CursorToVector3(1);
        if (PlayerStateManager.HasState(PlayerState.SetFollow) && interact.WasPressedThisFrame() && overInteractableLayer)
        {
            PlayerStateManager.RemoveState(PlayerState.SetFollow);
            PetMover.Instance.SetGoalPosition(goalPosition);
            UIHandler.Instance.CursorHelper.SetCursor(UIHandler.Instance.CursorHelper.defaultCursor);
        }
    }
    void HandleInteraction()
    {
        if (interact.WasPressedThisFrame())
        {
            Interaction.Instance.HandleClick();
        }
    }
}
