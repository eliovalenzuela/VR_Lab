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

        InvokeRepeating(nameof(ReadMode), 0, 0.5f);
    }

    public void ReadMode()
    {
        mode = (bool)Interfaz.ReadNodeValue("ns=1;s=:Robot:Applications:Main_app:bool:OpcPreciseMode:OpcPreciseMode[0]");
    }
    void Update()
    {
        

        if (mode)
        {
            HapticActor.ScaleFactor = 0.0001f;
        }
        else {
            HapticActor.ScaleFactor = 0.001f;
        }
    }
}
