using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class PlayerInputHandler : MonoBehaviour
{
    private InputAction interact;
    private InputAction cancel;
    private InputAction rotate;
    private InputAction raiseFurniture;
    private InputAction lowerFurniture;
    private InputAction setFreemove;
    void Awake()
    {
        interact = InputSystem.actions.FindAction("Interact");
        cancel = InputSystem.actions.FindAction("Cancel");
        rotate = InputSystem.actions.FindAction("Rotate");
        raiseFurniture = InputSystem.actions.FindAction("RaiseFurniture");
        lowerFurniture = InputSystem.actions.FindAction("LowerFurniture");
        setFreemove = InputSystem.actions.FindAction("SetFreemove");
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
        if (setFreemove.WasPressedThisFrame()) FurniturePlacer.Instance.SetFreemove(true);
        else if (setFreemove.WasReleasedThisFrame()) FurniturePlacer.Instance.SetFreemove(false);
        
        if (raiseFurniture.IsPressed()) FurniturePlacer.Instance.AddYOffset(Time.deltaTime);
        else if (lowerFurniture.IsPressed()) FurniturePlacer.Instance.AddYOffset(-Time.deltaTime);
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
