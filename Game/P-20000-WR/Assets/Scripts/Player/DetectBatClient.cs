using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BatSwinger;

public class DetectBatClient : MonoBehaviour
{
    // In charge of detecting the movement of the cursor (via the
    // BatTM IRL detection) and calculating the movement of the
    // in-game bat

    // Code debt: make an interface for this ??? -- lets be honest buddy, it ain't gettin done lol
    // While at it you could make the script fucking readable YOU BUM

    const int PORT = 12345;
    const string HOST = "127.0.0.1";
    const int BUFFER_SIZE = 1024;
    private TcpClient client;
    private NetworkStream stream;

    private float sweetSpotZRighty = BatPositionUtil.sweetSpotZRighty;
    private float sweetSpotZLefty = BatPositionUtil.sweetSpotZLefty;

    public bool readData = false;
    private bool clientOpen = false;

    public Vector2 framePixelPosition;
    public Vector2 gamePixelPosition;
    
    private Vector2 scaledDistance;
    private Vector2 canvasSZCenter = BatPositionUtil.GetCanvasSZCenter();

    private Vector3 batShift;
    private Vector3 defaultPosition;

    public GameObject batRange;
    public bool batTMSwung = false;
    private Vector3 sweetSpotOffset = new(0f, -0.25f, -1.85f); // from the Orientation gameObject which holds this script

    private BatSwinger batSwingerScript;
    private bool isRighty;

    // events for cursor tracking
    public event EventHandler OnTrackBat;
    public event EventHandler OnStopTrackBat;

    private void Start()
    {
        defaultPosition = BatPositionUtil.WorldSZCenter - sweetSpotOffset;
        // watch out for side switching when computing offsets
        batSwingerScript = GameObject.Find("Bat").GetComponent<BatSwinger>();
        batSwingerScript.OnRegisterSideSwitch += RegisterSideSwitch;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void RegisterSideSwitch(object sender, RegisterSideSwitchEventArgs e)
    {
        isRighty = e.IsRighty;
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
            List<float> parsedData = BatPositionUtil.ParseReceivedData(batData);  // returns a 3 len array

            // check for swing
            batTMSwung = parsedData[2] == 1;

            // get the position offsets into Vector2 form
            framePixelPosition.x = parsedData[0];
            framePixelPosition.y = parsedData[1];
   
            gamePixelPosition = BatPositionUtil.CalculateGamePixelPosition(framePixelPosition);

            // use mouse system to position the bat accordingly
            scaledDistance = BatPositionUtil.ParseMouseInput(
                new Vector2(gamePixelPosition.x, gamePixelPosition.y),
                canvasSZCenter
            );

            batShift = BatPositionUtil.CalculateBatShift(scaledDistance);

            // account for the animation shift when showing pointer
            sweetSpotOffset.z = isRighty ? sweetSpotZRighty : sweetSpotZLefty;
            defaultPosition = BatPositionUtil.WorldSZCenter - sweetSpotOffset;

            // shift the transform's position 
            transform.position = new Vector3(
                defaultPosition.x,
                defaultPosition.y - batShift.y,
                defaultPosition.z - batShift.z
            );
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

    void OnSceneUnloaded(Scene current)
    {
        batSwingerScript.OnRegisterSideSwitch -= RegisterSideSwitch;
        OnApplicationQuit();
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
