using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followObjByForce : MonoBehaviour
{
    public Transform toFollow;
    float speedScale = 100;
    float maxSpeedMagnitude = 5f;
    float maxRotationSpeedMagnitude = 1f;

    // Update is called once per frame
    void Update()
    {
        if (!toFollow)
            return;
        // DIRECTION
        Vector3 d = (toFollow.position - transform.position) * speedScale;
        if (d.magnitude > maxSpeedMagnitude)
            d = d / d.magnitude * maxSpeedMagnitude;
        GetComponent<Rigidbody>().velocity = d;

        // ROTATION
        transform.rotation = toFollow.rotation;
    }
}
