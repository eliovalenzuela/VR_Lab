using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCell : MonoBehaviour
{
    public bool activeCellView = false;
    public Color cellColor = Color.white;
    public float cubeSize = 3;
    public float offsetY = -1.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = cellColor;
        if (activeCellView)
        {
            Gizmos.DrawWireCube(transform.position+new Vector3(0, offsetY, 0),new Vector3(cubeSize, cubeSize, cubeSize));
        }
    }
}
