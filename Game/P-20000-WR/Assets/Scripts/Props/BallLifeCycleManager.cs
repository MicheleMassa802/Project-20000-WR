using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BatSwinger;

public class BallLifeCycleManager : MonoBehaviour
{
    private const string BatTag = "Bat";
    private const string PlayerTag = "Player";
    private const string StrikeZoneTag = "StrikeZone";
    private const string SweetSpotId = "SweetSpot";
    private const float BatWoodRadius = 0.25f;


    // struct to hold relevant collision data
    private struct BallEvent
    {
        private Collider collisionPair;  // the object that interacted with the ball to create the event
        private List<ContactPoint> contacts;  // each with their own points, normals, impulses, ...
    
        public BallEvent(Collider otherObject, List<ContactPoint> allContacts)
        {
            collisionPair = otherObject;
            contacts = allContacts;
        }
    }

    // event arg for passing ball outcome information
    public class BallOutcomeData : EventArgs
    {
        public BallOutcome Outcome { get; set; }
    }

    // class variables
    [SerializeField] private float timeToKill = 15f;

    private float contactToBallDistance = 0f;
    private List<BallEvent> ballLifeSequence = new();
    private GameObject sweetSpot;
    private Rigidbody ballRb;

    public float maxForceBoost;
    public float minForceBoost;
    public event EventHandler<BallOutcomeData> OnRegisterBallResults;
    


    private void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        sweetSpot = GameObject.Find(SweetSpotId);

        StartCoroutine(ManageBallLifeCycle());
    }


    // based on the different triggers/collisions with the enviroment, populate the sequence
    private void OnTriggerEnter(Collider collider)
    {
        // check if first contact
        if (ballLifeSequence.Count == 0)
        {
            Debug.Log($"Triggering {collider.name}");
            // first contact trigger can be either a strike or wild pitch
            if (collider.CompareTag(StrikeZoneTag))
            {
                RecordOutcome(BallOutcome.Strike);
                return;
            }
            else
            {
                RecordOutcome(BallOutcome.Ball);
                return;
            }
        }

        // make a new collision out of the collider that you interacted with
        ballLifeSequence.Add(new BallEvent(collider, null));
    }


    private void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider;

        // check if first contact
        if (ballLifeSequence.Count == 0)
        {
            Debug.Log($"Colliding {collider.name}");
            // first contact collision can be either a batted ball, HBP, or wild pitch
            if (collider.CompareTag(BatTag))
            {
                PowerUpBattedBall(collision);
                // fall through to record the contact
            }
            else if (collider.CompareTag(PlayerTag))
            {
                RecordOutcome(BallOutcome.HBP);
                return;
            }
            else
            {
                RecordOutcome(BallOutcome.Ball);
                return;
            }
        }

        // make a new collision out of the collider that you interacted with
        List<ContactPoint> allContacts = new();
        _ = collision.GetContacts(allContacts);
        ballLifeSequence.Add(new BallEvent(collider, allContacts));
    }


    private void PowerUpBattedBall(Collision collision)
    {
        // at the moment of detection, power up ball's reaction to being batted
        double impact;

        foreach (ContactPoint contact in collision.contacts)
        {
            // apply a small force corresponding to that contact's normal
            // proportional to the distance from the sweet spot

            contactToBallDistance = Vector3.Distance(sweetSpot.transform.position, contact.point);

            // get the force to apply based on the distance between the ball and the sweetspot
            impact = ForceImpact(contactToBallDistance);

            // apply unit force in opposite direction of contact normal with the calculated impact
            ballRb.AddForce(-contact.normal * (float)impact, ForceMode.Impulse);
        }
    }

    IEnumerator ManageBallLifeCycle()
    {
        // wait 15 seconds until destruction (can be interrupted when outcome is recorded early)
        yield return new WaitForSeconds(timeToKill);

        // after 15 seconds, analyze the ball's sequence, send off outcome, and sunset the ball
        AnalyzeForOutcome();
    }

    private void AnalyzeForOutcome()
    {
        // TODO: implement ... another day, follow guide on DetectBallStat
        RecordOutcome(BallOutcome.Undetectable);
    }

    private void RecordOutcome(BallOutcome registeredOutcome)
    {
        Debug.Log($"Recording {registeredOutcome}");

        // send off outcome of this ball for record registering and UI updates
        OnRegisterBallResults?.Invoke(this, new BallOutcomeData { Outcome = registeredOutcome });

        // destroy ball in 2 seconds (for dramatic effect)
        Destroy(gameObject, 2f);
    }


    // Function corresponding to y = (1 / (0.3 + e^-x)) - 0.55
    // Where x is distance between the ball and the bat's sweet spot
    // and y is the returned value between 1 and 0, dictating where in the range 
    // between the max and min force boost the current batted ball lands.
    private double ForceImpact(double x)
    {
        // adjust the input to account for the radius of the bat
        x -= BatWoodRadius;
        Debug.Log($"Adjusted input {x}");

        // get the complement of x (as the function is flipped)
        // x = -0.3 is the limit before effect is considered too small, we clamp
        x = (1 - x < -0.3f) ? -0.3f : 1 - x;

        double y = (1 / (0.3f + Math.Exp(-x))) - 0.55f;

        //  scale & return
        double result = ((maxForceBoost - minForceBoost) * y) + minForceBoost;

        Debug.Log($"Output {y} | Scaled {result}");
        return result;
    }
}
