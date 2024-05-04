using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSwinger : MonoBehaviour
{
    private Animator batAnimController;
    private Options optionsScript;  // fires off start event
    private MovePlayer movePlayerScript;  // fires off unlocking event

    private bool isRighty = true;

    [SerializeField] private bool checkForSwing = false;

    // Start is called before the first frame update
    void Start()
    {
        batAnimController = GetComponent<Animator>();

        optionsScript = GameObject.Find("BattingPopup").GetComponent<Options>();
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();

        // event handling
        optionsScript.OnGameStart += AtBatStart;
        movePlayerScript.OnPlayerUnlock += AtBatEnd;
    }


    private void Update()
    {
        if (checkForSwing)
        {
            if (Input.GetMouseButtonDown(0) || DetectBatClient.batTMSwung)
            {
                // generate a swing of the bat via an animation
                string swingAnim = isRighty ? "BatRighty" : "BatLefty";
                string idleAnim = isRighty ? "PrepRighty" : "PrepLefty";
                batAnimController.Play(swingAnim);
                // immediately play idle to return to idle state after swing
                // (as the transitions have exit time)
                batAnimController.Play(idleAnim);

            }
        }
    }




    private void AtBatStart(object sender, EventArgs e)
    {
        // determine batting side
        int rightyInt = PlayerPrefs.GetInt("isRighty");  // updated after player steps in
        isRighty = rightyInt == 1;

        // go into prep <correct side> animation
        string idleAnim = isRighty ? "PrepRighty" : "PrepLefty";
        batAnimController.Play(idleAnim);

        checkForSwing = true;  // start checking for swings

    }

    private void AtBatEnd(object sender, EventArgs e)
    {
        // stop checking for swing
        checkForSwing = false;
        // go back to idle animation
        batAnimController.Play("Idle");
    }
}
