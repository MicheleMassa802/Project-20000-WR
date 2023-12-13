using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            cameraPosition.position.x,
            transform.position.y,
            cameraPosition.position.z);
    }
}
