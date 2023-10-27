using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DigitalControl
{
    public int id;
    public string name;
    public ControlType type;
    public float[] configuration;
    public float value;
    public float lastValue;

    public float maxValue;

    public enum ControlType
    {
        fader, knob, switchtype, button
    }


    public DigitalControl(int _id, string name, ControlType type)
    {
        this.id = _id;
        this.name = name;
        this.type = type;
        float[] config = Database.GetConfiguration(type);
        int randomConfigMax = Random.Range(1, config.Length);
        float[] newConfig = new float[randomConfigMax + 1];
        Debug.Log("Setup config for " + name);
        Debug.Log("New config:" + randomConfigMax);

        for (int i = 0; i < newConfig.Length; i++)
        {
            newConfig[i] = config[i];
            Debug.Log(i + ":" + newConfig[i]);
        }
        this.configuration = newConfig;
        Debug.Log("this config:" + configuration.Length);

        this.maxValue = Mathf.Max(configuration);
    }

    public DigitalControl(ControlType _type, float[] _configuration)
    {
        type = _type;
        configuration = _configuration;

    }

    public DigitalControl(){
        
    }
    public string GetJSON()
    {
        return JsonUtility.ToJson(this);
    }


}

