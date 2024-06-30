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

    private const float sweetSpotZRighty = -1.85f;
    private const float sweetSpotZLefty = 1.85f;

    public bool readData = false;

    private Vector3 mousePos;
    private Vector3 batShift;
    private Vector3 defaultPosition;

    private Vector2 scaledDistance;
    private Vector2 canvasSZCenter;

    public GameObject batRange;
    public bool batMKSwung = false;
    private Vector3 sweetSpotOffset = new(0f, -0.25f, -1.85f); // from the Orientation gameObject which holds this script

    private BatSwinger batSwingerScript;
    private bool isRighty;

    private void Start()
    {
        defaultPosition = BatPositionUtil.WorldSZCenter - sweetSpotOffset;
        canvasSZCenter = BatPositionUtil.GetCanvasSZCenter();
        // watch out for side switching when computing offsets
        batSwingerScript = GameObject.Find("Bat").GetComponent<BatSwinger>();
        batSwingerScript.OnRegisterSideSwitch += (obj, eventArgs) => { isRighty = eventArgs.IsRighty; };
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
        float mouseY = BatPositionUtil.canvasY - mousePos.y;

        // get the position offsets into Vector2 form
        scaledDistance = BatPositionUtil.ParseMouseInput(
            new Vector2(mouseX, mouseY),
            canvasSZCenter
        );
        Debug.Log("Scaled: " + scaledDistance);
        batShift = BatPositionUtil.CalculateBatShift(scaledDistance);
        Debug.Log("Shift: " + batShift);

        // account for the animation shift when showing pointer
        sweetSpotOffset.z = isRighty ? sweetSpotZRighty : sweetSpotZLefty;
        defaultPosition = BatPositionUtil.WorldSZCenter - sweetSpotOffset;

        // shift the transform's position 
        transform.position = new Vector3(
            defaultPosition.x,
            defaultPosition.y - batShift.y,
            defaultPosition.z - batShift.z
        );
        Debug.Log("World Pos: " + transform.position);

        // check for swing
        batMKSwung = Input.GetMouseButtonDown(0);
    }


    public void ConnectClient() { readData = true; }
    public void DcClient() { readData = false; }
}
