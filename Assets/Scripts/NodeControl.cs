using System.Collections;
using System.Collections.Generic;
using game4automation;
using UnityEngine;

public class NodeControl : MonoBehaviour
{
    public OPCUA_Interface Interface;
    public string NodeId;

    public bool x, y, z;
    public double NodeValue =0.0f;

    private OPCUA_Node node;
    private OPCUANodeSubscription subscription;

    private float initialRotationX, initialRotationY, initialRotationZ;

    private void Awake()
    {
        initialRotationX = transform.localRotation.eulerAngles.x;
        initialRotationY = transform.localRotation.eulerAngles.y;
        initialRotationZ = transform.localRotation.eulerAngles.z;
    }

    // Start is called before the first frame update
    void Start()
    {

        if (Interface != null)
            Interface.EventOnConnected.AddListener(OnConnected);

    }

    public void OnConnected()
    {
        subscription = Interface.Subscribe(NodeId, NodeChanged);
    }

    public void NodeChanged(OPCUANodeSubscription sub, object obj) // Is called when Node Value of Node nodeid is changed
    {
        NodeValue = (double)obj;

    }

    void Update()
    {
        if (x)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(initialRotationX + (float)NodeValue, initialRotationY, initialRotationZ));
        }

        if (y)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(initialRotationX, initialRotationY + (float)NodeValue, initialRotationZ));
        }

        if (z)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(initialRotationX, initialRotationY, initialRotationZ + (float)NodeValue));
        }
    }
}
