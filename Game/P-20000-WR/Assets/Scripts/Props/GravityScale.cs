using System;
using UnityEngine;

public class GravityScale : MonoBehaviour
{
    // for modifying how gravity affects balls at different times
    private const float airGravity = 2.0f;
    private const float groundGravity = 0.5f;
    private const float pitchGravity = 0.5f;

    public float gravityScale = pitchGravity;
    public static float globalGravity = 9.81f;

    private Rigidbody object_RB;
    private BallLifeCycleManager ballLifeCycleManager;

    void OnEnable()
    {
        object_RB = GetComponent<Rigidbody>();
        object_RB.useGravity = false;
        ballLifeCycleManager = gameObject.GetComponent<BallLifeCycleManager>();
        if (ballLifeCycleManager != null )
        {
            ballLifeCycleManager.OnHitBall += OnHit;
            ballLifeCycleManager.OnGroundBall += OnGround;
        }
    }

    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.down;
        object_RB.AddForce(gravity, ForceMode.Acceleration);
    }

    // for when the ball is hit
    void OnHit(object sender, EventArgs e)
    {
        // x2 does the job well at better replicating a baseballs trajectory
        // since I originally made the ball lighter for when we are pitching!
        gravityScale = airGravity;
    }

    // for when the ball touches the ground
    void OnGround(object sender, EventArgs e)
    {
        // we go back to x0.5 to allow for the ball to slide better and feel lighter
        gravityScale = groundGravity;
    }
}
