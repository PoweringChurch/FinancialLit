using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
// this script exclusively handles player input so as to minimize forms of input not being centralized and difficult to manage
public class PlayerInputHandler : MonoBehaviour
{
    // technically not necessary, but makes it a lot more terse
    private InputAction interact;
    private InputAction cancel;
    private InputAction rotate;
    private InputAction raiseFurniture;
    private InputAction lowerFurniture;
    private InputAction setFreemove;
    
    // to be added
    private InputAction bindEsc;

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
        HandlePlacement();
        HandleInteraction();
        HandleMisc();
    }

    void HandlePlacement()
    {
        switch (PlacementManager.Instance.ActiveMode)
        {
            case PlacementManager.Mode.Furniture:
                if (cancel.WasPressedThisFrame())
                    PlacementManager.Instance.CancelPlace();
                if (interact.WasPressedThisFrame())
                    PlacementManager.Instance.TryPlace();
                if (rotate.WasPressedThisFrame())
                    PlacementManager.Instance.Furniture.RotateFurniture();
                if (setFreemove.WasPressedThisFrame())
                    PlacementManager.Instance.Furniture.SetFreemove(true);
                else if (setFreemove.WasReleasedThisFrame())
                    PlacementManager.Instance.Furniture.SetFreemove(false);
                if (raiseFurniture.IsPressed())
                    PlacementManager.Instance.Furniture.AddYOffset(Time.deltaTime);
                else if (lowerFurniture.IsPressed())
                    PlacementManager.Instance.Furniture.AddYOffset(-Time.deltaTime);
                break;

            case PlacementManager.Mode.Wall:
                if (cancel.WasPressedThisFrame())
                    PlacementManager.Instance.CancelPlace();
                if (interact.WasPressedThisFrame())
                    PlacementManager.Instance.TryPlace();
                break;
            case PlacementManager.Mode.Floor:
                if (cancel.WasPressedThisFrame())
                    PlacementManager.Instance.CancelPlace();
                if (interact.WasPressedThisFrame())
                    PlacementManager.Instance.TryPlace();
                break;
            case PlacementManager.Mode.None:
            default:
                break;
        }
    }
    void HandleMisc()
    {
        //setting follow
        var (goalPosition,overInteractableLayer) = UICursor.Instance.CursorToVector3(1);
        if (PlayerFlagManager.HasFlag(PlayerFlag.SetFollow) && interact.WasPressedThisFrame() && overInteractableLayer)
        {
            bool isOverUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (isOverUi)
            {   
                UICursor.Instance.SetCursor(UICursor.Instance.defaultCursor);
                PetHelper.petBehaviour.ActiveBehaviour = Behaviour.Default;
                PlayerFlagManager.RemoveFlag(PlayerFlag.SetFollow);
                return;
            }
            PlayerFlagManager.RemoveFlag(PlayerFlag.SetFollow);
            PetHelper.petMover
            .SetGoalPosition
            (goalPosition);
            UICursor.Instance.SetCursor(UICursor.Instance.defaultCursor);
        }
    }
    void HandleInteraction()
    {
        if (PlacementManager.Instance.ActiveMode != PlacementManager.Mode.None) return;
        if (interact.WasPressedThisFrame())
            Interaction.Instance.HandleClick();
    }
}
