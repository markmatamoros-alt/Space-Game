using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugTouchCreator : MonoBehaviour
{
    List<Touch> touches;

    public GameObject touchPrefab;
    // Start is called before the first frame update
    void Start()
    {
        //inputHelper = GameObject.Find("InputHelper").GetComponent<InputHelper>();
        touches = new List<Touch>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        touches.Clear();
        //foreach (Touch t in InputHelper.GetTouches())
        //for (int i = 0; i < InputHelper.GetTouches().Count; i++)
        for (int i = 0; i < Input.touches.Length; i++)

        {
            {
                //touches.Add(Input.touches[i]);
                //Touch t = InputHelper.GetTouches()[i];
                Touch t = Input.touches[i];
                touches.Add(t);
                GameObject newObj = Instantiate<GameObject>(touchPrefab, this.transform);
                newObj.GetComponent<UIDebugTouchController>().UpdateP(t.position.x, t.position.y);
                //Debug.Log("touch id:" + t.fingerId + " x:" + t.position.x + " deltaX:" + t.deltaPosition.x);
            }
        }

    }
}
