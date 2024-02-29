// This code contains 3D SYSTEMS Confidential Information and is disclosed to you
// under a form of 3D SYSTEMS software license agreement provided separately to you.
//
// Notice
// 3D SYSTEMS and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from 3D SYSTEMS is strictly prohibited.
//
// ALL 3D SYSTEMS DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". 3D SYSTEMS MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, 3D SYSTEMS assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of 3D SYSTEMS. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// 3D SYSTEMS products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// 3D SYSTEMS.
//
// Copyright (c) 2021 3D SYSTEMS. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.Profiling;






public class VirtualHaptic : MonoBehaviour
{
    
   public HapticPlugin HPlugin = null;

   public bool ShowGizmo = true;
   public bool ShowLabels = true;

    // El puerto y la dirección IP del ordenador receptor
    public int port = 1;
    public string ip = "192.168.31.99";

    // El valor que queremos enviar

    public double angleX = 0F;
    public double angleY = 0F;
    public double angleZ = 0F;


    public double curAngleX = 0F;
    public double curAngleY = 0F;
    public double curAngleZ = 0F;
    
    public double oldAngleX = 0F;
    public double oldAngleY = 0F;
    public double oldAngleZ = 0F;
    
    public double velX = 0F;
    public double velY = 0F;
    public double velZ = 0F;
    
    public int gripper = 0;

    private long oldTimestamp = 0;
    private long curTimestamp = 0;

    public double rotSpeedX = 0F;
    public double rotSpeedY = 0F;
    public double rotSpeedZ = 0F;

    public long timeDif = 0;

    public int contador = 1;

    // El socket UDP
    private Socket socket;
    private IPEndPoint endPoint;

    public float posicionXt1 = 0F;
    public float posicionYt1 = 0F;
    public float posicionXt2 = 0F;
    public float posicionYt2 = 0F;
    public float posicionXt3 = 0F;
    public float posicionYt3 = 0F;
    public long tiempot1 = 0;
    public long tiempot2 = 0;
    public long tiempot3 = 0;
    public float angulot1t2 = 0F;
    public float angulot2t3 = 0F;
    public float velocidadAngularX = 0F;
    public float velocidadAngularY = 0F;
    public float velocidadAngularZ = 0F;


    public double velXOld = 0F;
    public double velYOld = 0F;
    public double velZOld = 0F;

    public Vector3 e;
    public Vector3 posicion;

    public float direccion = 0;

    public float magnitud = 0.4f;

    public float limiteXPos = 75;
    public float limiteXNeg = -55;
    public float limiteYPos = 140;
    public float limiteYNeg = -72;
    public float limiteZPos = 64;
    public float limiteZNeg = -138;

    public float umbralX = 10f;
    public float umbralY = 10f;
    public float umbralZ = 10f;

    public float posX = 0F;
    public float posY = 0F;
    public float posZ = 0F;

    public Vector3 direccionEmpuje = new Vector3(0, 0, 0);

    private GameObject joint;

    public bool boton1 = false;
    public bool boton2 = false;

    public GameObject HapticCollider;
    public GameObject EndPointStaubli;
    public GameObject Touch;


    // Para recibir por UDP
    
    // The port number for the remote device.
    private const int receivePort = 11000;

    // The IP address of the remote device.
    private string receiveIP = "192.168.31.184";

    // The UDP socket for sending and receiving data.
    private UdpClient receiveUdpClient;

    // The IPEndPoint object to store the address and port of the remote device.
    private IPEndPoint receiveRemoteEndPoint;

    // The message to send to the remote device.
    private string message = "Is anybody there?";

    // The message received from the remote device.
    private string receiveMessage = "";



    private Vector3 ultimaPosicion;
    private Vector3 ultimaRotacion;
    public Vector3 robotTCPPos;
    public Vector3 robotTCPPosReal;
    public Vector3 robotTCPRot;
    private float tiempoAnterior;
    private EulerGet eulerGet;
    public float ScaleFactorPosition;
    public bool OPCUAIsConnected = false;
    public bool readyToMove = false;
    public bool positionBased = true;
    public Vector3 distancePos;

    public DemoReadNodeNotRecommendedOnlyRead positionX;
    public DemoReadNodeNotRecommendedOnlyRead positionY;
    public DemoReadNodeNotRecommendedOnlyRead positionZ;

