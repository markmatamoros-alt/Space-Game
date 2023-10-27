using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISmoothTransition : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(1, 1, 1);
    public bool changeScale;
    public Vector3 targetLocalPosition;
    public bool changePos;

    public float targetAlpha = 1;
    public bool changeAlpha;
    public float smoothTime = 0.2f;
    Vector3 refScale, refLPos;
    float refAlpha;
    void FixedUpdate()
    {
        if (changePos)
        {
            if (this.transform.localPosition != targetLocalPosition)
            {
                this.transform.localPosition = Vector3.SmoothDamp(this.transform.localPosition, targetLocalPosition, ref refLPos, smoothTime);
            }
        }
        if (changeScale)
        {
            if (this.transform.localScale != targetScale)
            {
                this.transform.localScale = Vector3.SmoothDamp(this.transform.localScale, targetScale, ref refScale, smoothTime);
            }
        }
        if (changeAlpha)
        {
            if (GetComponent<Image>().color.a != targetAlpha)
            {
                GetComponent<Image>().color = Tools.ColorAlpha(GetComponent<Image>().color, Mathf.SmoothDamp(GetComponent<Image>().color.a, targetAlpha, ref refAlpha, smoothTime));
            }
        }

    }
}
