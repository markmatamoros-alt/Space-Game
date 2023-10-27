using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
public class CommunicationManager : MonoBehaviour
{
    public bool isServer;
    public MainSceneController mainSceneController;
    public PlayerSceneController playerSceneController;
    string IP;
    int port;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    Thread receiveThread;

    string lastReceivedUDPPacket = "";
    string allReceivedUDPPackets = "";


    void Start()
    {
        init();

    }
    public void Setup()
    {

        IP = Settings.ip;
        port = Settings.port;

        if (isServer)
        {


            remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
            client = new UdpClient();

        }
        else
        {
            receiveThread = new Thread(
 new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();

        }




    }
    
    private static void Main()
    {
       UDPReceive receiveObj=new UDPReceive();
       receiveObj.init();
 
        string text="";
        do
        {
             text = Console.ReadLine();
        }
        while(!text.Equals("exit"));
    }

    private void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");
       
        // define port
        port = 8051;
 
        // status
        print("Sending to 127.0.0.1 : "+port);
        print("Test-Sending to this Port: nc -u 127.0.0.1  "+port+"");
 
   
        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
 
    }
 


    private void ReceiveData()
    {
        Debug.Log("Receive Data");
        if(!isServer)
        client = new UdpClient(port);
        while (true)
        {

            try
            {
                // Bytes empfangen.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // Bytes mit der UTF8-Kodierung in das Textformat kodieren.
                string text = Encoding.UTF8.GetString(data);

                // Den abgerufenen Text anzeigen.
                print(">> " + text);
                Debug.Log(text);
                // latest UDPpacket
                lastReceivedUDPPacket = text;
                if (isServer)
                {
                    mainSceneController.ReceiveMessage(lastReceivedUDPPacket);
                }
                else
                {
                    playerSceneController.ReceiveMessage(lastReceivedUDPPacket);

                }

                // ....
                //allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {

                Debug.Log(err.ToString());
            }
        }
    }

    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }

    public void SendString(string message)
    {
        Debug.Log("Send String");

        try
        {
            //if (message != "")
            //{

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }

}
