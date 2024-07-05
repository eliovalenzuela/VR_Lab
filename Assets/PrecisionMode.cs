using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game4automation;

public class PrecisionMode : MonoBehaviour
{
    public OPCUA_Interface Interfaz;
    public HapticPlugin HapticActor;
    private bool mode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mode = (bool)Interfaz.ReadNodeValue("ns=1;s=:Robot:Applications:Main_app:bool:OpcPreciseMode:OpcPreciseMode[0]");

        if (mode)
        {
            HapticActor.ScaleFactor = 0.0001f;
        }
        else {
            HapticActor.ScaleFactor = 0.001f;
        }
    }
}
