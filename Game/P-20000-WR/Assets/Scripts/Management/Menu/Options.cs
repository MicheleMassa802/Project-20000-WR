using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class Options : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;

    public TextMeshProUGUI speedText = null;
    private float speedValue;
    private float minSpeed = 10.0f;
    private float maxSpeedExtra = 95.8f;
    public TextMeshProUGUI wildnessText = null;
    private float wildnessValue;
    private float minWildness = 0.0f;
    private float maxWildnessExtra = 100.0f;

    public bool isRighty;


    public void BattingSideSetter(bool inputValue)
    {
        isRighty = inputValue;
    }

    public void PitchingSpeedSetter(float inputValue)
    {
        speedValue = inputValue * maxSpeedExtra + minSpeed;
        speedText.text = speedValue.ToString("0");  // whole number
    }

    public void PitchingWildnessSetter(float inputValue)
    {
        wildnessValue = inputValue * maxWildnessExtra + minWildness;
        wildnessText.text = wildnessValue.ToString("0");  // whole number
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
