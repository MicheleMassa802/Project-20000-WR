using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling.Editor;
using UnityEngine;

public class BatPositionUtil : MonoBehaviour
{
    // whole lotta sketchy code going on over here
    // DO NOT ASK ME WHICH HIGHER ENTITY SPOKE THESE CONSTANTS TO ME
    public static readonly float zRangePx = 128;   // scaled down
    public static readonly float yRangePx = 96;    // scaled down
    public static readonly float ySZCenter = 60;   // scaled down
    public static readonly float zRange = 3.85f;
    public static readonly float yRange = 3.1f;

    public static readonly float ySZHeightPx = 41;    // scaled down
    public static readonly float xSZWidthPx = 27;   // scaled down
    public static readonly float gameSZHeight = 1.3f;
    public static readonly float gameSZWidth = 0.9f;
    
    public static readonly float xRangeFramePx = 640;
    public static readonly float yRangeFramePx = 480;
    public static readonly float canvasX = Screen.width;
    public static readonly float canvasY = Screen.height;
    public static readonly Vector2 canvasToZoneScaling = new(
        (int)(canvasX / zRangePx),
        (int)(canvasY / yRangePx)
    );
    public static readonly Vector3 WorldSZCenter = new(0, 1.65f, 0);
    public static float sweetSpotZRighty = -1.85f;
    public static float sweetSpotZLefty = 1.85f;

    public static Vector2 GetCanvasSZCenter()
    {
        // return the canvas SZ center based on its scaled down measurements
        int canvasSZx = (int)(canvasX / 2);
        int canvasSZy = (int)(canvasY * ySZCenter / yRangePx);

        return new Vector2(canvasSZx, canvasSZy);
    }

    // helper in charge of outputing the scaled distance from the center of the strikezone
    // based on the input mouse position
    public static Vector2 ParseMouseInput(Vector2 mousePos, Vector2 sZcenter)
    {
        return (mousePos - sZcenter) / canvasToZoneScaling;
    }

    // helper for parsing the received string which has the form of a python dictionary
    // this is the updated version of the previously used ParseDic.
    public static List<float> ParseReceivedData(string strDic)
    {
        // get the values of each data piece
        // w/ a strDic of the form: {"scaled_dist": [x,x], "swing": y}
        List<float> result;

        int validCount = strDic.Count(c => c == '}');
        
        // error check
        if (validCount == 0)
        {
            return new List<float>() { 0, 0, 0};  // nothing received
        } 
        else if (validCount > 1)
        {
            // use the first (complete) set of sent data
            strDic = strDic.Split('}')[0];
        }

        // {"scaled_dist": [x,x], "swing": y}
        strDic = strDic.Replace("}", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);
        // "scaled_dist": x,x, "swing": y
        string[] strList = strDic.Split(',');
        // "scaled_dist": x---x---"swing": y

        // No need for 'None' check => handled in backend by sending {(0,0), 0}

        result = new();
        foreach (string s in strList)
        {
            // put s into result
            string moddedS;
            if (s.Contains(':'))
            {
                moddedS = s.Split(':')[1].Trim();
            }
            else
            {
                moddedS = s.Trim();
            }

            result.Add(float.Parse(moddedS));
        }
        return result;
    }

    // helper to calculate how the bat moves in the bat range zone based
    // on the positions provided in the parsedData float list (of 3 elements)
    public static Vector3 CalculateBatShift(Vector2 parsedData)
    {
        // parsedData.x and .y (the first two elements) contain the scaled pixel offsets
        // from the center of the canvas batting region panel

        // scale them to displacements in Unity units and add those displacements to the
        // bat's positions
        float inGameZ = parsedData.x * zRange / zRangePx;  
        float inGameY = parsedData.y * yRange / yRangePx;

        // add these distances to the top left coord of the range and return that
        return new Vector3(0, inGameY, inGameZ);
    }

    // helper to calculate the in-game pixel position given the in-frame pixel position
    public static Vector3 CalculateGamePixelPosition(Vector2 parsedData)
    {

        float inGameX = parsedData.x * canvasX / xRangeFramePx;
        float inGameY = parsedData.y * canvasY / yRangeFramePx;

        // add these distances to the top left coord of the range and return that
        return new Vector3(inGameX, inGameY, 0);
    }

    // helper to transform inGame offsets from the center of the strikezone into
    // ui bat range pixel offsets
    public static Vector2 GameToPixelStrikeZoneOffsets(Vector2 gamePosition)
    {
        float xScaled = -gamePosition.x * xSZWidthPx / gameSZWidth;  // negated to invert offset dir
        float yScaled = gamePosition.y * ySZHeightPx / gameSZHeight;
        return new Vector2(xScaled, yScaled);
    }
}
