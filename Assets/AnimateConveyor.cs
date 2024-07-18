using game4automation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateConveyor : MonoBehaviour
{
    public Drive ConveyorDrive;
    public OPCUA_Interface PLC_Interface;

    public string NodeIdPosition;
    public string NodeIdVelocity;

    public double initialPosition;
    public double actualPosition;
    public double actualVelocity;
    private void Start()
    {
        initialPosition = (double)PLC_Interface.ReadNodeValue(NodeIdPosition);
        InvokeRepeating(nameof(ReadValue), 0, 0.2f);
    }

    public void ReadValue()
    {
        actualPosition = (double)PLC_Interface.ReadNodeValue(NodeIdPosition);
        actualVelocity = (double)PLC_Interface.ReadNodeValue(NodeIdVelocity);
    }
    public Transform conveyor;
    public Transform alturaMinimaReferencia;
    public Transform alturaMaximaReferencia;

    public void Animar(float t)
    {
        t = Mathf.Clamp01(t);

        float alturaIntermedia = Mathf.Lerp(alturaMinimaReferencia.position.y, alturaMaximaReferencia.position.y, t);

        Vector3 nuevaPosicion = conveyor.position;
        nuevaPosicion.y = alturaIntermedia;
        conveyor.position = nuevaPosicion;
    }

    void Update()
    {
        ConveyorDrive.CurrentPosition = (float)(actualPosition - initialPosition);
        ConveyorDrive.CurrentSpeed = (float)-actualVelocity;
    }


}

