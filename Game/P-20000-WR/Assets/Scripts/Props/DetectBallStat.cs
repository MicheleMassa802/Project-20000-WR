using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallEventType
{
    Strike,
    Hit,
    Homer,
    Foul,
    HBP
}

public class DetectBallStat : MonoBehaviour
{

    readonly Dictionary<string, BallEventType> tags = new()
    {
        {"FoulRight", BallEventType.Foul},
        {"FoulLeft", BallEventType.Foul},
        {"HRCenter", BallEventType.Homer},
        {"HRRight", BallEventType.Homer},
        {"HRLeft", BallEventType.Homer},
        {"StrikeZone", BallEventType.Strike},
        {"Player", BallEventType.HBP},
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

    private void HandleContact(Collider other, bool isCollision)
    {
        BallEventType ballEvent;
        return;  // TO BE REMOVED
        if (tags.TryGetValue(other.tag, out ballEvent))
        {
            if (ballEvent == BallEventType.Foul)
            {
                Debug.Log("Foul Ball !");
            }
            else if (ballEvent == BallEventType.HBP)
            {
                Debug.Log("Hit By Pitch !");
            }
            else if (isCollision)  
            {
                // collision and no foul/HBP and within the tags => off the wall
                Debug.Log("Hit !");
                ballEvent = BallEventType.Hit;
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
            ballEvent = BallEventType.Hit;
        }

        // send out ball event

    }
}