    void Start()
    {
        // Create a UDP socket with the specified port number.
        receiveUdpClient = new UdpClient(receivePort);
        // Create an IPEndPoint object with the IP address and port number of the remote device.
        receiveRemoteEndPoint = new IPEndPoint(IPAddress.Parse(receiveIP), receivePort);

        eulerGet = HapticCollider.GetComponent<EulerGet>();

        ultimaPosicion = HapticCollider.transform.position;
        ultimaRotacion = new Vector3((float)eulerGet.EulerX, (float)eulerGet.EulerY, (float)eulerGet.EulerZ);
        
        tiempoAnterior = Time.time;

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Crear el punto final de destino
        endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        Invoke(nameof(GetInitialPosTCP), 3);
    }


    private void Update()
    {

        if (HPlugin == null)
            return;


        //e = bTransform.rotation.eulerAngles;
        //posicion = bTransform.position;

        //oldTimestamp = curTimestamp;
        //curTimestamp = GetTimestampInMicroseconds();
        //timeDif = (curTimestamp - oldTimestamp);

        //velXOld = velX;
        //velYOld = velY;
        //velZOld = velZ;

        //velX = Math.Round(HPlugin.CurrentVelocity[0], 2);
        //velY = Math.Round(HPlugin.CurrentVelocity[1], 2);
        //velZ = Math.Round(HPlugin.CurrentVelocity[2], 2);

        //oldAngleX = curAngleX;
        //curAngleX = HPlugin.CollisionMesh.GetComponent<Rigidbody>().angularVelocity.x;

        //oldAngleY = curAngleY;
        //curAngleY = HPlugin.JointAngles.y;

        //oldAngleZ = curAngleZ;
        //curAngleZ = HPlugin.JointAngles.z;

        //velocidadAngularX = ((float)(curAngleX - oldAngleX) / (timeDif)) * 1000000;
        //velocidadAngularY = ((float)(curAngleY - oldAngleY) / (timeDif)) * 1000000;
        //velocidadAngularZ = ((float)(curAngleZ - oldAngleZ) / (timeDif)) * 1000000; 

        //Debug.Log(collisionMeshRB.velocity.x.ToString("0000.00;-000.00") +
        //            ":" +
        //            collisionMeshRB.velocity.y.ToString("0000.00;-000.00") +
        //            ":" +
        //            collisionMeshRB.velocity.z.ToString("0000.00;-000.00") +
        //            ":" +
        //            collisionMeshRB.angularVelocity.x.ToString("0000.00;-000.00") +
        //            ":" +
        //            collisionMeshRB.angularVelocity.y.ToString("0000.00;-000.00") +
        //            ":" +
        //            collisionMeshRB.angularVelocity.z.ToString("0000.00;-000.00") +
        //            ";");

        //if (OPCUAIsConnected && !readyToMove)
        //{
        //    robotTCPPos[0] = HapticCollider.transform.position.x;
        //    robotTCPPos[1] = HapticCollider.transform.position.y;
        //    robotTCPPos[2] = HapticCollider.transform.position.z;

        //    robotTCPRot[0] = HapticCollider.transform.rotation.eulerAngles[0];
        //    robotTCPRot[1] = HapticCollider.transform.rotation.eulerAngles[1];
        //    robotTCPRot[2] = HapticCollider.transform.rotation.eulerAngles[2];

        //    readyToMove = true;
        //}

        if (OPCUAIsConnected && readyToMove)
        {
            // Only when speed-based is active
            if (!positionBased)
            {


                // Calcula el desplazamiento desde la última posición
                Vector3 desplazamiento = HapticCollider.transform.position - ultimaPosicion;

                // Calcula la velocidad lineal (velocidad en unidades por segundo)
                Vector3 velocidadLineal = desplazamiento / (Time.time - tiempoAnterior);
                velocidadLineal *= ScaleFactorPosition;
                // Calcula el cambio en la rotación desde la última rotación
                Vector3 cambioRotacion = new Vector3((float)eulerGet.EulerX, (float)eulerGet.EulerY, (float)eulerGet.EulerZ) - ultimaRotacion;

                // Convierte la velocidad angular a grados por segundo en Vector3
                Vector3 velocidadAngularGrados = cambioRotacion / (Time.time - tiempoAnterior);

                // Actualiza la última posición y rotación
                ultimaPosicion = HapticCollider.transform.position;
                ultimaRotacion = new Vector3((float)eulerGet.EulerX, (float)eulerGet.EulerY, (float)eulerGet.EulerZ);

                // Actualiza el tiempo anterior
                tiempoAnterior = Time.time;

                // Imprime las velocidades en Vector3
                Debug.Log("Velocidad Lineal: " + velocidadLineal + " unidades por segundo");
                Debug.Log("Velocidad Angular: " + velocidadAngularGrados + " grados por segundo");

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


                if (!HPlugin.bIsRelease)
                {
                    data = Encoding.UTF8.GetBytes(velocidadLineal.x.ToString("0000.00;-000.00") +
                                                    ":" +
                                                    velocidadLineal.y.ToString("0000.00;-000.00") +
                                                    ":" +
                                                    velocidadLineal.z.ToString("0000.00;-000.00") +
                                                    ":" +
                                                    velocidadAngularGrados.x.ToString("0000.00;-000.00") +
                                                    ":" +
                                                    velocidadAngularGrados.y.ToString("0000.00;-000.00") +
                                                    ":" +
                                                    velocidadAngularGrados.z.ToString("0000.00;-000.00") +
                                                    ";");
                }

                // Crear el punto final de destino
                //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                // Enviar el array de bytes al ordenador receptor
                socket.SendTo(data, endPoint);

                // Cerrar el socket
                //socket.Close();

                //// Check if there is any data available to receive from the UDP socket.
                //if (receiveUdpClient.Available > 0)
                //{
                //    // Receive the data from the UDP socket and store it in a byte array.
                //    byte[] receiveBytes = receiveUdpClient.Receive(ref receiveRemoteEndPoint);

                //    // Convert the byte array to a string and store it in the response variable.
                //    receiveMessage = Encoding.ASCII.GetString(receiveBytes);

                //    // Print the response to the console.
                //    Debug.Log("This is the message you received: " + receiveMessage);
                //    Debug.Log("This message was sent from: " + receiveRemoteEndPoint.Address.ToString() + " on their port number: " + receiveRemoteEndPoint.Port.ToString());
                //}

            }
            // Only when position-based is active
            else
            {

                //CalculateButtonMean(ref buttonValues, ref buttonMean);

                //buttonMean = true; // siempre true para falsear la entrada en el if

                //// if button is pressed
                //if (buttonMean)
                //{

                //CalculateMeans();

                // Calculating distance between new Position and Rotation
                // and initial Position and Rotation

                Vector3 currentEndPointPosition = new Vector3((float)positionX.myvar, (float)positionY.myvar, (float)positionZ.myvar);

                var distancePosX = HapticCollider.transform.position.x - robotTCPPos[0];
                var distancePosY = HapticCollider.transform.position.y - robotTCPPos[1];
                var distancePosZ = HapticCollider.transform.position.z - robotTCPPos[2];

                var distanceRotX = HapticCollider.transform.rotation.eulerAngles.x - robotTCPRot[0];
                var distanceRotY = HapticCollider.transform.rotation.eulerAngles.y - robotTCPRot[1];
                var distanceRotZ = HapticCollider.transform.rotation.eulerAngles.z - robotTCPRot[2];

                //distancePosX *= ScaleFactorPosition;
                //distancePosY *= ScaleFactorPosition;
                //distancePosZ *= ScaleFactorPosition;

                distancePos = distancePos.normalized;
                distancePos = new Vector3((currentEndPointPosition.x- robotTCPPosReal[0]) *ScaleFactorPosition + distancePosX, (currentEndPointPosition.y - robotTCPPosReal[1]) * ScaleFactorPosition + distancePosY, (currentEndPointPosition.z - robotTCPPosReal[2]) * ScaleFactorPosition + distancePosZ);

                Debug.Log(distancePos);

                byte[] data;

                data = Encoding.UTF8.GetBytes(distancePos.x.ToString("0000.00;-000.00") +
                                                ":" +
                                                distancePos.y.ToString("0000.00;-000.00") +
                                                ":" +
                                                distancePos.z.ToString("0000.00;-000.00") +
                                                ":" +
                                                distanceRotX.ToString("0000.00;-000.00") +
                                                ":" +
                                                distanceRotY.ToString("0000.00;-000.00") +
                                                ":" +
                                                distanceRotZ.ToString("0000.00;-000.00") +
                                                ";");

                //Debug.Log(distancePosX.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                distancePos.x.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                distancePos.y.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                distancePos.z.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                distanceRotX.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                distanceRotY.ToString("0000.00;-000.00") +
                //                                ":" +
                //                                distanceRotZ.ToString("0000.00;-000.00") +
                //                                ";");



                // Enviar el array de bytes al ordenador receptor
                socket.SendTo(data, endPoint);

                // Cerrar el socket
                //socket.Close();
                //}
                //// if button is NOT pressed
                //else
                //{
                //    HapticCollider.transform.position = hapticColliderPositionMean;
                //    HapticCollider.transform.rotation = Quaternion.Euler(hapticColliderRotationMean);
                //    SimpleStylus.transform.position = simpleStylusPositionMean;
                //    SimpleStylus.transform.rotation = Quaternion.Euler(simpleStylusRotationMean); ;
                //}

            }

        }

    }

#if UNITY_EDITOR

