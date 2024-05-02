using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSwinger : MonoBehaviour
{
    private Animator batAnimController;
    private Options optionsScript;  // fires off start event
    private MovePlayer movePlayerScript;  // fires off unlocking event

    // Start is called before the first frame update
    void Start()
    {
        batAnimController = GetComponent<Animator>();

        optionsScript = GameObject.Find("BattingPopup").GetComponent<Options>();
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();

        // event handling
        optionsScript.OnGameStart += PlayAnim;
        movePlayerScript.OnPlayerUnlock += StopAnim;
    }

    // set the bool for swinging side correctly and activate swinging
    private void PlayAnim(object sender, EventArgs e) 
    {
        int rightyInt = PlayerPrefs.GetInt("isRighty");  // updated after player steps in
        batAnimController.SetBool("swinging", true);
        batAnimController.SetBool("righty", rightyInt == 1);   
    }

    private void StopAnim(object sender, EventArgs e)
    {
        batAnimController.SetBool("swinging", false);
    }
}
