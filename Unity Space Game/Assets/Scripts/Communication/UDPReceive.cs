/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{


    public bool Debug;
    public bool DebugInSameMachine;

    public bool isServer;
    // receiving Thread
    private Thread receiveThread;

    // udpclient object
    private UdpClient client;

    // public
    // public string IP = "127.0.0.1"; default local
    public int port; // define > init

    // infos
    public string lastReceivedUDPPacket = "";
    public List<string> allReceivedUDPPackets; // clean up this from time to time!


    // start from shell

    // start from unity3d
    public void Start()
    {
        allReceivedUDPPackets = new List<string>();
        Debug = Settings.showDebug;

        //init();
    }

    // OnGUI
    void OnGUI()
    {
        if (Debug)
        {

            Rect rectObj = new Rect(40, 10, 200, 400);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;

            style.alignment = TextAnchor.UpperLeft;
            GUI.Box(rectObj, "# UDPReceive\n any IP " + port + " #\n"
                        + "\nLast Packet: \n" + lastReceivedUDPPacket
                        + "\n\nAll Messages: \n" + allReceivedUDPPackets.Count.ToString()
                    , style);
        }
    }

    // init
    public void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        // print("UDPSend.init()");

        // define port
        /*  if (DebugInSameMachine)
          {
              if (isServer)
              {
                  port = Settings.port;
              }
              else
              {
                  port = Settings.port + 1;
              }
          }
          else
          {
              port = Settings.port;

          }*/

        // print("UDPReceive port" + port);

        // status
        // print("Sending to 127.0.0.1 : " + port);
        // print("Test-Sending to this Port: nc -u 127.0.0.1  " + port + "");


        // ----------------------------
        // Abhören
        // ----------------------------
        // Lokalen Endpunkt definieren (wo Nachrichten empfangen werden).
        // Einen neuen Thread für den Empfang eingehender Nachrichten erstellen.
        port = Settings.portDevice;

        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Thread.Sleep(100);
        //Thread

    }

    // receive thread
    bool portInUse;
    private void ReceiveData()
    {
        /*
        for (int i = 0; i < System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().Length; i++)
        {
            //print("Port " + System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners()[i].Port);

            if (System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners()[i].Port == port)
            {
                portInUse = true;
                //print("Port in use");

                return;
            }
            else
            {
                portInUse = false;
                // print("Port available");
            }
        }
        if (!portInUse)
        {*/
        client = new UdpClient(port);
        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //client.EnableBroadcast=true;
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

                // latest UDPpacket
                lastReceivedUDPPacket = text;

                allReceivedUDPPackets.Add(text);

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
        // }
    }

    private void OnDestroy()
    {
        client?.Dispose();
        receiveThread?.Abort();


    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets.Clear();
        return lastReceivedUDPPacket;
    }

    public void ClearPackets()
    {
        allReceivedUDPPackets.Clear();

    }
}