using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MiniCamManager : MonoBehaviour
{
    public GameObject miniCamFrame;
    public List<GameObject> activeSecondaryCams = new List<GameObject>();
    private int nextCamToActivate = 0;
    private bool isSecondaryCamOn;

    private Options optionsScript;  // fires off start event
    private MovePlayer movePlayerScript;  // fires off unlocking event
    private PlayerInputActions playerInputActions;

    void Start()
    {
        // all cams start off, we sub to the start game event to activate them
        optionsScript = GameObject.Find("BattingPopup").GetComponent<Options>();
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();

        // event handling
        optionsScript.OnGameStart += StartUpSecondaryCams;
        movePlayerScript.OnPlayerUnlock += DeactivateCameras;

        // player input
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.CycleSecCam.performed += CycleCameras;
    }

    private void StartUpSecondaryCams(object sender, EventArgs e)
    {
        isSecondaryCamOn = true;
        miniCamFrame.SetActive(true);
        CycleCamerasHelper();
    }
    private void CycleCameras(InputAction.CallbackContext context)
    {
        if (!context.performed || !isSecondaryCamOn)
        {
            return;
        }
        CycleCamerasHelper();
    }

    private void CycleCamerasHelper()
    {
        if (isSecondaryCamOn)
        {
            int currentCamIndex = nextCamToActivate == 0 ? (activeSecondaryCams.Count - 1) : nextCamToActivate - 1;
            // deactivate current, activate next
            activeSecondaryCams[currentCamIndex].SetActive(false);
            activeSecondaryCams[nextCamToActivate].SetActive(true);

            nextCamToActivate = (nextCamToActivate + 1) % activeSecondaryCams.Count;
        }
    }

    private void DeactivateCameras(object sender, EventArgs e)
    {
        int currentCamIndex = nextCamToActivate == 0 ? (activeSecondaryCams.Count - 1) : nextCamToActivate - 1;
        activeSecondaryCams[currentCamIndex].SetActive(false);

        // reset for next activation
        isSecondaryCamOn = false;
        miniCamFrame.SetActive(false);
        nextCamToActivate = 0;
    }
}
