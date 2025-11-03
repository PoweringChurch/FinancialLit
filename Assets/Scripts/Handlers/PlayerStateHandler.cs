using UnityEngine;
public enum PlayerState { Menu, Home, Placement, View};
public class PlayerStateHandler : MonoBehaviour
{
    public static PlayerStateHandler Instance;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Camera menuCamera;
    private PlayerState currentState;
    public PlayerState CurrentState => currentState;
    void Awake()
    {
        Instance = this;
    }
    public void SetState(PlayerState state)
    {
        currentState = state;
        switch (state)
        {
            case PlayerState.Menu:
                gameCamera.enabled = false;
                menuCamera.enabled = true;
                break;
            case PlayerState.Home:
            case PlayerState.View:
                gameCamera.enabled = true;
                menuCamera.enabled = false;
                break;
        }
    }

    // Accept int and convert to enum
    public void SetState(int stateInt)
    {
        SetState((PlayerState)stateInt);
    }
}