using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D batCursor;
    private Vector2 cursorHotSpot;
    private Vector2 canvasSzCenter;
    private bool shouldTrack = false;

    public GameObject batTMCursor;

    // event handlers
    private MovePlayer movePlayerScript;  // fires off locking/unlocking event
    private DetectBatClient detectBatClientScript; // fires off bat tracking events

    // Start is called before the first frame update
    void Start()
    {
        cursorHotSpot = new Vector2(
            batCursor.width / 2,
            batCursor.height / 2
        ); // the middle

        canvasSzCenter = BatPositionUtil.GetCanvasSZCenter();
        canvasSzCenter.y = BatPositionUtil.canvasY - canvasSzCenter.y;  // flip off the y

        // event handling
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();
        detectBatClientScript = GameObject.Find("Orientation").GetComponent<DetectBatClient>();
        
        movePlayerScript.OnPlayerLock += DrawBat;
        movePlayerScript.OnPlayerUnlock += EraseBat;

        detectBatClientScript.OnTrackBat += StartTrackingBat;
        detectBatClientScript.OnStopTrackBat += StopTrackingBat;

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }


    private void DrawBat(object sender, EventArgs e)
    {
        Cursor.SetCursor(batCursor, cursorHotSpot, CursorMode.Auto);
    }

    private void EraseBat(object sender, EventArgs e)
    {
        RemoveCursor();
    }

    private void RemoveCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void StartTrackingBat(object sender, EventArgs e)
    {
        RemoveCursor();
        shouldTrack = true;
        StartCoroutine(TrackBat());
        batTMCursor.SetActive(true);
    }

    private void StopTrackingBat(object sender, EventArgs e)
    {
        shouldTrack = false;
        batTMCursor.SetActive(false);
    }

    IEnumerator TrackBat()
    {
        while (shouldTrack)
        {
            // render the cursor to the center of the strikezone + the offset recorded by the DetectBatClient
            batTMCursor.transform.position = new Vector3(
                detectBatClientScript.gamePixelPosition.x,
                BatPositionUtil.canvasY - detectBatClientScript.gamePixelPosition.y,
                0 
            );

            yield return null;
        }
    }

    void OnSceneUnloaded(Scene current)
    {
        movePlayerScript.OnPlayerLock -= DrawBat;
        movePlayerScript.OnPlayerUnlock -= EraseBat;

        detectBatClientScript.OnTrackBat -= StartTrackingBat;
        detectBatClientScript.OnStopTrackBat -= StopTrackingBat;
    }
}
