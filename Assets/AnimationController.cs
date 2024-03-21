using game4automation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public OPCUA_Interface Interface;
    public string NodeId;
    public short myvar =30;
    public float valorNormalizado;

    public AnimateConveyor animadorConveyor;
    public AnimatePlatform animadorPlatform;
    public AnimateBox animadorBox;

    private void Update()
    {
        myvar = (short)Interface.ReadNodeValue(NodeId);
    }

    void FixedUpdate()
    {
        valorNormalizado = Normalize(myvar, 30, 180);
        animadorPlatform.Animar(valorNormalizado);
        animadorConveyor.Animar(valorNormalizado);
        animadorBox.Animar(valorNormalizado);
    }

    public float Normalize(short value, float min, float max)
    {
        if (min == max)
        {
            Debug.LogError("Error: min y max son iguales, no se puede normalizar.");
            return 0f;
        }

        return (value-min)/(max- min);
    }
}
