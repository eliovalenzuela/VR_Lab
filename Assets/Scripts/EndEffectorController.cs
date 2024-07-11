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
using TMPro;

public class EndEffectorController : MonoBehaviour
{
    public TMP_Text hapticFeedbackButton;
    public Color hapticFeedbackOnColor;
    public Color hapticFeedbackOffColor;
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

    public Vector3 relativePosition, relativePositionRounded;
    public Vector3 RotationEndPointDegrees = Vector3.zero;
    public Vector3 RotationEndPointDegreesRounded = Vector3.zero;

    public GameObject initialPoint;
    public GameObject HapticActor;
    //public GameObject SpringAnchor;
    public GameObject HapticCollider;
    public Transform EndPoint;
    public Transform Flange;


    public OPCUA_Interface Interface;
    //public string NodeId = "ns=1;s=:Robot:Applications:Main_app:string:sOPCRobotyReady:sOPCRobotyReady[0]";
    public string NodeIdRunningApplication = "ns=1;s=:Robot:Applications:Main_app:string:sOPCRobotyReady:sOPCRobotyReady[0]";
    public string controlStatus;
    //public string applicationStatus;
    //public Matrix4x4 worldToLocalMatrix;
    //public Vector3 tempos;
    public UDPReceiver UDPReceiver;
    public float MinValueForceFeedback;
    public float MaxValueForceFeedback;


    private int activeCount = 0;

    private HapticPlugin hapticPlugin;
    private FollowObject followObject;
    private double MaxLimitForce;
    private float magnitude;
    private bool buttonforce = true;

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
        MaxLimitForce = (double)Interface.ReadNodeValue("ns=1;s=:Robot:Applications:Main_app:num:FxLimit:FxLimit[0]");

        if (!buttonforce)
        {
            hapticFeedbackButton.text = "Haptic Feedback Off";
            hapticFeedbackButton.color = hapticFeedbackOffColor;
        }
        else
        {
            hapticFeedbackButton.text = "Haptic Feedback On";
            hapticFeedbackButton.color = hapticFeedbackOnColor;
        }
    }


    private void FixedUpdate()
    {
        active = hapticPlugin.bIsGrabbing;
        controlStatus = (string)Interface.ReadNodeValue(NodeIdRunningApplication);
        //applicationStatus = (string)Interface.ReadNodeValue(NodeIdRunningApplication);
        if (controlStatus == "Restart") {
            SetInitialPosition = false;
            Waiting = true;

        }
        if (!string.IsNullOrEmpty(controlStatus))
        {
            if (Waiting)
            {
                Waiting = false;
                Invoke(nameof(SetWait), 2.0f);
            }

            if (controlStatus == "Connected" && SetInitialPosition)
            {
                if (!active)
                {
                    hapticPlugin.SetForce("Default Device", new double[] { 1, 1, 1 }, 0);
                    //EndPoint.position = Flange.position;
                    //EndPoint.rotation = Flange.rotation;

                    followObject.SetInitialPositions();
                    followObject.active = false;

                    //relativePosition = Vector3.zero;
                   // relativePositionRounded = Vector3.zero;

                    //relativePosition = transform.InverseTransformPoint(initialPoint.transform.position) * 1000;
                    /// = new Vector3(-Mathf.RoundToInt(relativePosition.y), Mathf.RoundToInt(relativePosition.x), Mathf.RoundToInt(relativePosition.z));

                    //socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    //active = true;
                }
                else
                {
                    
                    magnitude = CalculateTotalForce(UDPReceiver.Fx, UDPReceiver.Fy, UDPReceiver.Fz);
                    double[]  direction = { -UDPReceiver.Fy, -UDPReceiver.Fz, -UDPReceiver.Fx };
                    if (buttonforce)
                    {
                        hapticPlugin.SetForce("Default Device", direction, MapValue(magnitude, MinValueForceFeedback, (float)MaxLimitForce, 0f, MaxValueForceFeedback));
                    }

                    relativePosition = transform.InverseTransformPoint(initialPoint.transform.position) * 1000;
                    relativePositionRounded = new Vector3(-Mathf.RoundToInt(relativePosition.y),Mathf.RoundToInt(relativePosition.x), Mathf.RoundToInt(relativePosition.z));

                    RotationEndPointDegrees = (EndPoint.rotation * Quaternion.Inverse(initialPoint.transform.rotation)).eulerAngles;
                    RotationEndPointDegreesRounded = new Vector3(ObtenerRotacionAjustada(Mathf.RoundToInt(RotationEndPointDegrees.x)), ObtenerRotacionAjustada(Mathf.RoundToInt(RotationEndPointDegrees.y)), ObtenerRotacionAjustada(Mathf.RoundToInt(RotationEndPointDegrees.z)));

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
            //SpringAnchor.GetComponent<FollowObject>().enabled = false;

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
        initialPoint.transform.position = Flange.transform.position;
        SetInitialPosition = true;
        EndPoint.position = Flange.position;
        EndPoint.rotation = Flange.rotation;
        followObject.SetInitialPositions();
        //initialPoint.transform.position = Flange.position;
        //initialPoint.transform.rotation = Flange.rotation;
    }

    float CalculateTotalForce(double fx, double fy, double fz)
    {
        // Aplicamos la fórmula de la magnitud del vector
        
        return Mathf.Sqrt((float)fx * (float)fx + (float)fy * (float)fy + (float)fz * (float)fz);
    }

    float MapValue(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // Asegurar que fromMax y fromMin no sean iguales para evitar división por cero
        if (fromMax == fromMin)
        {
            Debug.LogWarning("fromMax y fromMin no pueden ser iguales.");
            return 0;
        }

        // Aplicar la fórmula de mapeo
        return toMin + ((value - fromMin) / (fromMax - fromMin)) * (toMax - toMin);
    }

    public void ButtonForce() {
        if (buttonforce)
        {
            hapticFeedbackButton.text = "Haptic Feedback Off";
            hapticFeedbackButton.color = hapticFeedbackOffColor;
            buttonforce = false;
        }
        else {
            hapticFeedbackButton.text = "Haptic Feedback On";
            hapticFeedbackButton.color = hapticFeedbackOnColor;
            buttonforce = true;
        }
            
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