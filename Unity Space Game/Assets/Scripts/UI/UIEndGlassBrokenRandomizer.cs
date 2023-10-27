using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEndGlassBrokenRandomizer : MonoBehaviour
{

    public GlassSetup[] glassSetups;
    RectTransform t;
    void OnEnable()
    {
        t = GetComponent<RectTransform>();
        int which=Random.Range(0,glassSetups.Length);
        t.localPosition=glassSetups[which].pos;
        t.eulerAngles=new Vector3(0,0,glassSetups[which].rotationZ);
        t.localScale=glassSetups[which].scale;

    }


    [System.Serializable]
    public class GlassSetup
    {
        public Vector3 pos;
        public float rotationZ;
        public Vector3 scale = new Vector3(1, 1, 1);
    }
}
