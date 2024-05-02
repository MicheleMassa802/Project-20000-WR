using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraManager : MonoBehaviour
{

    public Transform orientation;

    private float xRotation;
    private float yRotation;
    
    private PlayerInputActions playerInputActions;


    // Start is called before the first frame update
    void Awake()
    {
        // player input
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerInput = playerInputActions.Player.CamRotation.ReadValue<Vector2>();
        RegisterRotation(playerInput);
    }

    private void RegisterRotation(Vector2 playerInput)
    {
        yRotation += playerInput.x;
        xRotation -= playerInput.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // perform rotations
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);  

    }
}
