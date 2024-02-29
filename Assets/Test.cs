using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 forward = Vector3.zero;
    public Vector3 positionsWithRotation = Vector3.zero;

    void Update()
    {
        forward = transform.forward;

        positionsWithRotation = transform.TransformDirection(forward); ;
    }
}
