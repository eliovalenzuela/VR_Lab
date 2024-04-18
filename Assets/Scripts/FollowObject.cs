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
        if (GOToFollow != null && active)
        {
            if (position)
            {
                Vector3 tempPosition = new Vector3(-(GOToFollow.transform.position.x - initialPositionFollowObject.x), GOToFollow.transform.position.y - initialPositionFollowObject.y, -(GOToFollow.transform.position.z - initialPositionFollowObject.z));
                transform.position = initialPosition + tempPosition;
            }

            if (rotation)
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