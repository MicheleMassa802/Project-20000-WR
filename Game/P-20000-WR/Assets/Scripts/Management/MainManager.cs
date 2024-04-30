using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    // This is the first script to be called by the running game and its in charge
    // of invoking the bat input detector

    // NOTE: this makes Python a required install for running this game !!!

    private Process process;
    private const string processPath = @"Assets\Scripts\Management\Input\SendInput.py";


    void OnApplicationQuit()
    {
        // if using BatTM, stop script
        int inputType = PlayerPrefs.GetInt("inputType");
        if (inputType == 2)
        {
            StopBatInputDetection();
        }
    }

    // the function handling which input type to take when stepping into the plate
    public void StartGame()
    {
        // get the argument
        int inputType = PlayerPrefs.GetInt("inputType");

        // M&K specific
        if (inputType == 2) { StartBatInputDetection(); } 

    }

    public void StopGame()
    {
        // get the argument
        int inputType = PlayerPrefs.GetInt("inputType");

        // M&K specific
        if (inputType == 2) { StopBatInputDetection(); }

    }

    private void StartBatInputDetection()
    {
        // get the argument
        int rightyInt = PlayerPrefs.GetInt("isRighty");
        string isRighty = rightyInt == 0 ? "False" : "True";

        process = new Process();
        // open a python program under the given path 
        process.StartInfo.FileName = "python";
        process.StartInfo.Arguments = processPath + " --isRighty " + isRighty;

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();
        UnityEngine.Debug.Log("Input detection script started");
    }

    private void StopBatInputDetection()
    {
        process.Kill();
        UnityEngine.Debug.Log("Input detection script stopped");
    }

}
