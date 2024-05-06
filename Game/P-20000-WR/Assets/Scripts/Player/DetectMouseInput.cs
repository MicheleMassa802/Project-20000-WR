using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

public class DetectMouseInput : MonoBehaviour
{
    // In charge of detecting the movement of the cursor (via
    // mouse input) and calculating the movement of the
    // in-game bat

    // naming conventions follow those of DetectBatClient for the 
    // purpose of similarity
    // Code debt: make an interface for this ???

    private static readonly float screenHeight = Screen.height;
    private static readonly float xRangePx = 128;   // scaled down
    private static readonly float yRangePx = 96;    // scaled down

    private bool readData = false;

    private Vector3 mousePos;
    private Vector3 batShift;
    private Vector3 defaultPosition;

    private Vector2 scaledDistance;
    private Vector2 canvasSZCenter;
    private Vector2 canvasToZoneScaling;

    public GameObject batRange;
    public static bool batMKSwung = false;

    private void Start()
    {
        defaultPosition = transform.position;
        canvasSZCenter = BatPositionUtil.GetCanvasSZCenter();
        canvasToZoneScaling = new Vector2(
            (int)(Screen.width / xRangePx),
            (int)(Screen.height / yRangePx)
        );
    }

    private void Update()
    {
        if (readData)
        {
            MoveBat();
        }
    }

    private void MoveBat()
    {
        // in charge of moving bat based on the mouse position

        mousePos = Input.mousePosition;
        float mouseX = mousePos.x;
        float mouseY = screenHeight - mousePos.y;

        // get the position offsets into Vector2 form
        scaledDistance = BatPositionUtil.ParseMouseInput(
            new Vector2(mouseX, mouseY),
            canvasSZCenter,
            canvasToZoneScaling
        );
        Debug.Log("Scaled: " + scaledDistance);
        batShift = BatPositionUtil.CalculateBatShift(scaledDistance);

        // shift the transform's position
        transform.position = new Vector3(
            defaultPosition.x,
            defaultPosition.y - batShift.y,
            defaultPosition.z - batShift.z
        );

        // check for swing
        batMKSwung = Input.GetMouseButtonDown(0);
        

    }

    private void RenderCursor()
    {

    }

    public void ConnectClient() { readData = true; }
    public void DcClient() { readData = false; }
}
