using System;
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
    private bool processRunning = false;

    private MovePlayer movePlayerScript;  // fires off locking/unlocking event
    private DetectBatClient detectBatClientScript;
    private DetectMouseInput detectMouseInputScript;
    private Options optionsScript;

    public event EventHandler OnPlayBall;
    public event EventHandler OnQuitBall;


    private void Start()
    {
        // event handling
        GameObject batOrientation = GameObject.Find("Orientation");

        detectBatClientScript = batOrientation.GetComponent<DetectBatClient>();
        detectMouseInputScript = batOrientation.GetComponent <DetectMouseInput>();
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();
        optionsScript = GameObject.Find("BattingPopup").GetComponent<Options>();

        optionsScript.OnGameStart += StartGame;
        movePlayerScript.OnPlayerUnlock += StopGame;
    }


    // the function handling which input type to take when stepping into the plate
    private void StartGame(object sender, EventArgs e)
    {
        // get the argument
        int inputType = PlayerPrefs.GetInt("inputType");

        if (inputType == 1)  // BatTM specific
        { 
            UnityEngine.Debug.Log("Starting BatTM scripts");
            
            // start up host, then client and establish a connection
            StartBatInputHost();
            detectBatClientScript.ConnectClient();

        }
        else  // otherwise, we handle input via M&K
        {
            UnityEngine.Debug.Log("Starting M&K scripts");
            detectMouseInputScript.ConnectClient();
        }

        OnPlayBall?.Invoke(this, EventArgs.Empty);
    }

    private void StopGame(object sender, EventArgs e)
    {
        // get the argument
        int inputType = PlayerPrefs.GetInt("inputType");

        if (inputType == 1)  // BatTM specific
        {
            UnityEngine.Debug.Log("Stopping scripts");
            StopBatInputHost();
            detectBatClientScript.DcClient();
        }
        else  // otherwise, we handle input via M&K
        {
            UnityEngine.Debug.Log("Stopping M&K scripts");
            detectMouseInputScript.DcClient();
        }

        OnQuitBall?.Invoke(this, EventArgs.Empty);

    }

    private void StartBatInputHost()
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
        processRunning = true;
        UnityEngine.Debug.Log("Input detection script started");
    }

    private void StopBatInputHost()
    {
        process.Kill();
        UnityEngine.Debug.Log("Input detection script stopped");
        processRunning = false;
    }

    void OnApplicationQuit()
    {
        // if using BatTM, stop script
        int inputType = PlayerPrefs.GetInt("inputType");
        if (inputType == 1 && processRunning)
        {
            StopBatInputHost();
        }
    }

}
