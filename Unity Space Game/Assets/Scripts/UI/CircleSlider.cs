using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSlider : MonoBehaviour
{
    public UIPlayerGameControlElement uIPlayerGameControlElement;
    [SerializeField] Transform handle;
    [SerializeField] UIPlayerGameControlBackgroundGlow handleGlow;
    [SerializeField] InputHelper inputHelper;
    [SerializeField] Image fill;
    [SerializeField] Text valTxt;
    [SerializeField] float limitStart, limitEnd;
    [SerializeField] bool DebugWithMouse;
    bool isHolding;
    float holdingTimer;
    Vector3 mousePos;
    // Start is called before the first frame update
    float startPos;
    float currentPos;
    bool dragging;
    float angle;
    public float maxValue;

    int xInvert = 1;
    int yInvert = 1;
    float x, y, xP, yP;

    
    void Start()
    {
        //inputHelper = GameObject.Find("InputHelper").GetComponent<InputHelper>();
        touches = new List<Touch>();
    }
    public void Setup(float[] config)
    {
        maxValue = Mathf.Max(config);

    }
    List<Touch> touches;
    void Update()
    {
        CalculateTouchCoordinates();
        if (isHolding)
        {
            ControlHandle();
        }
        
    }

    void CalculateTouchCoordinates()
    {
        touches.Clear();
        //foreach (Touch t in InputHelper.GetTouches())
        foreach (Touch t in Input.touches)
        {

            
                touches.Add(t);
                //Debug.Log("touch id:" + t.fingerId + " x:" + t.position.x + " deltaX:" + t.deltaPosition.x);
            



        }
        if (touches.Count > 0)
        {
            x = touches[0].deltaPosition.x;
            y = touches[0].deltaPosition.y;
            Debug.Log("x read:"+x);
            xP = touches[0].position.x;
            yP = touches[0].position.y;
        }

            if(DebugWithMouse){
            x = Input.GetAxis("Mouse X")*10;
            y = Input.GetAxis("Mouse Y")*10;
            xP = Input.mousePosition.x;
            yP = Input.mousePosition.y;
            }
        
    }

    void ControlHandle()
    {
        //Get rotate direction
        if (xP > transform.position.x) // Is on the right half
        {
            Debug.Log("Right");
            yInvert = 1;
        }
        else // Is on the left half
        {
            Debug.Log("Left");
            yInvert = -1;

        }

        if (yP > transform.position.y) //Is on the top half
        {
            Debug.Log("Top");
            xInvert = 1;

        }
        else //Is on the bot half
        {
            Debug.Log("Bot");
            xInvert = -1;
        }


        //float angle = handle.rotation.normalized.z;
        //Debug.Log("Angle original " + angle);
        //angle += (0.1f * Input.GetAxis("Mouse X"));
        //Debug.Log("Angle add " + angle);

        //angle = (angle <= 0) ? (360 + angle) : angle;
        //Debug.Log("Angle changed " + angle);

        //Quaternion r = Quaternion.AngleAxis(angle + 135f, Vector3.forward);
        //handle.rotation = r;
        //handle.Rotate(new Vector3(0, 0, diff*Time.deltaTime*direction));
        angle = handle.localEulerAngles.z;
        angle = WrapAngle(angle);
        //Debug.Log(angle);
        angle = Tools.map(angle, limitStart, limitEnd, 0, maxValue);
        // Debug.Log(angle);
        //Debug.Log("Touch Count:" + touches.Count);

        if (angle < 0)
        {

            angle = 0;
            if (x * 0.1f * xInvert > 0 || y * 0.1f * yInvert < 0)
            {
                UpdatePosition();
            }

        }
        else if (angle > maxValue)
        {
            angle = maxValue;
            if (x * 0.1f * xInvert < 0 || y * 0.1f * yInvert > 0)
            {
                UpdatePosition();
            }

        }
        else
        {
            UpdatePosition();

        }




        //Vector2 dir = new Vector3(mousePos.x,0,0) - handle.position;
        //angle += Mathf.Atan2(0, -dir.x)*Mathf.Rad2Deg;
        //angle = (angle<=0)?(360+angle) : angle;
        //Quaternion r= Quaternion.AngleAxis(angle+135f, Vector3.forward);
        //handle.rotation=r;
        //angle=((angle>=315)?(angle-360):angle)+45;
        //float fillAmount= 0.75f - (angle/360f);
        //valTxt.text=Mathf.Round((fillAmount*100)/0.75f).ToString();
        //Debug.Log("drag");
    }

    void SenseDragTouch()
    {
        if(stopDragging!=null)
        StopCoroutine(stopDragging);
        isHolding = true;
    }

    IEnumerator StopDetectingTouch()
    {
        yield return new WaitForSeconds(0.3f);
        isHolding = false;
    }
    public void StartDrag()
    {
        Debug.Log("Knob Start Dragging");

        //startPos=Input.mousePosition.x;
        SenseDragTouch();
    }

    public void onHandleDrag()
    {
        SenseDragTouch();


    }
    Coroutine stopDragging;
    public void EndDrag()
    {
        Debug.Log("Knob Stop Dragging");
        stopDragging = StartCoroutine(StopDetectingTouch());
        //Debug.Log(Mathf.Round(angle));
        // uIPlayerGameControlElement.ChangeSlider(Mathf.Round(angle));
    }

    void UpdatePosition()
    {
                Debug.Log("x:" + x+", y:"+y+", xinvert:"+xInvert+", yinvert:"+yInvert);

        Debug.Log("UpdatePosition:" + touches.Count + ", Rotation:"+(-2 * (x * 0.01f) * xInvert) + (2 * (y * 0.01f) * yInvert));

        handleGlow.IsPressed();
        //Debug.Log("UpdatePosition x:" + touches[0].deltaPosition.x);
        handle.Rotate(new Vector3(0, 0, (-2 * (x * 0.01f) * xInvert) + (2 * (y * 0.01f) * yInvert)));

        //Debug.Log("Angle euler z " + angle);
        valTxt.text = Mathf.Round(angle).ToString();
        fill.fillAmount = Tools.map(angle, 0, maxValue, 0.1f, 0.9f);
        uIPlayerGameControlElement.ChangeSlider(Mathf.Round(angle));
    }



    private static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    private static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }
}
