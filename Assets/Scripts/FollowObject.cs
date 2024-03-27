using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject GOToFollow;
    public bool position = true;
    public bool rotation = true;
    public bool active = false;

    public Vector3 initialPosition;
    public Vector3 initialPositionFollowObject;
    
    public Quaternion initialRotation;
    public Quaternion initialRotationFollowObject;

    private void FixedUpdate()
    {
        if(GOToFollow != null && active)
        {
            if(position)
                transform.position = initialPosition + GOToFollow.transform.position- initialPositionFollowObject;

            if(rotation)
                transform.rotation = initialRotation * (GOToFollow.transform.rotation * Quaternion.Inverse(initialRotationFollowObject));
        }
    }

    public void SetInitialPositions()
    {
        initialPosition = transform.position;
        initialPositionFollowObject = GOToFollow.transform.position;

        //initialRotation = transform.rotation;
        //initialRotationFollowObject = GOToFollow.transform.rotation;
    }
}
