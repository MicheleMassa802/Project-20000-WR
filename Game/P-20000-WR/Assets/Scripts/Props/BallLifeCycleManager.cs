using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallLifeCycleManager : MonoBehaviour
{

#region Enums for tags and outcomes

    public enum BallContactPoint
    {
        Bat,
        Player,
        StrikeZone,
        Foul,
        HR,
        Field,
        Untagged
    }

    public enum BallOutcome
    {
        Strike,
        Hit,
        Ball,
        Homer,
        Foul,
        HBP,
        Out,
        Undetectable
    }

    #endregion

    private const string SweetSpotId = "SweetSpot";
    private const float BatWoodRadius = 0.25f;


    // struct to hold relevant collision data
    private struct BallEvent
    {
        public Collider collisionPair;  // the object that interacted with the ball to create the event
        public List<ContactPoint> contacts;  // each with their own points, normals, impulses, ...
    
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
    [SerializeField] private float timeToKill = 10f;

    private float contactToBallDistance = 0f;
    private List<BallEvent> ballLifeSequence = new();
    private GameObject sweetSpot;
    private Rigidbody ballRb;
    private TrailRenderer trailRenderer;

    public float maxForceBoost;
    public float minForceBoost;
    public event EventHandler<BallOutcomeData> OnRegisterBallResults;
    public event EventHandler OnHitBall;
    public event EventHandler OnGroundBall;

    private bool checkedForOutcome = false;



    private void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        sweetSpot = GameObject.Find(SweetSpotId);
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null) { trailRenderer.enabled = false; }

        StartCoroutine(ManageBallLifeCycle());
    }


    // based on the different triggers/collisions with the enviroment, populate the sequence
    private void OnTriggerEnter(Collider collider)
    {
        if (checkedForOutcome) return;

        // check if first contact
        if (ballLifeSequence.Count == 0)
        {
            // first contact trigger can be either a strike or wild pitch
            if (collider.CompareTag(BallContactPoint.StrikeZone.ToString()))
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
        if (checkedForOutcome) return;

        Collider collider = collision.collider;

        // check if first contact
        if (ballLifeSequence.Count == 0)
        {
            // first contact collision can be either a batted ball, HBP, or wild pitch
            if (collider.CompareTag(BallContactPoint.Bat.ToString()))
            {
                // power up ball, set its gravity scale back to 1, and activate trails
                PowerUpBattedBall(collision);
                OnHitBall?.Invoke(this, EventArgs.Empty);
                if (trailRenderer != null) { trailRenderer.enabled = true; }
                // fall through to record the contact
            }
            else if (collider.CompareTag(BallContactPoint.Player.ToString()))
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

        // no matter if its first collision, if we hit an untagged object,
        // restore ground gravity for the ball
        if (collider.CompareTag(BallContactPoint.Untagged.ToString()))
        {
            OnGroundBall?.Invoke(this, EventArgs.Empty);
        }

        // make a new collision out of the collider that you interacted with
        List<ContactPoint> allContacts = new();
        _ = collision.GetContacts(allContacts);
        ballLifeSequence.Add(new BallEvent(collider, allContacts));
    }


    private void PowerUpBattedBall(Collision collision)
    {
        if (checkedForOutcome) return;

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
        if (checkedForOutcome) return;

        foreach (BallEvent lifeCycleEvent in ballLifeSequence)
        {
            Enum.TryParse(lifeCycleEvent.collisionPair.tag, out BallContactPoint bcp);

            // balls, strikes, and HBPs already accounted for
            switch (bcp)
            {
                case BallContactPoint.HR:
                    // HR immediately returns 
                    RecordOutcome(BallOutcome.Homer);
                    return;

                case BallContactPoint.Field:
                    // hit immediatly returns
                    RecordOutcome(BallOutcome.Hit);
                    return;


                case BallContactPoint.Foul:
                    // got to fould without returning early from hit => foul
                    RecordOutcome(BallOutcome.Foul); 
                    return;

                // bat, or ground => skip until we can recognize
                default: 
                    continue;
            }
        }

        // if you get to the end without recording a hit/foul/homer/strike/ball/HBP
        // the ball stayed in the infield => out
        RecordOutcome(BallOutcome.Out);
    }

    private void RecordOutcome(BallOutcome registeredOutcome)
    {
        // send off outcome of this ball for record registering and UI updates
        OnRegisterBallResults?.Invoke(this, new BallOutcomeData { Outcome = registeredOutcome });
        checkedForOutcome = true;
        
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

        // get the complement of x (as the function is flipped)
        // x = -0.3 is the limit before effect is considered too small, we clamp
        x = (1 - x < -0.3f) ? -0.3f : 1 - x;

        double y = (1 / (0.3f + Math.Exp(-x))) - 0.55f;

        //  scale & return
        double result = ((maxForceBoost - minForceBoost) * y) + minForceBoost;

        return result;
    }
}
