using game4automation;
using UnityEditor;
using UnityEngine;

public class DemoReadNodeNotRecommended : MonoBehaviour
{

    public OPCUA_Interface Interface;
    public string NodeId;
    public double myvar;
    public bool x, y, z;
    public float timeInSeconds;
    private float initialRotationX, initialRotationY, initialRotationZ;

    private void Awake()
    {
        initialRotationX = transform.localRotation.eulerAngles.x;
        initialRotationY = transform.localRotation.eulerAngles.y;
        initialRotationZ = transform.localRotation.eulerAngles.z;
    }

    private void Start()
    {
        Application.targetFrameRate = -1;

        InvokeRepeating(nameof(ReadValue), 0, timeInSeconds);
    }

    public void ReadValue()
    {
        myvar = (double)Interface.ReadNodeValue(NodeId);
    }

    void Update()
    {
        if (x)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(initialRotationX + (float)myvar, initialRotationY, initialRotationZ));
        }

        if (y)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(initialRotationX, initialRotationY + (float)myvar, initialRotationZ));
        }

        if (z)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(initialRotationX, initialRotationY, initialRotationZ + (float)myvar));
        }
        
    }

    
}
