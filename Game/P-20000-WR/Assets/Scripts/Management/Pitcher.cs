using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pitcher : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject ezBallPrefab;
    public Transform pitcherHand;
    public GameObject strikeZone;
    public bool easyMode;
    public int timeOut = 8;
    public float throwForce = 1.7f;
    public float throwAngle = 5f;

    private float zVariability;
    private float yVariability;
    private float variableOffset;
    private const float ballWeight = 0.15f;
    private float flightTime;

    private Vector3 dot;
    private Vector3 pitchLocation;
    private Vector3 handPosition;

    private Vector3 velo;

    // Start is called before the first frame update
    void Start()
    {
        easyMode = true;
        handPosition = pitcherHand.position;

        dot = strikeZone.transform.position;
        SetupPitchingSettings();
        InvokeRepeating("Pitch", 3.0f, 10f);
    }


    private void Pitch()
    {
        GameObject ball = easyMode ? ezBallPrefab : ballPrefab;
        // instantiate a ball directed at the mound
        pitchLocation = RandomLocation();
        Vector3 throwDirection = pitchLocation - handPosition;

        float xVelo = throwDirection.x / flightTime;
        float yVelo = (throwDirection.y + (-0.5f * Physics.gravity.y * flightTime * flightTime * ballWeight)) / flightTime;
        // 0.5 is the gravity multiplier on the ball during a pitch
        float zVelo = throwDirection.z / flightTime;
        velo = new Vector3(xVelo, yVelo, zVelo) * throwForce;

        // spawn ball
        GameObject instantiatedBall = Instantiate(ball, pitcherHand.position, Quaternion.identity);

        // apply
        Rigidbody ballRb = instantiatedBall.GetComponent<Rigidbody>();
        ballRb.velocity = velo;

    }

    private Vector3 RandomLocation()
    {
        float randZ = UnityEngine.Random.Range(-zVariability, zVariability);
        float randY = UnityEngine.Random.Range(-yVariability, yVariability);

        Vector3 pitchLoc = new Vector3(dot.x, dot.y + randY, dot.z + randZ);
        return pitchLoc;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(handPosition, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pitchLocation, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(handPosition, pitchLocation);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(handPosition, handPosition + velo);

    }

    private void SetupPitchingSettings()
    {

        // wildness settings
        int wildness = PlayerPrefs.GetInt("wildness");

        // come up with the variability ranges based on the player's set wildness
        variableOffset = 0;

        if (wildness >= 90)
        {
            variableOffset = 1.0f;
        }
        else if (wildness >= 70)
        {
            variableOffset = 0.5f;
        }
        else if (wildness >= 50)
        {
            variableOffset = 0.2f;
        }
        // otw, no offset from full strike zone

        // set variability for randomness calculations
        zVariability = 0.0f;
        yVariability = 0.0f;

        if (wildness >= 25)
        {
            // use full strike zone + offset
            zVariability = (strikeZone.transform.localScale.x / 2) + variableOffset;
            yVariability = (strikeZone.transform.localScale.y / 2) + variableOffset;
        }
        else if (wildness >= 1)
        {
            // dots + 0.15 offset
            zVariability = 0.15f;
            yVariability = 0.15f;
        }
        // wildness == 0 means dots


        // speed settings
        int speed = PlayerPrefs.GetInt("speed");

        // 90 mph => 0.5 seconds flight time (realistically)
        // + 0.3f game offset -> to make it fun yo :<)

        flightTime = (90.0f / speed * 0.5f) + 0.3f;
    }

}
