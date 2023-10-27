using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class UDPMessage
{
    public string sender;
    public int id;
    public string msg;

    public int controlID;
    public string val;
    public int pin;

    public DigitalControl[] digitalControl;
    public PhysicalControl[] physicalControl;
    public Instruction instruction;
    public UDPMessage()
    {

    }
}
