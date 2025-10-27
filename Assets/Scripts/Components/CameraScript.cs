using UnityEngine;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class CameraScript : MonoBehaviour
{
    private float movespeed = 20f;
    void Update()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        var moveVect2 = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().normalized;
        var movement = new Vector3(1,0,1) * moveVect2.y; //transform.forward * moveVect2.y;
        movement += transform.right*moveVect2.x;
        transform.position += movement*Time.deltaTime*movespeed;
    }
}
