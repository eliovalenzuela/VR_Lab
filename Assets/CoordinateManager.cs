using UnityEngine;

public class CoordinateManager : MonoBehaviour
{
    [Header("References")]
    public Transform objectA;
    public Transform objectB;
    public Transform objectC;

    [Header("Coordinate-System Settings")]
    public Vector3 A;
    public Vector3 AB;
    public Vector3 AC;

    [ContextMenu("Apply New Coordinates")]
    public void ApplyNewCoordinates()
    {
        // just for making sure this transform is reset
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Make this object parent of objectA,objectB and objectC keeping their current transforms
        // For reverting it later store current parents
        var parentA = objectA.parent;
        var parentB = objectB.parent;
        var parentC = objectC.parent;

        objectA.SetParent(transform);
        objectB.SetParent(transform);
        objectC.SetParent(transform);

        // place this object to the new pivot point A
        // and rotate it to the correct axis
        // so that its right (X) vector euqals AB
        // and its up (Y) vector equals AC
        // Unity will handle the rotation accordingly
        transform.position = A;
        transform.right = AB;
        transform.up = AC;

        // Optionally reset the objects to the old parents
        objectA.SetParent(parentA);
        objectB.SetParent(parentB);
        objectC.SetParent(parentC);
    }

    // Use this method to place another object in the coordinate system of this object
    // without any parenting
    public void SetPosition(Transform obj, Vector3 relativePosition)
    {
        // sets the obj to relativePosition in the 
        // local coordinate system of this rotated and translated manager
        obj.position = transform.TransformPoint(relativePosition);

        // adjust the rotation
        // Quaternions are added by multiplying them
        // so first we want the changed coordinate system's rotation
        // then add the rotation it had before
        obj.rotation = transform.rotation * obj.rotation;
    }

    // Only for visualization of the pivot point A and the 
    // AB(red) and AC(green) axis in the SceneView
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(A, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(A, A + AB);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(A, A + AC);
    }
}