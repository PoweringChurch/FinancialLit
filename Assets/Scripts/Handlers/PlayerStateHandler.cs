using UnityEngine;
using System.Collections.Generic;

public enum PlayerState {Menu, Home, View}; //only one state
public enum PlayerStatus {Shopping, Placement} //multiple statuses
public class PlayerStateHandler : MonoBehaviour
{
    public static PlayerStateHandler Instance;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Camera menuCamera;

    private PlayerState currentState;
    private List<PlayerStatus>  currentStatuses;

    public PlayerState CurrentState => currentState;
    public List<PlayerStatus> CurrentStatuses => currentStatuses;

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
    public void AddStatus(PlayerStatus status)
    {
        currentStatuses.Add(status);
    }
    public void AddStatus(int statusInt)
    {
        AddStatus((PlayerStatus)statusInt);
    }
    public void RemoveStatus(PlayerStatus status)
    {
        currentStatuses.Remove(status);
    }
    public void RemoveStatus(int statusInt)
    {
        RemoveStatus((PlayerStatus)statusInt);
    }
    public bool HasStatus(PlayerStatus status)
    {
        return currentStatuses.Contains(status);
    }
    public bool HasStatus(int statusInt)
    {
        return HasStatus((PlayerStatus)statusInt);
    }
}