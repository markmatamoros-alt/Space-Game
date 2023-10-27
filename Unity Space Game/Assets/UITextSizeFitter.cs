using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextSizeFitter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<RectTransform>().sizeDelta=new Vector2(transform.parent.GetComponent<RectTransform>().sizeDelta.y,transform.parent.GetComponent<RectTransform>().sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
