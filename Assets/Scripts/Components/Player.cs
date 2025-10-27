using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Camera gameCamera;
    public Camera menuCamera;
    void Start()
    {
        menuCamera.enabled = true;
        gameCamera.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void SetActiveCam(string camname)
    {
        if (camname == "menu")
        {
            menuCamera.enabled = true;
            gameCamera.enabled = false;
        }
        else if (camname == "game")
        {
            gameCamera.enabled = true;
            menuCamera.enabled = false;
        }
    }
}