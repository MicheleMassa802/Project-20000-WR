using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallOutcome
{
    Strike,
    Hit,
    Ball,
    Homer,
    Foul,
    HBP,
    Undetectable,
    Unset
}

public class DetectBallStat : MonoBehaviour
{

    public readonly static Dictionary<string, BallOutcome> tags = new()
    {
        {"FoulRight", BallOutcome.Foul},
        {"FoulLeft", BallOutcome.Foul},
        {"HRCenter", BallOutcome.Homer},
        {"HRRight", BallOutcome.Homer},
        {"HRLeft", BallOutcome.Homer},
        {"StrikeZone", BallOutcome.Strike},
        {"Player", BallOutcome.HBP},
    };

    

    // based on the different triggers around the stadium, call the appropriate event
    private void OnTriggerEnter(Collider other)
    {
        HandleContact(other, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleContact(collision.collider, true);
    }

    private static void HandleContact(Collider other, bool isCollision)
    {
        BallOutcome ballEvent;
        return;  // TO BE REMOVED

        // CHANGE TO SWITCH YOU FUCKING DONKEY
        if (tags.TryGetValue(other.tag, out ballEvent))
        {
            if (ballEvent == BallOutcome.Foul)
            {
                Debug.Log("Foul Ball !");
            }
            else if (ballEvent == BallOutcome.HBP)
            {
                Debug.Log("Hit By Pitch !");
            }
            else if (isCollision)  
            {
                // collision and no foul/HBP and within the tags => off the wall
                Debug.Log("Hit !");
                ballEvent = BallOutcome.Hit;
            }
            else
            {
                // for all triggers
                Debug.Log(other.tag + " !");
            }
            
             
        }
        else
        {
            // unknown behaviour
            Debug.Log("Something happened, probably a ground ball");
            ballEvent = BallOutcome.Hit;
        }

        // send out ball event

    }
}
