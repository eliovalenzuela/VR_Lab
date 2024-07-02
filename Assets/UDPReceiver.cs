using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    // Puerto en el que el socket estará escuchando
    public int port = 10;
    public double Fx, Fy, Fz, Tx, Ty, Tz;

    // Hilo para recibir datos en segundo plano
    private Thread receiveThread;

    // Socket UDP
    private UdpClient udpClient;

    // Variable para almacenar datos recibidos
    private string receivedData;

    void Start()
    {
        // Inicializar el socket UDP y el hilo de recepción
        InitUDP();
    }

    void InitUDP()
    {
        // Crear un hilo nuevo para recibir datos
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log($"UDP Receiver iniciado en el puerto {port}");
    }

    void ReceiveData()
    {
        udpClient = new UdpClient(port);
        while (true)
        {
            try
            {
                // Dirección IP de cualquier origen (0.0.0.0) y el puerto específico
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

                // Recibir datos
                byte[] data = udpClient.Receive(ref remoteEndPoint);

                // Convertir los datos recibidos a string
                // Iteramos sobre los 6 conjuntos de valores (0 a 5).
                for (int j = 0; j <= 5; j++)
                {
                    int bytenum = 0;
                    int[,] HEXNUMBER = new int[4, 2];

                    // Conversión de dec -> hex
                    while (bytenum < 4)
                    {
                        int index = (4 * j) + bytenum;
                        int cociente = data[index];

                        // Convertimos el byte a dos dígitos hexadecimales.
                        for (int i = 0; i <= 1; i++)
                        {
                            HEXNUMBER[bytenum, i] = cociente % 16;
                            cociente = Mathf.FloorToInt(cociente / 16);
                        }
                        bytenum++;
                    }

                    // Conversión de hex -> dec
                    int DECNUMBER = 0;
                    bool isNegative = HEXNUMBER[0, 0] == 15;

                    for (int i = 0; i < 8; i++)
                    {
                        int nibble = HEXNUMBER[3 - (i / 2), i % 2];

                        if (isNegative)
                        {
                            nibble = 15 - nibble;  // Complemento a 1
                        }

                        DECNUMBER += nibble * (int)Mathf.Pow(16, i);
                    }

                    if (isNegative)
                    {
                        DECNUMBER = -DECNUMBER;
                    }

                    // Asignamos el resultado al valor correspondiente de F/T con su factor de escala.
                    switch (j)
                    {
                        case 0:
                            Fx = DECNUMBER / 10000f;
                            break;
                        case 1:
                            Fy = DECNUMBER / 10000f;
                            break;
                        case 2:
                            Fz = DECNUMBER / 10000f;
                            break;
                        case 3:
                            Tx = DECNUMBER / 100000f;
                            break;
                        case 4:
                            Ty = DECNUMBER / 100000f;
                            break;
                        case 5:
                            Tz = DECNUMBER / 100000f;
                            break;
                        default:
                            break;
                    }
                }

                // Logeamos los resultados para verificar.
                Debug.Log($"Fx: {Fx}, Fy: {Fy}, Fz: {Fz}, Tx: {Tx}, Ty: {Ty}, Tz: {Tz}");

                // Logear los datos recibidos (puede ser removido para producción)
                Debug.Log($"Datos recibidos: {receivedData}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error al recibir datos: {e.Message}");
            }
        }
    }

    void OnDisable()
    {
        // Limpiar y cerrar el socket cuando el objeto es desactivado
        if (udpClient != null)
        {
            udpClient.Close();
        }

        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
    }

    // Método para acceder a los datos recibidos desde otros scripts
    public string GetReceivedData()
    {
        return receivedData;
    }
}
