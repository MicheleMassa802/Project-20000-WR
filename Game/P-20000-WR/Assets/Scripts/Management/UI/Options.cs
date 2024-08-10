using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System;
using UnityEditor;

public class Options : MonoBehaviour
{
    public enum InputTypes
    {
        MouseAndKeyboard,  // 0
        BatTM  // 1
    }

    public TextMeshProUGUI speedText = null;
    private float speedValue;
    private float minSpeed = 20.0f;
    private float maxSpeedExtra = 85.8f;

    public TextMeshProUGUI wildnessText = null;
    private float wildnessValue;
    private float minWildness = 0.0f;
    private float maxWildnessExtra = 100.0f;

    private InputTypes inputType;
    public bool isRighty;

    public GameObject optionsMenu;
    private MovePlayer movePlayerScript;  // fires off locking/unlocking event

    public event EventHandler OnGameStart;


    private void Start()
    {
        // set default values
        /*
        isRighty = PlayerPrefs.GetInt("isRighty") == 0 ? false : true;
        speedValue = PlayerPrefs.GetInt("speed");
        wildnessValue = PlayerPrefs.GetInt("wildness");
        inputType = PlayerPrefs.GetInt("inputType");
        */

        isRighty = true;
        speedValue = 0.0f;
        wildnessValue = 0.0f;
        inputType = InputTypes.MouseAndKeyboard;
        Debug.Log(isRighty + " - " + speedValue + " - " + wildnessValue);
        PitchingSpeedSetter(speedValue);
        PitchingWildnessSetter(wildnessValue);
        InputTypeSetter((int)inputType);
        
        // event handling
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();
        movePlayerScript.OnPlayerLock += StepOntoPlate;
        movePlayerScript.OnPlayerUnlock += StepOffPlate;

        // deactivate the options menu
        optionsMenu.SetActive(false);
    }

    public void PitchingSpeedSetter(float inputValue)
    {
        speedValue = inputValue * maxSpeedExtra + minSpeed;
        speedText.text = speedValue.ToString("0");  // whole number
        PlayerPrefs.SetInt("speed", (int)speedValue);
    }

    public void PitchingWildnessSetter(float inputValue)
    {
        wildnessValue = inputValue * maxWildnessExtra + minWildness;
        wildnessText.text = wildnessValue.ToString("0");  // whole number
        PlayerPrefs.SetInt("wildness", (int)wildnessValue);
    }

    public void PitchingBallSetter(bool isEzBallSelected)
    {
        Pitcher.easyMode = isEzBallSelected;
    }

    public void InputTypeSetter(int inputTypeValue)
    {
        inputType = (InputTypes)Enum.Parse(typeof(InputTypes), inputTypeValue.ToString());
        PlayerPrefs.SetInt("inputType", (int)inputType);
    }


    #region Short functions triggered by events (not UI)
    private void StepOntoPlate(object sender, EventArgs e) { optionsMenu.SetActive(true); }
    private void StepOffPlate(object sender, EventArgs e) { optionsMenu.SetActive(false); }

    public void StartGame()
    {
        optionsMenu.SetActive(false);
        // Invoke event for start of game
        OnGameStart?.Invoke(this, EventArgs.Empty);
        Debug.Log("PLAYBALL");

    }

    #endregion

    public Dictionary<string, int> GetOptionsMenuInfo()
    {
        Dictionary<string, int> result = new()
        {
            { "PitchSpeed", (int)speedValue },
            { "PitchWildness", (int)wildnessValue },
            { "IsRighty", isRighty ? 1 : 0 },
            { "InputType", (int)inputType }
        };

        return result;
    }
}
