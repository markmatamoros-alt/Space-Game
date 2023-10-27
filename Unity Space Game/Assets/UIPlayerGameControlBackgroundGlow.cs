using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerGameControlBackgroundGlow : MonoBehaviour
{
    private Image bg;
    private Color originC;
    private Color targetC;
    [SerializeField]
    private Color activeC;
    [SerializeField]
    private float smoothTime = 0.2f;
    [SerializeField]

    private float timeToFadeOut = 0.2f;
    Coroutine fadeOutCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        bg = this.GetComponent<Image>();
        originC = bg.color;
        targetC = originC;
    }

    // Update is called once per frame
    void OnGUI()
    {
        if (bg.color != targetC)
        {

            bg.color = Color.Lerp(bg.color, targetC, Time.deltaTime / smoothTime);
        }


    }

    public void IsPressed()
    {

            targetC = activeC;
            StopAllCoroutines();
            fadeOutCoroutine=StartCoroutine(FadeOut(timeToFadeOut));

    }

    IEnumerator FadeOut(float delay){
        yield return new WaitForSeconds(delay);
        targetC = originC;
    }


}
