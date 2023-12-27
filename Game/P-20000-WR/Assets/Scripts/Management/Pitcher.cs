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
    private const float ballWeight = 0.15f;
    private const float flightTime = 1.0f;

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
        zVariability = (strikeZone.transform.localScale.x / 2) + 0.2f;
        yVariability = (strikeZone.transform.localScale.y / 2) + 0.2f;

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
        // generate a random location to throw the ball at
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



}
