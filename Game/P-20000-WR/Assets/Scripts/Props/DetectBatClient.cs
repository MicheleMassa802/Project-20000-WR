using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class DetectBatClient : MonoBehaviour
{
    // in charge of detecting the bat IRL and moving the game object accordingly
    // attached to the bat object based on the respective batting range

    const int PORT = 12345;
    const string HOST = "127.0.0.1";
    const int BUFFER_SIZE = 1024;
    private TcpClient client;
    private NetworkStream stream;
    
    private MovePlayer movePlayerScript;  // fires off locking/unlocking event
    private bool readData = false;

    private Vector3 wrtPlayerPos;
    public GameObject batRange;

    public GameObject mainManagerObj;
    private MainManager mainManager;


    private void Start()
    {
        mainManager = mainManagerObj.GetComponent<MainManager>();

        // event handling
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();
        movePlayerScript.OnPlayerLock += LockPlayer;
        movePlayerScript.OnPlayerUnlock += UnlockPlayer;

    }


    IEnumerator ReceiveData()
    {
        while (true)
        {
            if (stream.DataAvailable)
            {
                byte[] data = new byte[BUFFER_SIZE];
                int bytes = stream.Read(data, 0, data.Length);
                string batData = Encoding.UTF8.GetString(data, 0, bytes);
                MoveBat(batData);
            }
            yield return null;
        }
    }

    private void MoveBat(string batData)
    {
        // in charge of moving bat based on the batData sent by the camera
        if (readData)
        {
            Debug.Log(batData);
            List<float> parsedData = BatPositionUtil.ParseDic(batData);  // returns a 4 len array
            transform.position = BatPositionUtil.CalculateBatPos(parsedData);
            transform.rotation = Quaternion.Euler(parsedData[3], transform.rotation.eulerAngles.y,
                transform.rotation.eulerAngles.z);
        } 
    }


    private void LockPlayer(object sender, EventArgs e)
    {
        // if input type is BatTM, start running the python program and run the server
        int inputType = PlayerPrefs.GetInt("inputType");
        if (inputType == 2)
        {
            mainManager.StartGame();
            // then connect to the server
            ConnectToServer();

            // move bat to center of bat range and start reading in input data
            readData = true;
            wrtPlayerPos = transform.position;
            transform.position = batRange.transform.position;
        }

        // otherwise, we handle input via M&K/GamePad


    }

    private void UnlockPlayer(object sender, EventArgs e)
    {
        // if using BatTM, stop script
        int inputType = PlayerPrefs.GetInt("inputType");
        if (inputType == 2)
        {
            // DC from the server
            client.Close();
            // then stop running it
            mainManager.StopGame();

            // move bat back to player's control and stop receiving data
            readData = false;
            transform.position = wrtPlayerPos;
        }

    }

    // connect to server called upon stepping into the plate and using the BatTM setting
    private void ConnectToServer()
    {
        client = new TcpClient(HOST, PORT);
        stream = client.GetStream();

        byte[] data = new byte[BUFFER_SIZE];
        int bytes = stream.Read(data, 0, data.Length);
        string welcomeMsg = Encoding.UTF8.GetString(data, 0, bytes);
        Debug.Log(welcomeMsg);

        // after detecting the welcome message, wait for player to get in the
        // box to parse all bat data
        StartCoroutine(ReceiveData());
    }


    void OnApplicationQuit()
    {
        int inputType = PlayerPrefs.GetInt("inputType");
        if (inputType == 2)
        {
            client.Close();
        }
    }


}
