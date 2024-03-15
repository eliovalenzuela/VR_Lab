using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlatform : MonoBehaviour
{
    public Transform plataforma;
    public Transform escalaMinimaReferencia;
    public Transform escalaMaximaReferencia;

    // Función para animar la plataforma
    public void Animar(float t)
    {
        // Asegurarse de que t esté en el rango [0, 1]
        t = Mathf.Clamp01(t);

        // Calcular la escala intermedia utilizando Lerp
        float escalaIntermedia = Mathf.Lerp(escalaMinimaReferencia.localScale.y, escalaMaximaReferencia.localScale.y, t);

        // Actualizar la escala de la plataforma
        Vector3 nuevaEscala = plataforma.localScale;
        nuevaEscala.y = escalaIntermedia;
        plataforma.localScale = nuevaEscala;
    }
}

