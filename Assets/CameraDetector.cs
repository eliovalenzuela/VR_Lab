using game4automation;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetector : MonoBehaviour
{
    private short currentCamera =0;
    private int previousCamera=-1;
    public List<GameObject> cameras = new List<GameObject>();
    public EndEffectorController endEffectorController;

    public OPCUA_Interface Interface;
    public string NodeId;

    void Update()
    {
        currentCamera = (short)Interface.ReadNodeValue(NodeId);

        if (previousCamera!= currentCamera)
        {
            SetCamera(currentCamera);
            previousCamera = currentCamera;
            Debug.Log("La cámara activa es la numero: " + currentCamera);
            //transform.rotation = Quaternion.Inverse(activeCamera.transform.rotation);
            transform.rotation = cameras[currentCamera].transform.rotation;
            endEffectorController.SetInitialPosition = false;
            endEffectorController.Waiting = true;
        }
    }

    public void SetCamera(int index)
    {
        for(int i=0; i< cameras.Count; i++)
        {
            cameras[i].SetActive(i == index ? true : false);
        }
        currentCamera = (short)index;
    }

    //public void SetCamera(string id)
    //{
    //    for (int i = 0; i < cameras.Count; i++)
    //    {
    //        if(cameras[i].id == id)
    //        {
    //            cameras[i].SetActive(true);
    //            currentCamera = i;
    //        }
    //        else
    //        {
    //            cameras[i].camera.SetActive(false);
    //        }
    //    }
    //}
}