    //float CalcAngle(float startPos, float endPos)
    //{
    //    float result = 0;

    //    if(startPos < 0 && endPos < 0)
    //    {
    //        result = Mathf.Abs(endPos - startPos);
    //    }else if(startPos < 0 && endPos > 0)
    //    {
    //        result = endPos + Mathf.Abs(startPos);
    //    }
    //    else
    //    {
    //        result = Mathf.Abs(endPos - startPos);
    //    }

    //    return result;
    //}
    
    //void DrawNavSpeedArea(GameObject joint, Vector3 jnormal ,Vector3 rotAxis, Vector3 directionVec, Vector2 zone0, Vector2 zone1, Vector2 zone2, bool isBlocked)
    //{
    //    Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);

        
    //    Vector3 jointDZ;

    //    if (isBlocked==false)
    //    {

    //        jointDZ = Quaternion.AngleAxis(zone0.x, rotAxis) * directionVec;



    //        Handles.DrawSolidArc(joint.transform.position,
    //                joint.transform.worldToLocalMatrix * jnormal,
    //                jointDZ,
    //                CalcAngle(zone0.x, zone0.y),
    //                0.015f);

    //        Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.2f);
    //        jointDZ = Quaternion.AngleAxis(zone1.x, rotAxis) * directionVec;


    //        Handles.DrawSolidArc(joint.transform.position,
    //                joint.transform.worldToLocalMatrix * jnormal,
    //                jointDZ,
    //                CalcAngle(zone1.x, zone1.y),
    //                0.015f);

