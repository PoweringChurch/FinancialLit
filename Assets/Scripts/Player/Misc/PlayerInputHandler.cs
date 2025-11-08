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
        HandlePetBehaviour();
        HandleInteraction();
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
            Debug.Log("placed");
            FurniturePlacer.Instance.Place();
        }
    }
    void HandlePetBehaviour()
    {
        if (PetStateManager.HasState(PetState.Following) && interact.WasPressedThisFrame())
        {
            PetBehaviour.Instance.StopFollowing();
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
