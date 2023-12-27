using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling.Editor;
using UnityEngine;

public class BatPositionUtil : MonoBehaviour
{
    // whole lotta sketchy code going on over here
    private static readonly float zRangePx = 640;
    private static readonly float yRangePx = 480;
    private static readonly float zRange = 3.85f;
    private static readonly float yRange = 3.1f;
    private static Vector3 rangeCenter = new Vector3(-0.2f, 1.85f, 0f);
    private static Vector3 topLeftRange =
        new Vector3(-0.2f, rangeCenter.y + (yRange / 2), rangeCenter.z + (zRange / 2));

    // helper for parsing the received string which has the form of a python dictionary
    public static List<float> ParseDic(string strDic)
    {
        // get the values of each data piece
        // w/ a strDic of the form: {"pos": [x,x], "spd": y, "dir": z}
        List<float> result;

        int validCount = strDic.Count(c => c == '}');
        strDic = strDic.Replace("}", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);
        string[] strList = strDic.Split(',');

        if (strList.Length == 3  || validCount > 1)
        {
            // null values (no ',' in position)
            result = new List<float>() { 0, 0, 0, 0 };
            return result;
        }

        // otw, non-null values being fed
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


    // helper to calculate how the bat moves in the bat range zone based on the positions
    // provided in the parsedData float list
    public static Vector3 CalculateBatPos(List<float> parsedData)
    {
        float inGameZ = parsedData[0] * zRange / zRangePx;
        float inGameY = parsedData[1] * yRange / yRangePx;

        // add these distances to the top left coord of the range and return that
        return new Vector3(topLeftRange.x, topLeftRange.y - inGameY, topLeftRange.z - inGameZ);
    }
}
