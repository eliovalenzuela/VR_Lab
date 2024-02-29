using System.Collections;
using System.Collections.Generic;
using game4automation;
using UnityEngine;

public class NodeControl : MonoBehaviour
{
    public OPCUA_Interface Interface;
    public string NodeId;

    public bool x, y, z;
    public float NodeValue;
    public float PositionX;
    public float PositionY;
    public float PositionZ;

    private OPCUA_Node node;
    private OPCUANodeSubscription subscription;

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
        NodeValue = (float)obj;
        if (x)
        {
            PositionX = (float)obj; // sets the new position based on the new value 
        }

        if (y)
        {
            PositionY = (float)obj; // sets the new position based on the new value 
        }

        if (z)
        {
            PositionZ = (float)obj; // sets the new position based on the new value 
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(PositionX, PositionY, PositionZ));
    }
}
