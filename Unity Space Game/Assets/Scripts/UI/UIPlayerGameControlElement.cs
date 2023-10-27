using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerGameControlElement : MonoBehaviour
{
    public UIPlayerGameControls uIPlayerGameControls;
    public UIPlayerGameControlBackgroundGlow uIPlayerGameControlBackgroundGlow;
    public DigitalControl digitalControl;
    public Text controlName;

    public Toggle toggle;
    public GameObject typeToggle, typeButton, typeSliderH, typeSliderV, typeKnob;
    public Text btText, btTextShadow, tgText, tgTextShadow;
    public int w, h;
    public void Setup()
    {

        //SetName
        //ReadFromXML.LoadControlNames();
        //int whichName = Mathf.RoundToInt(Random.Range(0, Database.ControlNames.Length - 1));
        //Debug.Log("databasecontrolnames:"+Database.ControlNames.Length);
        //string _name = Database.ControlNames[whichName];

        controlName.text = digitalControl.name;
        if (digitalControl.type == DigitalControl.ControlType.fader)
        {
            if (w > h)
            {
                typeSliderH.SetActive(true);
                typeSliderH.GetComponent<UISliderController>().Setup(digitalControl.configuration);
            }
            else
            {
                typeSliderV.SetActive(true);
                typeSliderV.GetComponent<UISliderController>().Setup(digitalControl.configuration);
            }

        }
        if (digitalControl.type == DigitalControl.ControlType.button)
        {
            if (w >= 2 && h >= 2)
            {

            }
            else
            {
                if (w == 1 && h == 1)
                {
                    tgText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);

                }
                else
                {
                    tgText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 72);

                }
            }
            controlName.text = "";
            typeButton.SetActive(true);
            btText.text = digitalControl.name;
            btTextShadow.text = digitalControl.name;

        }
        if (digitalControl.type == DigitalControl.ControlType.knob)
        {
            typeKnob.SetActive(true);
            typeKnob.GetComponent<CircleSlider>().Setup(digitalControl.configuration);

        }
        if (digitalControl.type == DigitalControl.ControlType.switchtype)
        {
            if (w >= 2 && h >= 2)
            {

            }
            else
            {
                tgText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
                tgText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 160);
            }
            typeToggle.SetActive(true);
            controlName.text = "";
            tgText.text = digitalControl.name;
            tgTextShadow.text = digitalControl.name;
        }
    }
    void Start()
    {

    }



    void ChangeControlValue(float _val)
    {
        uIPlayerGameControlBackgroundGlow.IsPressed();
        _val = Mathf.Round(_val);
        digitalControl.value = _val;
        uIPlayerGameControls.playerSceneController.ChangeControlValue(digitalControl.id, _val);
    }


    public void ChangeToggle()
    {

        float stateFloat = toggle.isOn ? 1 : 0;

        toggle.GetComponent<Animator>().SetBool("on", toggle.isOn);
        ChangeControlValue(stateFloat);
        Debug.Log(digitalControl.name + ":" + stateFloat);
    }

    public void PressButton()
    {

        ChangeControlValue(1);
        ChangeControlValue(0);

    }

    public void ChangeSlider(float _val)
    {
        uIPlayerGameControlBackgroundGlow.IsPressed();

        ChangeControlValue(_val);

    }

}
