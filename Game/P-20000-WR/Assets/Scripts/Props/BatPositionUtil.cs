using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling.Editor;
using UnityEngine;

public class BatPositionUtil : MonoBehaviour
{
    // whole lotta sketchy code going on over here
    private static readonly float zRangePx = 128;  // scaled down
    private static readonly float yRangePx = 96;  // scaled down
    private static readonly float zRange = 3.85f;
    private static readonly float yRange = 3.1f;
    private static Vector3 rangeCenter = new Vector3(-0.2f, 1.85f, 0f);
    private static Vector3 topLeftRange =
        new Vector3(-0.2f, rangeCenter.y + (yRange / 2), rangeCenter.z + (zRange / 2));


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

            Debug.Log("Modded " + moddedS);
            result.Add(float.Parse(moddedS));
        }
        return result;
    }

    // helper to calculate how the bat (orientation) moves in the bat range zone based
    // on the positions provided in the parsedData float list (of 3 elements)
    public static Vector3 CalculateBatShift(List<float> parsedData)
    {
        // parsedData[0] and [1] contain the scaled pixel offsets from the center
        // of the canvas batting region panel

        // scale them to displacements in Unity units and add those displacements to the
        // bat's positions
        float inGameZ = parsedData[0] * zRange / zRangePx;
        float inGameY = parsedData[1] * yRange / yRangePx;

        // add these distances to the top left coord of the range and return that
        return new Vector3(0, inGameY, inGameZ);
    }
}
