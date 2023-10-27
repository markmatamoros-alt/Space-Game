using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Database
{
    public static string[] ControlNames;
    public static int count;
    public static List<DigitalControl> digitalControls;

    public static void SetControlName(string[] _names)
    {
        ControlNames = new string[_names.Length];
        for (int i = 0; i < _names.Length; i++)
        {
            //System.Array.Resize<string>(ref Database.ControlNames, Database.ControlNames.Length + 1);
            ControlNames[i] = _names[i];

        }
    }
    public static float[] GetConfiguration(DigitalControl.ControlType _type)
    {
        for (int i = 0; i < Database.digitalControls.Count; i++)
        {
            if (Database.digitalControls[i].type == _type)
            {
                return Database.digitalControls[i].configuration;
            }
        }
        return null;
    }

    public static float[] SetConfiguration(DigitalControl.ControlType _type)
    {
        float[] conf;
        for (int i = 0; i < Database.digitalControls.Count; i++)
        {
            if (Database.digitalControls[i].type == _type)
            {
                int randomMax = Mathf.RoundToInt(Database.digitalControls[i].configuration[Random.Range(2, Database.digitalControls[i].configuration.Length)]);
                conf = new float[randomMax];
                for (int j = 0; j < conf.Length; j++)
                {
                    conf[j] = Database.digitalControls[i].configuration[j];
                }


                return conf;

            }
        }
        return null;

    }
}
