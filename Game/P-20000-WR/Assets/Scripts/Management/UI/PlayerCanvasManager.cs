using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCanvasManager : MonoBehaviour
{
    public GameObject battingRange;

    private Options optionsScript;  // fires off start event
    private MovePlayer movePlayerScript;  // fires off unlocking event

    // Start is called before the first frame update
    void Start()
    {
        optionsScript = GameObject.Find("BattingPopup").GetComponent<Options>();
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();

        // event handling
        optionsScript.OnGameStart += ShowRange;
        movePlayerScript.OnPlayerUnlock += HideRange;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // the function handling
    private void ShowRange(object sender, EventArgs e) { battingRange.SetActive(true); }
    private void HideRange(object sender, EventArgs e) { battingRange.SetActive(false); }

    void OnSceneUnloaded(Scene current)
    {
        optionsScript.OnGameStart -= ShowRange;
        movePlayerScript.OnPlayerUnlock -= HideRange;
    }
}


   
