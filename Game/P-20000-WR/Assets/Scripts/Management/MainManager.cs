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


    void Start()
    {
        process = new Process();
        // open a python program under the given path 
        process.StartInfo.FileName = "python";
        process.StartInfo.Arguments = processPath;

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();
    }

    void OnApplicationQuit()
    {
        process.Kill();
        UnityEngine.Debug.Log("Input detection script stopped");
    }

}
