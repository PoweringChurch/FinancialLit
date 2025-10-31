using UnityEngine;
using UnityEngine.InputSystem;
public enum PlayerState {
        Menu,
        Game
    };
public class Player : MonoBehaviour
{
    public Camera gameCamera;
    public Camera menuCamera;
    public Inventory inventory;
    private PlayerState currentState;
    
    private int money;
    public int Money {
        get {
            return money;
        }
    }
    void Start()
    {
        menuCamera.enabled = true;
        gameCamera.enabled = false;
        currentState = PlayerState.Menu; //start in menu
        //debug
        GameObject desk = Resources.Load<GameObject>("Furniture/Desk");
        InventoryItem Desk = new InventoryItem("Desk", desk);
        inventory.AddItem(Desk, 10);
        Debug.Log($"Player now has {inventory.GetItem("Desk").Count} desks.");
    }
    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Game:
                MoveCamera();
                ZoomCamera();
                break;
        }
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
    public bool Spend(int amount)
    {
        post = money - amount;
        if (post < 0)
        {
            return false; //can't buy
        }
        money -= amount;
        return true; //can buy, did buy
    }
    private float camMovespeed = 20f;
    private float currentZoom = 10f;
    private float minZoom = 5f;
    private float maxZoom = 15f;
    private float zoomSpeed = 100f;
    private void MoveCamera()
    {
        var moveVect2 = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        var movement = new Vector3(1, 0, 1) * moveVect2.y;
        movement += gameCamera.transform.right * moveVect2.x;
        gameCamera.transform.position += movement * Time.deltaTime * camMovespeed;
    }
    private void ZoomCamera()
    {
        // Get zoom input (you can bind this to mouse wheel or another action)
        Vector2 scrollInput = InputSystem.actions.FindAction("Zoom").ReadValue<Vector2>();
        // Adjust zoom based on scroll or input
        currentZoom -= scrollInput.y * zoomSpeed * Time.deltaTime;
        // Clamp zoom between min and max
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        // Update camera size (for orthographic camera) or field of view (for perspective)
        gameCamera.orthographicSize = currentZoom;
    }
}
