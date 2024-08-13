using UnityEngine;

public class ConsoleToGUI : MonoBehaviour
{
    private string myLog = "*begin log";
    private string output;
    private string stack;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
    }

    void OnGUI()
    {
        myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20), myLog);
    }
}