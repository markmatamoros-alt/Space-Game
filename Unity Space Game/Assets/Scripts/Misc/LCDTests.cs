using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

using Uduino;
using UnityEngine.UI;

public class LCDTests : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField left, right;
    void Start()
    {

    }

    string[] randomStringText = { "Test test testingly testing", "Amazon is a company also", "Google is a company", "Space Pants that are really nice", "Super Lord Darth Vader", "Simple" };

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            SetLCDName("pL_LCD", randomStringText[Random.Range(0, randomStringText.Length)]);
            SetLCDName("pR_LCD", randomStringText[Random.Range(0, randomStringText.Length)]);
        }
    }
    public void SetLCDName(string _id, string _name)
    {
        string textToSend = _name;
        textToSend = textToSend.Replace(" ", "_");
        //if (textToSend.Length >= 32)
        //    textToSend = textToSend.Substring(0, 31);

        //byte[] bytes = Encoding.UTF7.GetBytes(textToSend);
        //UduinoManager.Instance.sendCommand(_id, bytes, 0);

        //UduinoManager.Instance.sendCommand(_id, bytes, bytes.Length);
                    Debug.Log(textToSend);

        if (textToSend.Length >= 22)
        {
            textToSend = textToSend.Substring(0, 22);

        }
                    Debug.Log(textToSend);

        UduinoManager.Instance.sendCommand(_id, textToSend);

        Debug.Log(textToSend.Length);
        /*
        if (textToSend.Length <= 17)
        {
            
            UduinoManager.Instance.sendCommand(_id, textToSend, -1);

        }
        else
        {        Debug.Log(textToSend);

            string firstHalf = textToSend.Substring(0, 15);
             Debug.Log(firstHalf);

            string secondHalf = textToSend.Substring(15, textToSend.Length-15);
                Debug.Log(secondHalf);

            UduinoManager.Instance.sendCommand(_id, firstHalf, -1);
            UduinoManager.Instance.sendCommand(_id, secondHalf, 0);
        }
*/
    }

    public void SendLeft()
    {
        SetLCDName("pL_LCD", left.text);

    }


    public void SendRight()
    {
        SetLCDName("pR_LCD", right.text);

    }
}
