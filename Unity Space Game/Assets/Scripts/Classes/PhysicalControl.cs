using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhysicalControl
{
    public int id;
    public string name;
    public PhysicalControlType type;
    public float[] configuration;
    public float[] value;
    public float[] lastValue;
    public int[] pin;
    public int[] led;
    public string lcdPin;

    public float knobMax;
    public float knobMin;

    public enum PhysicalControlType
    {
        rockerSwitch, latchingButton, knob, joystick, lcd
    }

    public PhysicalControl(int id, PhysicalControlType type, int[] _pin, int[] _led, string _lcd, float[] configuration)
    {
        this.id = id;
        this.type = type;
        this.pin = _pin;
        this.led = _led;
        this.lcdPin = _lcd;
        this.configuration = configuration;
        value = new float[pin.Length];
        lastValue = new float[pin.Length];

        //In the case of rocker and latching the values are inverted, the default is 1;
        if (type == PhysicalControlType.rockerSwitch || type == PhysicalControlType.latchingButton)
        {
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = configuration[configuration.Length - 1];
                lastValue[i] = configuration[configuration.Length - 1];
                
            }
        }
    }

    public PhysicalControl()
    {

    }

    public string GetJSON()
    {
        return JsonUtility.ToJson(this);
    }

}

