using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDevice
{
    public int id;
    public bool ready;
    public List<DigitalControl> digitalControls;
    public List<PhysicalControl> physicalControls;

    public int shipHealth;

    public Instruction currentInstructions;
    public int score;

    public PlayerDevice()
    {

    }

    public int GetRandomDigitalControl()
    {
        return Random.Range(0, digitalControls.Count - 1);
    }
    public int GetRandomPhysicalControl()
    {
        int randomNumber = Random.Range(0, physicalControls.Count);
        while (this.physicalControls[randomNumber].type == PhysicalControl.PhysicalControlType.joystick)
        {
            randomNumber = Random.Range(0, physicalControls.Count);

        }
        return randomNumber;
    }
}
