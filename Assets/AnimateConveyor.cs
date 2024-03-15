using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateConveyor : MonoBehaviour
{
    public Transform conveyor;
    public Transform alturaMinimaReferencia;
    public Transform alturaMaximaReferencia;

    // Función para animar la plataforma
    public void Animar(float t)
    {
        // Asegurarse de que t esté en el rango [0, 1]
        t = Mathf.Clamp01(t);

        // Calcular la posición intermedia utilizando Lerp
        float alturaIntermedia = Mathf.Lerp(alturaMinimaReferencia.position.y, alturaMaximaReferencia.position.y, t);

        // Actualizar la posición de la plataforma
        Vector3 nuevaPosicion = conveyor.position;
        nuevaPosicion.y = alturaIntermedia;
        conveyor.position = nuevaPosicion;
    }
}

