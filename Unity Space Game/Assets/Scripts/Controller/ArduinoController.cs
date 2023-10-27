using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;
using UnityEngine.UI;

public class ArduinoController : MonoBehaviour
{
    public PlayerSceneController player;
    public bool stopReading;
    public bool readBlackHoleJoysticks;
    public Text debugText;
    public Color[] currentLEDColor;

    void Start()
    {
        debugText.gameObject.SetActive(Settings.showDebug);
        currentLEDColor = new Color[50];

    }
    public void Setup()
    {
        for (int i = 0; i < player.thisDevice.physicalControls.Count; i++)
        {
            if (player.thisDevice.physicalControls[i].type == PhysicalControl.PhysicalControlType.knob)
            {
                UduinoManager.Instance.pinMode(AnalogPin.A0, PinMode.Input);
            }
            else if (player.thisDevice.physicalControls[i].type == PhysicalControl.PhysicalControlType.lcd)
            {

            }
            else
            {
                for (int j = 0; j < player.thisDevice.physicalControls[i].pin.Length; j++)
                {

                    UduinoManager.Instance.pinMode(player.thisDevice.physicalControls[i].pin[j], PinMode.Input_pullup);
                }
            }
        }

    }


    void Update()
    {
                                
        if (!stopReading && UduinoManager.Instance.isConnected())
        {
            for (int i = 0; i < player.thisDevice.physicalControls.Count; i++)
            {
                if (player.thisDevice.physicalControls[i].type == PhysicalControl.PhysicalControlType.knob)
                {
                    player.thisDevice.physicalControls[i].value[0] = UduinoManager.Instance.analogRead(AnalogPin.A0);

                    player.thisDevice.physicalControls[i].value[0] = UduinoManager.Instance.analogRead(AnalogPin.A0);
                    float mappedValue = Tools.map(player.thisDevice.physicalControls[i].value[0], player.thisDevice.physicalControls[i].knobMin, player.thisDevice.physicalControls[i].knobMax, Mathf.Min(player.thisDevice.physicalControls[i].configuration), Mathf.Max(player.thisDevice.physicalControls[i].configuration));
                    float clampedValue = Mathf.Clamp(mappedValue, Mathf.Min(player.thisDevice.physicalControls[i].configuration), Mathf.Max(player.thisDevice.physicalControls[i].configuration));
                    if (Settings.showDebug)
                        debugText.text = "OriginalValue:" + player.thisDevice.physicalControls[i].value[0] + " mapped:" + clampedValue + " rounded:" + Mathf.RoundToInt(clampedValue);
                    float whichLedValue = Tools.map(clampedValue, Mathf.Min(player.thisDevice.physicalControls[i].configuration), Mathf.Max(player.thisDevice.physicalControls[i].configuration), 0, player.thisDevice.physicalControls[i].led.Length - 1);
                    int whichLed = Mathf.RoundToInt(whichLedValue);
                    //Debug.Log("whichledvalue" + whichLedValue);
                    //Debug.Log("whichled" + whichLed);
                    /*
                    for (int j = 0; j < player.thisDevice.physicalControls[i].led.Length; j++)
                    {
                        if (j == whichLed)
                        {
                            SetLED(player.thisDevice.physicalControls[i].led[j], Settings.LedActiveColor);

                        }
                        else
                        {
                            SetLED(player.thisDevice.physicalControls[i].led[j], Color.black);

                        }

                    }*/
                    player.ChangePhysicalControlValue(i, 0, Mathf.RoundToInt(clampedValue));

                }
                else if (player.thisDevice.physicalControls[i].type == PhysicalControl.PhysicalControlType.lcd)
                {
                }
                else if (player.thisDevice.physicalControls[i].type == PhysicalControl.PhysicalControlType.joystick)
                {
                    if (readBlackHoleJoysticks)
                    {
                        for (int j = 0; j < player.thisDevice.physicalControls[i].pin.Length; j++)
                        {

                            player.ChangePhysicalControlValue(i, j, player.thisDevice.physicalControls[i].value[j]);

                            player.thisDevice.physicalControls[i].value[j] = UduinoManager.Instance.digitalRead(player.thisDevice.physicalControls[i].pin[j]);

                        }
                    }
                }
                else
                {

                    for (int j = 0; j < player.thisDevice.physicalControls[i].pin.Length; j++)
                    {
                        player.ChangePhysicalControlValue(i, j, player.thisDevice.physicalControls[i].value[j]);
                        /*
                        if (player.thisDevice.physicalControls[i].type == PhysicalControl.PhysicalControlType.latchingButton)
                        {
                            if (player.thisDevice.physicalControls[i].value[j] <= 0)
                            {
                                SetLED(player.thisDevice.physicalControls[i].led[j], Color.black);

                            }
                            else
                            {
                                SetLED(player.thisDevice.physicalControls[i].led[j], Settings.LedActiveColor);

                            }
                        }*/
                        player.thisDevice.physicalControls[i].value[j] = UduinoManager.Instance.digitalRead(player.thisDevice.physicalControls[i].pin[j]);

                    }


                }
            }
        }

    }

    public void SetLCDName(string _id, string _name)
    {
        string textToSend = _name;
        textToSend = textToSend.Replace(" ", "_");
        if (textToSend.Length >= 22)
        {
            textToSend = textToSend.Substring(0, 22);

        }
        Debug.Log("Arduino " + _id + " to " + _name);
        UduinoManager.Instance.sendCommand(_id, textToSend);

    }
    public void ClearLCDs()
    {

        UduinoManager.Instance.sendCommand("c_LCDs");

    }
    public void SetLED(int which, Color _c)
    {
        /*
    if (currentLEDColor[which] != _c)
    {
        stopReading = true;
        Debug.Log("Setting LED " + which + " to " + _c.ToString());

        UduinoManager.Instance.sendCommand("setLED", which, (int)Mathf.FloorToInt(_c.r)*255, (int)Mathf.FloorToInt(_c.g)*255, (int)Mathf.FloorToInt(_c.b)*255);
        currentLEDColor[which] = _c;
        stopReading = false;

    }*/
    }
}
