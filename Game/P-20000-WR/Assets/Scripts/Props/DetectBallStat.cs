using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallEventType
{
    Strike,
    Hit,
    Homer,
    Foul
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
        {"StrikeZone", BallEventType.Strike}
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
        if (tags.TryGetValue(other.tag, out ballEvent))
        {
            if (isCollision)
            {
                Debug.Log("Hit !");
                ballEvent = BallEventType.Hit;
            }
            else
            {
                Debug.Log(other.tag + " !");
            }
            
            
        }
        else
        {
            // unknown behaviour
            Debug.Log("Something happened, we are not quite sure why but probably a hit?");
            ballEvent = BallEventType.Hit;
        }

        // send out ball event

    }
}
