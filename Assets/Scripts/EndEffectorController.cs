using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using game4automation;
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


    public float ScaleFactorPosition = 200;
    public GameObject HapticActor;
    public GameObject HapticCollider;
    public Transform Flange;
    public Vector3 ultimaPosicion;
    public Vector3 robotTCPPos;
    public Vector3 robotTCPPosReal;
    public Vector3 robotTCPRot;
    public float tiempoAnterior;
    public bool OPCUAIsConnected = false;
    public Vector3 distancePos;
    public Vector3 desplazamiento;
    public Vector3 velocidadLineal;


    public OPCUA_Interface Interface;
    public string NodeId = "ns=1;s=:Robot:Applications:Main_app:string:sOPCRobotyReady:sOPCRobotyReady[0]";
    public string NodeIdRunningApplication = "ns=1;s=:Robot:Applications:Main_app:string:sOPCRobotyReady:sOPCRobotyReady[0]";
    public string controlStatus;
    public string applicationStatus;
    //public Matrix4x4 worldToLocalMatrix;
    //public Vector3 tempos;

    private int activeCount = 0;


    private HapticPlugin hapticPlugin;

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

    }


    private void FixedUpdate()
    {
        active = hapticPlugin.bIsGrabbing;
            // Calcula el desplazamiento desde la última posición
            desplazamiento = HapticCollider.transform.position - ultimaPosicion;

            // Calcula la velocidad lineal (velocidad en unidades por segundo)
            velocidadLineal = desplazamiento / (Time.time - tiempoAnterior);
            velocidadLineal *= ScaleFactorPosition;


            // Actualiza la última posición y rotación
            ultimaPosicion = HapticCollider.transform.position;

            // Actualiza el tiempo anterior
            tiempoAnterior = Time.time;

            // Imprime las velocidades en Vector3
            Debug.Log("Velocidad Lineal: " + velocidadLineal + " unidades por segundo");


            // Crear el socket UDP
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            byte[] data = Encoding.UTF8.GetBytes(0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ";");


            if (active)
            {
            data = Encoding.UTF8.GetBytes(velocidadLineal.x.ToString("0000.00;-000.00") +
                                                ":" +
                                                velocidadLineal.y.ToString("0000.00;-000.00") +
                                                ":" +
                                                velocidadLineal.z.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ";");

            }

            else
            {

                data = Encoding.UTF8.GetBytes(0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ";");

            }
            // Enviar el array de bytes al ordenador receptor
            socket.SendTo(data, endPoint);


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
