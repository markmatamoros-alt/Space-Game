using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerGameControls : MonoBehaviour
{
    public PlayerSceneController playerSceneController;
    public int gridX = 5, gridY = 2;
    public bool[,] gridOccupied;
    public Transform container;
    public GameObject controlPrefab;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (Settings.showDebug)
        {
           //DebugSetupContainer();
        }

    }

    void DebugSetupContainer()
    {
        playerSceneController.thisDevice.digitalControls = new List<DigitalControl>();
        ReadFromXML.LoadSettings();
        ReadFromXML.LoadControlNames();
        ReadFromXML.LoadDigitalControls();
        int amount = Random.Range(1, 7);
        //amount = 1;
        for (int i = 0; i < amount; i++)
        {
            DigitalControl.ControlType[] types = { DigitalControl.ControlType.button, DigitalControl.ControlType.fader, DigitalControl.ControlType.knob, DigitalControl.ControlType.switchtype };
            int randomType = Random.Range(0, types.Length);
            const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want
            string myString = "";
            int charAmount = Random.Range(4, 32); //set those to the minimum and maximum length of your string
            for (int j = 0; j < charAmount; j++)
            {
                myString += glyphs[Random.Range(0, glyphs.Length)];
            }
            myString=Database.ControlNames[Random.Range(0,Database.ControlNames.Length)];
            playerSceneController.thisDevice.digitalControls.Add(new DigitalControl(i, myString, types[randomType]));
        }
        Debug.Log("Number of controls:" + playerSceneController.thisDevice.digitalControls.Count);
        SetupContainer();
    }
    public void SetupContainer()
    {
        float width = container.GetComponent<RectTransform>().sizeDelta.x;
        float height = container.GetComponent<RectTransform>().sizeDelta.y;
        float tileW = width / gridX;
        float tileH = height / gridY;
        Debug.Log(width + "," + tileH);
        gridOccupied = new bool[gridX, gridY];
        int currentControl = 0;
        bool sizeBlocksOccupied = false;
        bool blockPassingLimits = false;
        while (CheckIfAllGridsOccupied() == false || currentControl < playerSceneController.thisDevice.digitalControls.Count || sizeBlocksOccupied || blockPassingLimits)
        {
            //Debug.Log("Restarting While Loop");
            ClearContainer();

            gridOccupied = new bool[gridX, gridY];
            currentControl = 0;
            sizeBlocksOccupied = false;
            blockPassingLimits = false;

            for (int x = 0; x < gridX; x++)
            {
                for (int y = 0; y < gridY; y++)
                {
                    if (!gridOccupied[x, y])
                    {
                        if (currentControl < playerSceneController.thisDevice.digitalControls.Count)
                        {
                            //set block random size
                            int w, h;
                            w = Random.Range(1, gridX + 1);
                            h = Random.Range(1, gridY + 1);

                            //if in the corner, the size will be 1
                            if (y == gridY - 1)
                            {
                                h = 1;
                            }
                            if (x == gridX - 1)
                            {
                                w = 1;
                            }
                            //Debug.Log("x:" + x + " y:" + y + " w:" + w + " h:" + h);


                            if (w > gridX - x || y > gridY - y)
                            {
                                blockPassingLimits = true;
                            }
                            /*
                            while (CheckIfSizeOccupiesAdditionalBlocks(w, h))
                            {
                                w = Random.Range(1, 3);
                                h = Random.Range(1, 3);
                                if (y == gridY - 1)
                                {
                                    h = 1;
                                }
                                if (x == gridX - 1)
                                {
                                    w = 1;
                                }
                            }*/
                            if (!blockPassingLimits)
                            {
                                GameObject newControl = Instantiate<GameObject>(controlPrefab, container);
                                newControl.GetComponent<UIPlayerGameControlElement>().digitalControl = playerSceneController.thisDevice.digitalControls[currentControl];
                                newControl.GetComponent<UIPlayerGameControlElement>().w = w;
                                newControl.GetComponent<UIPlayerGameControlElement>().h = h;
                                newControl.GetComponent<UIPlayerGameControlElement>().Setup();
                                newControl.GetComponent<UIPlayerGameControlElement>().uIPlayerGameControls = this;
                                //newControl.GetComponent<RectTransform>().localPosition = new Vector3((x * tileW) + tileW / 2, (-y * tileH) - tileH / 2, 0);
                                newControl.GetComponent<RectTransform>().localPosition = new Vector3((x * tileW), (-y * tileH), 0);
                                newControl.GetComponent<RectTransform>().sizeDelta = new Vector2(tileW * w, tileH * h);
                                currentControl += 1;

                                //if (x < gridX-1 && y<gridY-1)
                                //{

                                for (int wi = 0; wi < w; wi++)
                                {
                                    for (int hi = 0; hi < h; hi++)
                                    {

                                        if (gridOccupied[x + wi, y + hi] == false)
                                        {
                                            //Debug.Log((x + wi) + "," + (y + hi) + " is now occupied");
                                            gridOccupied[x + wi, y + hi] = true;
                                        }
                                        else
                                        {
                                            sizeBlocksOccupied = true;
                                            //Debug.Log((x + wi) + "," + (y + hi) + " WAS OCCUPIED");

                                        }


                                    }
                                }
                            }
                            //}
                        }
                        else
                        {

                        }

                    }
                }

            }
        }
        //Debug.Log("Occupied:" + CheckIfAllGridsOccupied());

    }

    bool CheckIfSizeOccupiesAdditionalBlocks(int w, int h)
    {
        int totalBlocksNeeded = w * h;

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                for (int wi = 0; wi < w; wi++)
                {
                    for (int hi = 0; hi < h; hi++)
                    {
                        if (gridOccupied[x + wi, y + hi] == true)
                        {
                            return true;
                        }

                    }
                }

            }
        }
        return false;
    }

    bool CheckIfAllGridsOccupied()
    {
        int quantity = 0;

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                if (gridOccupied[x, y] == true)
                {
                    quantity += 1;

                }

            }
        }
        Debug.Log("QuantityX:" + quantity + ",gridX:" + gridX + ",gridY:" + gridY);
        if (quantity >= gridX * gridY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void ClearContainer()
    {
        foreach (Transform child in container)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
