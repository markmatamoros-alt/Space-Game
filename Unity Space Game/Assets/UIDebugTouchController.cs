using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugTouchController : MonoBehaviour
{
    public Text tx;
    public void UpdateP(float x, float y)
    {
        this.transform.position = new Vector3(x, y, 0);
        tx.text = "x:" + x + ", y:" + y;
    }
}
