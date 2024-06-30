using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class DetectBatClient : MonoBehaviour
{
    // In charge of detecting the movement of the cursor (via the
    // BatTM IRL detection) and calculating the movement of the
    // in-game bat

    // Code debt: make an interface for this ??? -- lets be honest buddy, it ain't gettin done lol

    const int PORT = 12345;
    const string HOST = "127.0.0.1";
    const int BUFFER_SIZE = 1024;
    private TcpClient client;
    private NetworkStream stream;
    
    public bool readData = false;
    private bool clientOpen = false;

    public Vector2 scaledDistance;
    private Vector3 batShift;
    private Vector3 defaultPosition;

    public GameObject batRange;
    public bool batTMSwung = false;

    // events for cursor tracking
    public event EventHandler OnTrackBat;
    public event EventHandler OnStopTrackBat;

    private void Start()
    {
        defaultPosition = transform.position;
    }


    IEnumerator ReceiveData()
    {
        while (readData)
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
            List<float> parsedData = BatPositionUtil.ParseReceivedData(batData);  // returns a 3 len array
            // get the position offsets into Vector2 form
            scaledDistance = new Vector2(parsedData[0], parsedData[1]);
            batShift = BatPositionUtil.CalculateBatShift(scaledDistance);

            // shift the transform's position
            transform.position = new Vector3(
                defaultPosition.x,
                defaultPosition.y - batShift.y,
                defaultPosition.z - batShift.z
            );

            // check for swing
            batTMSwung = parsedData[2] == 1;
        } 

    }


    public void ConnectClient()
    {
        // move bat to center of bat range and start reading in input data
        readData = true;

        // only called with BatTM input
        ConnectToServer();

        // start tracking bat movement in-game via cursor
        OnTrackBat?.Invoke(this, EventArgs.Empty);
    }

    public void DcClient()
    {
        // only called with BatTM input

        // move bat back to player's control and stop receiving data
        readData = false;

        // stop reading in input data
        StopCoroutine(ReceiveData());
        // DC from the server
        client.Close();

        // stop tracking bat movement in-game via cursor
        OnStopTrackBat?.Invoke(this, EventArgs.Empty);

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
        clientOpen = true;

        // after detecting the welcome message, wait for player to get in the
        // box to parse all bat data
        StartCoroutine(ReceiveData());
    }


    void OnApplicationQuit()
    {
        int inputType = PlayerPrefs.GetInt("inputType");
        if (inputType == 1 && clientOpen)
        {
            client.Close();
        }
    }


}
