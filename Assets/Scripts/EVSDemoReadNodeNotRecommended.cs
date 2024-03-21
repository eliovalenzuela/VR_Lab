using game4automation;
using UnityEditor;
using UnityEngine;

public class EVSDemoReadNodeNotRecommended : MonoBehaviour
{

    public OPCUA_Interface Interface;
    public string NodeId;
    public HapticPlugin HapticPlugin;
    public float valor;
    public bool myvar;

    void Update()
    {
        myvar = (bool)Interface.ReadNodeValue(NodeId);
        // Accede al script del GameObject
        valor = (float)(HapticPlugin.GlobalScale);

        if (myvar)
        {
            HapticPlugin.GlobalScale = 0.0001f;
            HapticPlugin.ScaleFactor = 0.0001f;
        }
        else
        {
            HapticPlugin.GlobalScale = 0.001f;
            HapticPlugin.ScaleFactor = 0.001f;
        }
    }
}
