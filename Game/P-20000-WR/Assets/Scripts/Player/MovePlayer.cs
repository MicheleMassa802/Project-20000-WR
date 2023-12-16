using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{

    [SerializeField] bool isLocked;
    [SerializeField] bool isInLockedZone;

    public Transform orientation;

    private Rigidbody playerRB;
    private Vector3 moveDirection = Vector3.zero;
    private const float SPEED = 400;

    private PlayerInputActions playerInputActions;

    public event EventHandler OnPlayerLock;
    public event EventHandler OnPlayerUnlock;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody>();

        // player input
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.BattingLock.performed += ToggleBattingLock;

    }


    void Update()
    {
        Vector2 playerInput = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Move(playerInput);
    }

    #region Player Input

    public void Move(Vector2 playerInput)
    {
        if (isLocked)
        {
            // no movement possible
            return;
        }

        // otw, move player according to input
        moveDirection = orientation.forward * playerInput.y + orientation.right * playerInput.x;
        playerRB.AddForce(moveDirection.normalized * SPEED, ForceMode.Force);
    }

    public void ToggleBattingLock(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (isInLockedZone && isLocked)
        {
            // unlock
            isLocked = false;
            OnPlayerUnlock?.Invoke(this, EventArgs.Empty);
        }
        else if (isInLockedZone && !isLocked)
        {
            // lock
            isLocked = true;
            OnPlayerLock?.Invoke(this, EventArgs.Empty);
        }
        // otw ignore (not in locked zone)
    }
    #endregion


    #region Triggers && World Interaction

    private void OnTriggerEnter(Collider other) {  HandleBattingBoxEnter(other);  }
    private void OnTriggerStay(Collider other) { HandleBattingBoxEnter(other); }
    private void OnTriggerExit(Collider other) { HandleBattingBoxExit(other); }


    private void HandleBattingBoxEnter(Collider other)
    {
        // when triggering the locked zone for batting
        // listen for player input
        if (other.CompareTag("BatBox") && !isInLockedZone)
        {
            isInLockedZone = true;
        }
    }

    private void HandleBattingBoxExit(Collider other)
    {
        // when triggering the locked zone for batting
        // listen for player input
        if (other.CompareTag("BatBox") && isInLockedZone)
        {
            isInLockedZone = false;
        }
    }
    #endregion


}
