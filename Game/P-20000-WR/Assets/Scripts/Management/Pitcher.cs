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
    public float throwForce = 10.0f;

    private float xVariability;
    private float yVariability;
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
        xVariability = (strikeZone.transform.localScale.x / 2) + 0.2f;
        yVariability = (strikeZone.transform.localScale.y / 2) + 0.2f;

        InvokeRepeating("Pitch", 3.0f, 10f);
    }


    private void Pitch()
    {
        GameObject ball = easyMode ? ezBallPrefab : ballPrefab;
        // instantiate a ball directed at the mound
        pitchLocation = RandomLocation();
        Vector3 throwDirection = handPosition - pitchLocation;

        // calculations
        throwDirection = throwDirection.normalized * throwForce;
        float distance = throwDirection.magnitude;

        float throwAngle = 5f;
        float velocity = Mathf.Sqrt((distance * -Physics.gravity.y) / (Mathf.Sin(2 * throwAngle * Mathf.Deg2Rad)));
        Vector3 initVelo = new Vector3(-(velocity * Mathf.Cos(throwAngle * Mathf.Deg2Rad)), velocity * Mathf.Sin(throwAngle * Mathf.Deg2Rad), 0);
        velo = initVelo;

        // spawn ball
        GameObject instantiatedBall = Instantiate(ball, pitcherHand.position, Quaternion.identity);

        // apply
        Rigidbody ballRb = instantiatedBall.GetComponent<Rigidbody>();
        ballRb.velocity = initVelo;

    }

    private Vector3 RandomLocation()
    {
        // generate a random location to throw the ball at
        float randX = UnityEngine.Random.Range(-xVariability, xVariability);
        float randY = UnityEngine.Random.Range(-yVariability, yVariability);
        Vector3 pitchLoc = new Vector3(dot.x + randX, dot.y + randY, dot.z);
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
        Gizmos.DrawLine(handPosition, velo);

    }



}
