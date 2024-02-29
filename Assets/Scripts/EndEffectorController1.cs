using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class EndEffectorController1 : MonoBehaviour
{
    public bool active = false;
    public bool SetInitialPosition = false;
    // El socket UDP
    private Socket socket;
    private IPEndPoint endPoint;

    // El puerto y la dirección IP del ordenador receptor
    public int port = 1;
    public string ip = "192.168.31.99";

    public Vector3 InitialPosition;
    public Vector3 PositionEndPointMilimeters = Vector3.zero;
    public Transform EndPoint;
    public Transform Flange;

    void Start()
    {
        // Crear el socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Crear el punto final de destino
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
    }


    private void Update()
    {
        if (active)
        {
            SetInitialPosition = false;
            //var temp = EndPoint;//.InverseTransformPoint(Flange.position);
            var temp = EndPoint.localPosition; //.InverseTransformPoint(EndPoint.position);
            PositionEndPointMilimeters = new Vector3(-temp.x, temp.z, -temp.y) * 1000;
            //PositionEndPointMilimeters = new Vector3(-EndPoint.position.x, EndPoint.position.z, -EndPoint.position.y) * 1000;

            

            byte[] data = Encoding.UTF8.GetBytes(PositionEndPointMilimeters.x.ToString("0000.00;-000.00") +
                                                ":" +
                                                 PositionEndPointMilimeters.y.ToString("0000.00;-000.00") +
                                                ":" +
                                                 PositionEndPointMilimeters.z.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ";");


                //data = Encoding.UTF8.GetBytes(velocidadLineal.x.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                velocidadLineal.y.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                velocidadLineal.z.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                velocidadAngularGrados.x.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                velocidadAngularGrados.y.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                velocidadAngularGrados.z.ToString("0000.00;-000.00") +
                //                                ";");

            // Enviar el array de bytes al ordenador receptor
            socket.SendTo(data, endPoint);
        }
        

        if(SetInitialPosition)
        {
            InitialPosition = new Vector3(-EndPoint.localPosition.x, EndPoint.localPosition.z, -EndPoint.localPosition.y) * 1000;

            byte[] data = Encoding.UTF8.GetBytes(InitialPosition.x.ToString("0000.00;-000.00") +
                                                ":" +
                                                InitialPosition.y.ToString("0000.00;-000.00") +
                                                ":" +
                                                InitialPosition.z.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ":" +
                                                0.ToString("0000.00;-000.00") +
                                                ";");

            // Enviar el array de bytes al ordenador receptor
            socket.SendTo(data, endPoint);
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
