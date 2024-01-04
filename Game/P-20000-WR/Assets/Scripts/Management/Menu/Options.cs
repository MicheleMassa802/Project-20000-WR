using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class Options : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;

    public TextMeshProUGUI speedText = null;
    private float speedValue;
    private float minSpeed = 20.0f;
    private float maxSpeedExtra = 85.8f;
    public TextMeshProUGUI wildnessText = null;
    private float wildnessValue;
    private float minWildness = 0.0f;
    private float maxWildnessExtra = 100.0f;

    public bool isRighty;


    public void Start()
    {
        // set default values
        /*
        isRighty = PlayerPrefs.GetInt("isRighty") == 0 ? false : true;
        speedValue = PlayerPrefs.GetInt("speed");
        wildnessValue = PlayerPrefs.GetInt("wildness");
        */
        isRighty = true;
        speedValue = 0.0f;
        wildnessValue = 0.0f;
        Debug.Log(isRighty + " - " + speedValue + " - " + wildnessValue);
        BattingSideSetter(isRighty);
        PitchingSpeedSetter(speedValue);
        PitchingWildnessSetter(wildnessValue);
    }


    public void BattingSideSetter(bool inputValue)
    {
        isRighty = inputValue;
        PlayerPrefs.SetInt("isRighty", isRighty ? 1 : 0);
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

    public void Back()
    {
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public Dictionary<string, int> GetOptionsMenuInfo()
    {
        Dictionary<string, int> result = new()
        {
            { "PitchSpeed", (int)speedValue },
            { "PitchWildness", (int)wildnessValue },
            { "IsRighty", isRighty ? 1 : 0 }
        };

        return result;
    }
}
