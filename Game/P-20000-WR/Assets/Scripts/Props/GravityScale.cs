using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityScale : MonoBehaviour
{
    // for modifying how gravity affects balls at different times

    public float gravityScale = 0.5f;

    public static float globalGravity = -9.81f;

    Rigidbody object_RB;

    void OnEnable()
    {
        object_RB = GetComponent<Rigidbody>();
        object_RB.useGravity = false;
    }

    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        object_RB.AddForce(gravity, ForceMode.Acceleration);
    }

    // for when the ball is hit
    void OnHit()
    {
        gravityScale = 1.0f;
    }
}