    //        jointDZ = Quaternion.AngleAxis(zone2.x, rotAxis) * directionVec;


    //        Handles.DrawSolidArc(joint.transform.position,
    //                joint.transform.worldToLocalMatrix * jnormal,
    //                jointDZ,
    //                CalcAngle(zone2.x, zone2.y),
    //                0.015f);

    //    }
    //    else
    //    {
    //        jointDZ =  directionVec;



    //        Handles.DrawSolidArc(joint.transform.position,
    //                joint.transform.worldToLocalMatrix * jnormal,
    //                jointDZ,
    //                360.0f,
    //                0.015f);
    //    }


        
    //}

    //void OnDrawGizmos()
    //{
                
    //    if (ShowGizmo)
    //    {
    //        DrawNavSpeedArea(joint0, joint0.transform.up, Vector3.up, Vector3.forward, HPlugin.SliderTXZ0, HPlugin.SliderTXZ1n, HPlugin.SliderTXZ1p, HPlugin.FreezeTranslation.HasFlag(HapticPlugin.Axis.X));
    //        DrawNavSpeedArea(joint1, joint1.transform.right, Vector3.right, Vector3.up, HPlugin.SliderTYZ0, HPlugin.SliderTYZ1n, HPlugin.SliderTYZ1p, HPlugin.FreezeTranslation.HasFlag(HapticPlugin.Axis.Y));
    //        DrawNavSpeedArea(joint2, joint2.transform.right, Vector3.right, Vector3.up, HPlugin.SliderTZZ0, HPlugin.SliderTZZ1n, HPlugin.SliderTZZ1p, HPlugin.FreezeTranslation.HasFlag(HapticPlugin.Axis.Z));
    //        DrawNavSpeedArea(joint3, joint3.transform.up, Vector3.up, Vector3.forward, HPlugin.SliderRXZ0, HPlugin.SliderRXZ1n, HPlugin.SliderRXZ1p, HPlugin.FreezeRotation.HasFlag(HapticPlugin.Axis.X));
    //        DrawNavSpeedArea(joint4, joint4.transform.right, Vector3.right, Vector3.up, HPlugin.SliderRYZ0, HPlugin.SliderRYZ1n, HPlugin.SliderRYZ1p, HPlugin.FreezeRotation.HasFlag(HapticPlugin.Axis.Y));
    //        DrawNavSpeedArea(joint5, joint5.transform.forward, Vector3.forward, Vector3.up, HPlugin.SliderRZZ0, HPlugin.SliderRZZ1n, HPlugin.SliderRZZ1p, HPlugin.FreezeRotation.HasFlag(HapticPlugin.Axis.Z));

