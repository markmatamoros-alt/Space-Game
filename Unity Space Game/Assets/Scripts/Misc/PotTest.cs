using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;
public class PotTest : MonoBehaviour
{
    public Color[] currentLEDColor;

    // Start is called before the first frame update
    void Start()
    {
        UduinoManager.Instance.pinMode(AnalogPin.A0, PinMode.Input); //Pin mode in analog to enable reading
        currentLEDColor = new Color[50];


    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKey(KeyCode.A))
        {
            for (int i = 0; i < 50; i++)
            {
                SetLED(i, Color.cyan);
            }

        }
        if (Input.GetKey(KeyCode.B))
        {
            for (int i = 0; i < 50; i++)
            {
                SetLED(i, Color.magenta);
            }

        }
        */
        float readValue = UduinoManager.Instance.analogRead(AnalogPin.A0);
        float whichLedValue = Mathf.Clamp(Tools.map(readValue, 50, 600, 0, 4),0,4);
        int whichLed = Mathf.RoundToInt(whichLedValue);
        Debug.Log("whichledvalue"+whichLedValue);
        Debug.Log("whichled"+whichLed);
        for (int j = 0; j < 5; j++)
        {
            if (j == whichLed)
            {
                SetLED(j, Color.green);

            }
            else
            {
                SetLED(j, Color.black);

            }
        }

        Debug.Log(readValue);
        
    }

    public void SetLED(int which, Color _c)
    {
        //UduinoManager.Instance.sendCommand("setLED", which, (int)Mathf.FloorToInt(_c.r), (int)Mathf.FloorToInt(_c.g), (int)Mathf.FloorToInt(_c.b));

        if (currentLEDColor[which] != _c)
        {
            Debug.Log("Setting LED "+which+" to "+_c.ToString());
            UduinoManager.Instance.sendCommand("setLED", which, (int)Mathf.FloorToInt(_c.r), (int)Mathf.FloorToInt(_c.g), (int)Mathf.FloorToInt(_c.b));
            currentLEDColor[which] = _c;
        }
    }
}
