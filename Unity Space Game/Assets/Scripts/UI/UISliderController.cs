using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderController : MonoBehaviour
{
    public UIPlayerGameControlElement uIPlayerGameControlElement;
    public Slider slider;
    public RectTransform sliderFill;

    public Transform textContainer;
    public GameObject textPrefab;
    GameObject[] textGO;
    float[] config;
    public bool isVertical;

    void Start()
    {
        //Setup(new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
    }
    public void Setup(float[] _config)
    {
        config = _config;
        slider.maxValue = Mathf.Max(config);
        Debug.Log("config length:" + config.Length);
        Debug.Log("config first:" + config[0]);
        Debug.Log("config last:" + config[config.Length - 1]);
        Debug.Log("slider max=" + slider.maxValue);

        textGO = new GameObject[config.Length];
        for (int i = 0; i < config.Length; i++)
        {
            textGO[i] = Instantiate<GameObject>(textPrefab, textContainer);
            textGO[i].GetComponent<Text>().text = (i + 1).ToString();
        }
        UpdateUI();
    }
    public void SliderChange()
    {
        UpdateUI();
        uIPlayerGameControlElement.ChangeSlider(slider.value);
    }

    void UpdateUI()
    {
        Debug.Log(" sliderValue=" + slider.value);

        int sliderValueInt = Mathf.RoundToInt(slider.value);
        Debug.Log(" sliderValueInt=" + sliderValueInt);

        for (int i = 0; i < config.Length; i++)
        {
            if (config[i] != null && textGO[i] != null)
            {
                if (config[i] == sliderValueInt)
                {
                    textGO[i].GetComponent<Text>().color = Tools.HexColor("#68FFB4");
                }
                else
                {
                    textGO[i].GetComponent<Text>().color = Tools.ColorAlpha(Tools.HexColor("#543C18"), 0.5f);

                }
            }
        }
        if (isVertical)
            sliderFill.localScale = new Vector3(1, Tools.map(slider.value, slider.minValue, slider.maxValue, 0, 1), 1);
        else
            sliderFill.localScale = new Vector3(Tools.map(slider.value, slider.minValue, slider.maxValue, 0, 1), 1, 1);

    }
}