    //        Gizmos.color = Color.white;
    //        Vector3 direction;

    //        direction = Quaternion.AngleAxis(HPlugin.JointAngles[0], Vector3.up) * Vector3.forward - joint0.transform.position;
    //        Gizmos.DrawLine(joint0.transform.position, joint0.transform.position + (direction.normalized * 0.06f));

    //        direction = Quaternion.AngleAxis(HPlugin.JointAngles[1] * -1.0f, Vector3.right) * Vector3.forward + joint1.transform.position;
    //        Gizmos.DrawLine(joint1.transform.position, joint1.transform.position + (direction.normalized * 0.06f));

    //        direction = Quaternion.AngleAxis((HPlugin.JointAngles[2]- HPlugin.JointAngles[1]) * -1.0f, Vector3.right) * Vector3.forward - joint2.transform.position;
    //        Gizmos.DrawLine(joint2.transform.position, joint2.transform.position + (direction.normalized * 0.02f));

    //        direction = Quaternion.AngleAxis(HPlugin.GimbalAngles[0] * -1.0f, Vector3.up) * Vector3.forward - joint3.transform.position;
    //        Gizmos.DrawLine(joint3.transform.position, joint3.transform.position + (direction.normalized * 0.02f));

    //        direction = Quaternion.AngleAxis(HPlugin.GimbalAngles[1] * -1.0f, Vector3.right) * Vector3.up - joint4.transform.position;
    //        Gizmos.DrawLine(joint4.transform.position, joint4.transform.position + (direction.normalized * 0.02f));

    //        direction = Quaternion.AngleAxis(HPlugin.GimbalAngles[2], Vector3.forward) * Vector3.up - joint5.transform.position;
    //        Gizmos.DrawLine(joint5.transform.position, joint5.transform.position + (direction.normalized * 0.02f));
    //    }

    //    if (ShowLabels)
    //    {

    //        Handles.Label(joint0.transform.position, "JX Angle: " + HPlugin.JointAngles[0]);
    //        Handles.Label(joint1.transform.position, "JY Angle: " + HPlugin.JointAngles[1]);
    //        Handles.Label(joint2.transform.position, "JZ Angle: " + (HPlugin.JointAngles[2]-HPlugin.JointAngles[1]));
    //        Handles.Label(joint3.transform.position, "GX Angle: " + HPlugin.GimbalAngles[0]);
    //        Handles.Label(joint4.transform.position, "GY Angle: " + HPlugin.GimbalAngles[1]);
    //        Handles.Label(joint5.transform.position, "GZ Angle: " + HPlugin.GimbalAngles[2]);
            
    //    }

    //}


    //public static long GetTimestampInMicroseconds()
    //{
    //    // Obtener el número de ticks desde el 1 de enero de 0001 a las 00:00:00 UTC
    //    long ticks = DateTime.UtcNow.Ticks;
    //    // Convertir los ticks en microsegundos
    //    long microseconds = (long)ticks / 10;
    //    // Devolver el valor
    //    return microseconds;
    //}
#endif

    public void SetOPCUAState(bool active)
    {
        OPCUAIsConnected = active;
    }

    private void OnDisable()
    {
        socket.Close();
        receiveUdpClient.Close();
    }

    private void OnDestroy()
    {
        socket.Close();
        receiveUdpClient.Close();
    }

    // Close the UDP socket when the application quits.
    void OnApplicationQuit()
    {
        socket.Close();
        receiveUdpClient.Close();
    }

    public void GetInitialPosTCP()
    {
        if (OPCUAIsConnected)
        {
            Vector3 currentEndPointPosition = new Vector3((float)positionX.myvar, (float)positionY.myvar, (float)positionZ.myvar);

            robotTCPPos[0] = EndPointStaubli.transform.position.x; 
            robotTCPPos[1] = EndPointStaubli.transform.position.y;
            robotTCPPos[2] = EndPointStaubli.transform.position.z; 

            robotTCPPosReal[0] = currentEndPointPosition.x;
            robotTCPPosReal[1] = currentEndPointPosition.y;
            robotTCPPosReal[2] = currentEndPointPosition.z;

            robotTCPRot[0] = EndPointStaubli.transform.rotation.eulerAngles[0];
            robotTCPRot[1] = EndPointStaubli.transform.rotation.eulerAngles[1];
            robotTCPRot[2] = EndPointStaubli.transform.rotation.eulerAngles[2];

            Touch.transform.position = EndPointStaubli.transform.position;

            readyToMove = true;

            Debug.Log("ReadyToMove");
        }
    }
}





