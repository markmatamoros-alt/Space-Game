using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using Firesplash.UnityAssets.SocketIO;

//using Socket.Quobject.SocketIoClientDotNet.Client;

public class APICommunication : MonoBehaviour
{
    public SocketIOCommunicator socket;

    public MainSceneController main;
    //private QSocket socket;
    public void SetupSocket()
    {
        socket.socketIOAddress = Settings.APIaddress;
        socket.Instance.On("connect", (string data) =>
        {
            Debug.Log("Connected to socket on " + Settings.APIaddress);

            //NOTE: All those emitted and received events (except connect and disconnect) are made to showcase how this asset works. The technical handshake is done automatically.

            //First of all we knock at the servers door
            //EXAMPLE 1: Sending an event without payload data
            //SocketEmit("testMessage", "{\"msg\":\"ok\"}");
            //SocketEmit("testMessage", "{\"gameAPid\":" + 0 + ", \"groupID\":" + 0 + "}");
        });

        socket.Instance.On("gameStart", (string data) =>
        {
            Debug.Log("Server: " + data);
            string datastring = data.ToString();
            SimpleJSON.JSONNode jsonInfo = SimpleJSON.JSON.Parse(datastring);

            if (jsonInfo["apid"] == Settings.APIAppId)
            {
                main.Reset();
                main.groupID = jsonInfo["groupID"];
                main.requiredPlayers = jsonInfo["players"];
                main.requiredPlayers=Mathf.Clamp(main.requiredPlayers,3,6);
            }
        });

        socket.Instance.On("gameStatus", (string data) =>
        {
            Debug.Log("Server: " + data);
            string datastring = data.ToString();
            SimpleJSON.JSONNode jsonInfo = SimpleJSON.JSON.Parse(datastring);
            Debug.Log("this app id:"+Settings.APIAppId+" | received app id:"+jsonInfo["gameApid"] );
            Debug.Log("app id:"+jsonInfo["gameApid"] );

            if (jsonInfo["gameApid"] == Settings.APIAppId)
            {
                main.SendGameStatusToAPI();
            }
        });

        socket.Instance.On("disconnect", (string payload) =>
        {
            if (payload.Equals("io server disconnect"))
            {
                Debug.Log("Disconnected from socket on " + Settings.APIaddress);
            }
            else
            {
                Debug.LogWarning("We have been unexpecteldy disconnected. This will cause an automatic reconnect. Reason: " + payload);
            }
        });

        socket.Instance.Connect();

        /*
        Debug.Log("Setting Up Socket");
        socket = IO.Socket(Settings.APIaddress);
        socket.On(QSocket.EVENT_CONNECT, () =>
        {
            Debug.Log("Connected to" + Settings.APIaddress);
            //socket.Emit("chat", "Thank You");
            socket.On("gameStart", data =>
            {
                Debug.Log("Server: " + data);
                string datastring = data.ToString();
                SimpleJSON.JSONNode jsonInfo = SimpleJSON.JSON.Parse(datastring);

                if (jsonInfo["apid"] == Settings.APIAppId)
                {
                    main.WaitingForPlayers();
                    main.groupID = jsonInfo["groupID"];
                    main.requiredPlayers = jsonInfo["players"];
                }
            });

            socket.On("gameStatus", data =>
            {
                Debug.Log("Server: " + data);
                string datastring = data.ToString();
                SimpleJSON.JSONNode jsonInfo = SimpleJSON.JSON.Parse(datastring);


                if (jsonInfo["gameApid"] == Settings.APIAppId)
                {
                    main.GetGameStatus();
                }

            });




        });
        */
        //string retrievedText = "From Server: gameStatus {\"gameApid\":\"101\"}";
        /*
        string retrievedText ="From Server: gameStart {\"apid\":3,\"players\":3,\"groupID\":52858,\"experienceid\":1,\"groupData\":[{\"rfid_id\":56861,\"rfid\":23688,\"fname\":\"Tiffany\",\"lname\":\"Ziemann\",\"accesslevel\":1,\"lastupdate\":\"2021-02-26T19:41:21.000Z\",\"additionaldata\":\"\",\"prize\":null,\"prizevended\":0,\"preregid\":null,\"email\":\"Cecile_Lueilwitz@hotmail.com\",\"phone\":\"597.791.6293\",\"ride\":\"\",\"rfidGroupId\":52858},{\"rfid_id\":23463,\"rfid\":45788,\"fname\":\"Lola\",\"lname\":\"Hayes\",\"accesslevel\":1,\"lastupdate\":\"2021-02-26T19:41:21.000Z\",\"additionaldata\":\"\",\"prize\":null,\"prizevended\":0,\"preregid\":null,\"email\":\"Raoul.Toy@hotmail.com\",\"phone\":\"1-524-271-2428 x4804\",\"ride\":\"\",\"rfidGroupId\":52858},{\"rfid_id\":15930,\"rfid\":88814,\"fname\":\"Gregory\",\"lname\":\"Rolfson\",\"accesslevel\":1,\"lastupdate\":\"2021-02-26T19:41:21.000Z\",\"additionaldata\":\"\",\"prize\":null,\"prizevended\":0,\"preregid\":null,\"email\":\"Jett77@hotmail.com\",\"phone\":\"1-978-942-1540 x66517\",\"ride\":\"\",\"rfidGroupId\":52858}]}";
        string[] splitArray = retrievedText.Split(char.Parse("{"));
        string json = "{" + splitArray[1];
        string[] splitFunction = splitArray[0].Split(char.Parse(":"));
        string functionText = splitFunction[1];
        functionText = functionText.Replace(" ", "");
        Debug.Log(functionText);
        Debug.Log(json);*/
    }
    public IEnumerator GetRequestFromAPI()
    {

        UnityWebRequest webRequest = UnityWebRequest.Get(Settings.APIaddress);
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.LogError(webRequest.error);
            yield break;
        }


