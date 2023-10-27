using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Instruction
{
    public string instructionName;
    public float value;
    public float currentTime;
    public bool isPhysical;
    public int pin;

    public int pinOrder;
    public bool active;

    public DigitalControl.ControlType type;
    public PhysicalControl.PhysicalControlType physicalType;

    public Instruction()
    {

    }

    public Instruction(string instructionName, float value, float currentTime, DigitalControl.ControlType _type)
    {
        this.instructionName = instructionName;
        this.value = value;
        this.currentTime = currentTime;
        this.type = _type;
        isPhysical = false;
        active=true;

    }

    public Instruction(string instructionName, int _pin, int _pinOrder, float value, float currentTime, PhysicalControl.PhysicalControlType _physicalType)
    {
        this.instructionName = instructionName;
        this.value = value;
        this.currentTime = currentTime;
        this.physicalType = _physicalType;
        this.pin=_pin;
        this.pinOrder=_pinOrder;
        isPhysical = true;
        active=true;
    }

    public string GetJSON()
    {
        return JsonUtility.ToJson(this);
    }
}
