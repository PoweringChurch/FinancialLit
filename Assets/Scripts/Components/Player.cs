using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Camera gameCamera;
    public Camera menuCamera; 
    public enum PlayerState {
        Menu,
        Game
    };
    private PlayerState currentState;
    void Start()
    {
        menuCamera.enabled = true;
        gameCamera.enabled = false;
        currentState = PlayerState.Menu; //start in menu
    }
    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Game:
                MoveCamera();
                break;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame) // Left mouse button click
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
            }
        }
    }
    public void EnterPlacement()
    {
        //set current state to placement
        //depricated?
    }
    public void EnterGame()
    {
        //set game cam to enabled
        gameCamera.enabled = true;
        menuCamera.enabled = false;
        //set current state to viewing
        currentState = PlayerState.Game;
    }
    public void EnterMenu()
    {
        //set menu cam to enabled
        menuCamera.enabled = true;
        gameCamera.enabled = false;
        //set current state to menu
        currentState = PlayerState.Menu;
    }
    private float camMovespeed = 20f;
    //current nonfunctional vv 
    private float currentZoom = 1f;
    private float minZoom = 0.1f;
    private float maxZoom = 1f;
    private float defaultCamsize = 20f;
    private void MoveCamera()
    {
        var moveVect2 = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        var movement = new Vector3(1,0,1) * moveVect2.y;
        movement += gameCamera.transform.right*moveVect2.x;
        gameCamera.transform.position += movement*Time.deltaTime*camMovespeed;
    }
}