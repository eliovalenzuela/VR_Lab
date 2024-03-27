using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using game4automation;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EndEffectorController : MonoBehaviour
{
    public bool active = false;
    public bool SetInitialPosition = false;
    public bool Waiting = true;
    // El socket UDP
    private Socket socket;
    private Socket socketReceive;
    private IPEndPoint endPoint;

    IPAddress ipAdd = IPAddress.Parse("192.168.31.207");
    IPEndPoint remoteEP;

    // El puerto y la dirección IP del ordenador receptor
    public int port = 1;
    public string ip = "192.168.31.99";

    public Vector3 relativePosition,relativePositionRounded;
    public Vector3 RotationEndPointDegrees = Vector3.zero;
    public Vector3 RotationEndPointDegreesRounded = Vector3.zero;

    public GameObject initialPoint;
    public GameObject HapticActor;
    public GameObject SpringAnchor;
    public GameObject HapticCollider;
    public Transform EndPoint;
    public Transform Flange;


    public OPCUA_Interface Interface;
    public string NodeId = "ns=1;s=:Robot:Applications:Main_app:string:sOPCRobotyReady:sOPCRobotyReady[0]";
    public string NodeIdRunningApplication = "ns=1;s=:Robot:Applications:Main_app:string:sOPCRobotyReady:sOPCRobotyReady[0]";
    public string controlStatus;
    public string applicationStatus;
    //public Matrix4x4 worldToLocalMatrix;
    //public Vector3 tempos;

    private int activeCount = 0;

    public bool trampeado = false;

    private HapticPlugin hapticPlugin;
    private FollowObject followObject;

    void Start()
    {
        // Crear el socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //socketReceive = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Crear el punto final de destino
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        //remoteEP = new IPEndPoint(ipAdd, 2);

        socket.Connect(endPoint);

        hapticPlugin = HapticActor.GetComponent<HapticPlugin>();
        followObject = GetComponent<FollowObject>();
    }


    private void FixedUpdate()
    {
        active = hapticPlugin.bIsGrabbing;
        controlStatus = (string)Interface.ReadNodeValue(NodeId);
        applicationStatus = (string)Interface.ReadNodeValue(NodeIdRunningApplication);
        if (!string.IsNullOrEmpty(applicationStatus) || trampeado)
        {
            if (Waiting)
            {
                Waiting = false;
                Invoke(nameof(SetWait), 2.0f);
            }

            if(controlStatus == "Connected" && SetInitialPosition)
            {
                if (!active)
                {
                    //EndPoint.position = Flange.position;
                    //EndPoint.rotation = Flange.rotation;

                    followObject.SetInitialPositions();
                    followObject.active = false;

                    relativePosition = Vector3.zero;
                    relativePositionRounded = Vector3.zero;

                    relativePosition = transform.InverseTransformPoint(initialPoint.transform.position) * 1000;
                    relativePositionRounded = new Vector3(-Mathf.RoundToInt(relativePosition.y), Mathf.RoundToInt(relativePosition.x), Mathf.RoundToInt(relativePosition.z));

                    //socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    //active = true;
                }
                else
                {

                    relativePosition = transform.InverseTransformPoint(initialPoint.transform.position)*1000;
                    relativePositionRounded = new Vector3(-Mathf.RoundToInt(relativePosition.y), Mathf.RoundToInt(relativePosition.x), Mathf.RoundToInt(relativePosition.z));

                    RotationEndPointDegrees = (EndPoint.rotation * Quaternion.Inverse(initialPoint.transform.rotation)).eulerAngles;
                    RotationEndPointDegreesRounded = new Vector3(ObtenerRotacionAjustada(Mathf.RoundToInt(RotationEndPointDegrees.x)), ObtenerRotacionAjustada(Mathf.RoundToInt(RotationEndPointDegrees.y)), ObtenerRotacionAjustada(Mathf.RoundToInt(RotationEndPointDegrees.z))) ;

                    byte[] data = Encoding.UTF8.GetBytes(relativePositionRounded.x.ToString("0000.00;-000.00") +
                                                        ":" +
                                                         relativePositionRounded.y.ToString("0000.00;-000.00") +
                                                        ":" +
                                                         relativePositionRounded.z.ToString("0000.00;-000.00") +
                                                        ":" +
                                                        RotationEndPointDegreesRounded.x.ToString("0000.00;-000.00") +
                                                        ":" +
                                                        RotationEndPointDegreesRounded.y.ToString("0000.00;-000.00") +
                                                        ":" +
                                                        RotationEndPointDegreesRounded.z.ToString("0000.00;-000.00") +
                                                        ";");

                    // Enviar el array de bytes al ordenador receptor
 
                        socket.SendTo(data, endPoint);

                    followObject.active = true;
                    Debug.Log("Enviando");
                }  

            }
        }
        else
        {
            relativePositionRounded = Vector3.zero;
            followObject.active = false;
            SpringAnchor.GetComponent<FollowObject>().enabled = false;
            //socket.Close();
            Interface.Restart();
            active = false;
            SetInitialPosition = false;
            Waiting = true;
        }

        //byte[] receivedData = new byte[100];
        //int k = socket.Receive(receivedData);
        //Debug.Log(k);

    }
    float ObtenerRotacionAjustada(float rotacionOriginal)
    {
        // Ajusta la rotación al rango de -180 a +180 grados
        float rotacionAjustada = rotacionOriginal % 360f;

        if (rotacionAjustada > 180f)
        {
            rotacionAjustada -= 360f;
        }
        else if (rotacionAjustada < -180f)
        {
            rotacionAjustada += 360f;
        }

        return rotacionAjustada;
    }

    public void SetWait()
    {
        SetInitialPosition = true;
        EndPoint.position = Flange.position;
        EndPoint.rotation = Flange.rotation;
        //initialPoint.transform.position = Flange.position;
        //initialPoint.transform.rotation = Flange.rotation;
    }

    private void OnDisable()
    {
        // Cerrar el socket
            socket.Close();
    }

    private void OnDestroy()
    {
        // Cerrar el socket
            socket.Close();
    }

    private void OnApplicationQuit()
    {
        // Cerrar el socket
            socket.Close();
    }
}