        Debug.Log("Received from API:" + webRequest.downloadHandler.text);
        //From Server: gameStatus {"gameApid":"101"}
        string retrievedText = webRequest.downloadHandler.text;
        string[] splitArray = retrievedText.Split(char.Parse("{"));
        string json = "{" + splitArray[1];
        string[] splitFunction = splitArray[0].Split(char.Parse(":"));
        string functionText = splitFunction[1];
        functionText = functionText.Replace(" ", "");
        Debug.Log(functionText);
        Debug.Log(json);
        SimpleJSON.JSONNode jsonInfo = SimpleJSON.JSON.Parse(webRequest.downloadHandler.text);
        if (jsonInfo["gameStart"] != null)
        {
            if (jsonInfo["apid"] == Settings.APIAppId)
            {
                main.WaitingForPlayers();
                main.groupID = jsonInfo["groupID"];
                main.requiredPlayers = jsonInfo["players"];
            }
        }
        if (jsonInfo["gameStatus"] != null)
        {
            if (jsonInfo["gameApid"] == Settings.APIAppId)
            {
                main.SendGameStatusToAPI();
            }
        }


    }

    public void SocketEmit(string function, string json)
    {
        SimpleJSON.JSONNode jsonObject = SimpleJSON.JSON.Parse(json);
        socket.Instance.Emit(function, json, false);
    }
    public IEnumerator Post(string bodyJsonString)
    {

        var request = new UnityWebRequest(Settings.APIaddress, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
    }
    /*
        public async void CheckAPIStatus()
        {
            int players = (await GetGameStart()).players;
            Debug.Log("got result" + players);

        }

        private async Task<GameStartAPI> GetGameStart()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(Settings.APIaddress));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            GameStartAPI info = JsonUtility.FromJson<GameStartAPI>(jsonResponse);
            return info;
        }

        public IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;
                }
            }
        }*/
}
