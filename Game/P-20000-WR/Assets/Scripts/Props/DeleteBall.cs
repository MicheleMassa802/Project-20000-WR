using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class DeleteBall : MonoBehaviour
{
    // Delete this game object after 15 seconds of its existence
    private void Start()
    {
        Destroy(gameObject, 15f);  
    }
}
