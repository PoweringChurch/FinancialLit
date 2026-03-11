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
        HandleFurniturePlacer();
        HandleWallPlacer();
        HandleInteraction();
        HandleMisc();
    }
    void HandleFurniturePlacer()
    {  
        if (cancel.WasPressedThisFrame())
            FurniturePlacer.Instance.CancelPlacement();
        if (rotate.WasPressedThisFrame())
            FurniturePlacer.Instance.RotateFurniture();
        if (interact.WasPressedThisFrame() && FurniturePlacer.Instance.onPlacement)
            FurniturePlacer.Instance.Place();
        if (setFreemove.WasPressedThisFrame()) 
            FurniturePlacer.Instance.SetFreemove(true);
        else if (setFreemove.WasReleasedThisFrame()) 
            FurniturePlacer.Instance.SetFreemove(false);
        if (raiseFurniture.IsPressed()) 
            FurniturePlacer.Instance.AddYOffset(Time.deltaTime);
        else if (lowerFurniture.IsPressed()) 
            FurniturePlacer.Instance.AddYOffset(-Time.deltaTime);
    }
    void HandleWallPlacer()
    {
        if (!PlayerFlagManager.HasFlag(PlayerFlag.WallPlacement))
            return;
        if (cancel.WasPressedThisFrame())
            WallPlacement.Instance.CancelPlacement();
        if (interact.WasPressedThisFrame())
            WallPlacement.Instance.TryPlaceWall();
    }
    void HandleMisc()
    {
        //setting follow
        var (goalPosition,overInteractableLayer) = UICursor.Instance.CursorToVector3(1);
        if (PlayerFlagManager.HasFlag(PlayerFlag.SetFollow) && interact.WasPressedThisFrame() && overInteractableLayer)
        {
            print("clicked");
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
        if (interact.WasPressedThisFrame())
            Interaction.Instance.HandleClick();
    }
}
