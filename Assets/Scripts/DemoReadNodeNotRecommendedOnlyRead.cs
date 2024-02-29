using game4automation;
using UnityEditor;
using UnityEngine;

public class DemoReadNodeNotRecommendedOnlyRead : MonoBehaviour
{

    public OPCUA_Interface Interface;
    public string NodeId;
    public double myvar;

    void Update()
    {
        myvar = (double)Interface.ReadNodeValue(NodeId);
    }
}
