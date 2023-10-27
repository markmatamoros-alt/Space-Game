

/*
 
    -----------------------
    UDP-Send
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
    // > gesendetes unter
    // 127.0.0.1 : 8050 empfangen
   
    // nc -lu 127.0.0.1 8050
 
        // todo: shutdown thread at the end
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{
    public bool Debug;
    public bool DebugInSameMachine;
    public bool isServer;

    private static int localPort;

    // prefs
    private string[] IP;  // define in init
    public int[] port;  // define in init

    // "connection" things
    IPEndPoint[] remoteEndPoint;
    private UdpClient[] client;

    // gui
    string strMessage = "";


    // call it from shell (as program)
    private static void Main()
    {
        UDPSend sendObj = new UDPSend();
        sendObj.init();

        // testing via console
        // sendObj.inputFromConsole();

        // as server sending endless
        sendObj.sendEndless(" endless infos \n");

    }
    // start from unity3d
    public void Start()
    {
        Debug = Settings.showDebug;
        //init();
    }

    // OnGUI
    void OnGUI()
    {
        if (Debug)
        {
            Rect rectObj = new Rect(40, 380, 200, 400);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;

            style.alignment = TextAnchor.UpperLeft;
            GUI.color = Color.white;
            GUI.Box(rectObj, "# UDPSend-Data\n" + IP[0] + ":" + port[0] + " #\n", style);

            // ------------------------
            // send it
            // ------------------------
            float y = 420;
            if (isServer)
                y = 360;
            strMessage = GUI.TextField(new Rect(40, y, 140, 20), strMessage);
            if (GUI.Button(new Rect(190, y, 40, 20), "send"))
            {
                sendString(strMessage + "\n");
            }
        }
    }

    // init
    public void init()
    {
        // Endpunkt definieren, von dem die Nachrichten gesendet werden.
        print("UDPSend.init()");

        // define

        if (isServer)
        {
            IP = new string[6];
            port = new int[6];
            client = new UdpClient[6];
            remoteEndPoint = new IPEndPoint[6];

            for (int i = 0; i < port.Length; i++)
            {
                IP[i] = Settings.deviceIP[i];
                port[i] = Settings.port + (i + 1);

            }
        }
        else
        {

            IP = new string[1];
            port = new int[1];
            client = new UdpClient[1];
            remoteEndPoint = new IPEndPoint[1];
            IP[0] = Settings.deviceIP[0];
            port[0] = Settings.port;
        }

        /*
                // define port
                if (DebugInSameMachine)
                {
                    if (isServer)
                    {
                        port = Settings.port + 1;
                    }
                    else
                    {
                        port = Settings.port;
                    }
                }
                else
                {
                    port = Settings.port;

                }*/


        print("UDPSend port" + port);
        // ----------------------------
        // Senden
        // ----------------------------
        for (int i = 0; i < port.Length; i++)
        {
            remoteEndPoint[i] = new IPEndPoint(IPAddress.Parse(IP[i]), port[i]);
            client[i] = new UdpClient();
        }
        //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        //client.EnableBroadcast=true;
        //client.Connect(remoteEndPoint);
        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }

    // inputFromConsole
    private void inputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();

                // Den Text zum Remote-Client senden.
                if (text != "")
                {

                    // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
                    byte[] data = Encoding.UTF8.GetBytes(text);

                    // Den Text zum Remote-Client senden.
                    for (int i = 0; i < client.Length; i++)
                    {
                        // Den message zum Remote-Client senden.
                        client[i].Send(data, data.Length, remoteEndPoint[i]);
                    }
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    public void sendString(string message)
    {
        try
        {
            //if (message != "")
            //{

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);

            for (int i = 0; i < client.Length; i++)
            {
                print("Sending to"+IP[i]+":"+port[i]);
                // Den message zum Remote-Client senden.
                client[i].Send(data, data.Length, remoteEndPoint[i]);
            }
            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    public void sendString(string message, int _which)
    {
        try
        {
            //if (message != "")
            //{

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);


            // Den message zum Remote-Client senden.
            client[_which].Send(data, data.Length, remoteEndPoint[_which]);
                print("Sending to"+IP[_which]+":"+port[_which]);

            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }


    private void OnDestroy()
    {
        for (int i = 0; i < client.Length; i++)
        {
            client[i]?.Dispose();
        }
    }
    // endless test
    private void sendEndless(string testStr)
    {
        do
        {
            sendString(testStr);


        }
        while (true);

    }

}

