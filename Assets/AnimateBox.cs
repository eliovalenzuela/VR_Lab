using game4automation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBox : MonoBehaviour
{
    public Transform box;
    public Transform alturaMinimaReferencia;
    public Transform alturaMaximaReferencia;

    public void Animar(float t)
    {
        t = Mathf.Clamp01(t);

        float alturaIntermedia = Mathf.Lerp(alturaMinimaReferencia.position.y, alturaMaximaReferencia.position.y, t);

        Vector3 nuevaPosicion = box.position;
        nuevaPosicion.y = alturaIntermedia;
        box.position = nuevaPosicion;
    }
}

