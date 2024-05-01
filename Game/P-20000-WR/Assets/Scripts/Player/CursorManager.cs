using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D batCursor;

    private MovePlayer movePlayerScript;  // fires off locking/unlocking event
    private Vector2 cursorHotSpot;

    // Start is called before the first frame update
    void Start()
    {
        cursorHotSpot = new Vector2(
            batCursor.width / 2,
            batCursor.height / 2
        ); // the middle

        // event handling
        movePlayerScript = GameObject.Find("Player").GetComponent<MovePlayer>();
        movePlayerScript.OnPlayerLock += DrawBat;
        movePlayerScript.OnPlayerUnlock += EraseBat;

    }


    private void DrawBat(object sender, EventArgs e)
    {
        Cursor.SetCursor(batCursor, cursorHotSpot, CursorMode.Auto);
    }

    private void EraseBat(object sender, EventArgs e)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
