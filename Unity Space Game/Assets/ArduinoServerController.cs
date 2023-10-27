using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class ArduinoServerController : MonoBehaviour
{
    bool winActive;
    bool loseActive;

    bool[] damageLightsActive = { false, false, false };
    bool blackHoleWarningActive = false;
    bool blackHoleInstanceActive = false;
    bool initializeLightsActive = false;
    //bool gameWinLightsActive = false;
    //bool gameLoseLightsActive = false;

    //int gameWinLightsPin = 13;
    //int gameLoseLightsPin = 4;

    void Start()
    {
        UduinoManager.Instance.pinMode(Settings.arduinoPinWin, PinMode.Output);
        UduinoManager.Instance.pinMode(Settings.arduinoPinLoss, PinMode.Output);

        UduinoManager.Instance.pinMode(Settings.arduinoDamageLightsPinOne, PinMode.Output);
        UduinoManager.Instance.pinMode(Settings.arduinoDamageLightsPinTwo, PinMode.Output);
        UduinoManager.Instance.pinMode(Settings.arduinoDamageLightsPinThree, PinMode.Output);

        //Set Black Hole and Initialization Pins
        UduinoManager.Instance.pinMode(Settings.arduinoBlackHoleWarningLightsPin, PinMode.Output);
        UduinoManager.Instance.pinMode(Settings.arduinoBlackHoleInstanceLightsPin, PinMode.Output);
        UduinoManager.Instance.pinMode(Settings.arduinoInitializeLightsPin, PinMode.Output);
        //UduinoManager.Instance.pinMode(gameWinLightsPin, PinMode.Output);
        //UduinoManager.Instance.pinMode(gameLoseLightsPin, PinMode.Output);
    }

    public void Win()
    {
        winActive = true;
        StartCoroutine(StopWin(Settings.arduinoTimerEnd));
    }

    public void Lose()
    {
        loseActive = true;
        StartCoroutine(StopLose(Settings.arduinoTimerEnd));
    }

    //Handles HIGH triggering, followed with a coroutine for timer
    public void DamageLights(int damageStep)
    {
        damageLightsActive[damageStep - 1] = true;

        if (damageStep == 1)
        {
            StartCoroutine(StopDamageLights(Settings.arduinoDamageLightsPinOne, Settings.arduinoFlexMaxPinTime));
        }

        if (damageStep == 2)
        {
            StartCoroutine(StopDamageLights(Settings.arduinoDamageLightsPinTwo, Settings.arduinoFlexMaxPinTime));
        }

        if (damageStep == 3)
        {
            StartCoroutine(StopDamageLights(Settings.arduinoDamageLightsPinThree, Settings.arduinoFlexMaxPinTime));
        }
    }

    public void BlackHoleWarningLights()
    {
        blackHoleWarningActive = true;
        StartCoroutine(StopBlackHoleWarningLights(Settings.arduinoFlexMaxPinTime));
    }

    public void BlackHoleInstanceLights()
    {
        blackHoleInstanceActive = true;
        StartCoroutine(StopBlackHoleInstanceLights(Settings.arduinoFlexMaxPinTime));
    }

    public void InitializeLights()
    {
        initializeLightsActive = true;
        StartCoroutine(StopInitializeLights(Settings.arduinoFlexMaxPinTime));
    }

    /*public void GameWinLights()
    {
        gameWinLightsActive = true;
        StartCoroutine(StopGameWinLights(flexMaxPinTime));

    }*/

    /*public void GameLoseLights()
    {
        gameLoseLightsActive = true;
        StartCoroutine(StopGameLoseLights(flexMaxPinTime));
    }*/

    IEnumerator StopWin(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(Settings.arduinoPinWin, State.LOW);

        //winActive = false;
    }

    IEnumerator StopLose(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(Settings.arduinoPinLoss, State.LOW);

        //loseActive = false;
    }

    //Delays at the beginnning, followed by turning the pin to LOW 
    IEnumerator StopDamageLights(int pin, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(pin, State.LOW);

    }

    IEnumerator StopBlackHoleWarningLights(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleWarningLightsPin, State.LOW);

        //blackHoleWarningActive = false;
    }

    IEnumerator StopBlackHoleInstanceLights(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleInstanceLightsPin, State.LOW);

        //blackHoleInstanceActive = false;
    }

    IEnumerator StopInitializeLights(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(Settings.arduinoInitializeLightsPin, State.LOW);

        //initializeLightsActive = false;
    }

    /*IEnumerator StopGameWinLights(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(gameWinLightsPin, State.LOW);

        gameWinLightsActive = false;
    }*/

    /*IEnumerator StopGameLoseLights(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        UduinoManager.Instance.digitalWrite(gameLoseLightsPin, State.LOW);

        gameLoseLightsActive = false;
    }*/

    void Update()
    {
        if (winActive)
        {
            winActive = false;

            turnOffDamageLightFeeds();
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleWarningLightsPin, State.LOW);
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleInstanceLightsPin, State.LOW);

            Debug.Log("******LIGHT: Win Triggered");
            UduinoManager.Instance.digitalWrite(Settings.arduinoPinWin, State.HIGH);
        }
        if (loseActive)
        {
            loseActive = false;

            turnOffDamageLightFeeds();
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleWarningLightsPin, State.LOW);
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleInstanceLightsPin, State.LOW);

            Debug.Log("******LIGHT: Lose Triggered");
            UduinoManager.Instance.digitalWrite(Settings.arduinoPinLoss, State.HIGH);
        }

        for (int i = 0; i < damageLightsActive.Length; i++)
        {
            if (damageLightsActive[i])
            {
                damageLightsActive[i] = false;
                turnOffDamageLightFeeds();
                Debug.Log("******LIGHT: Damage Lights Triggered Phase:" + (i + 1));

                if (i == 0)
                {
                    UduinoManager.Instance.digitalWrite(Settings.arduinoDamageLightsPinOne, State.HIGH);
                }
                if (i == 1)
                {
                    UduinoManager.Instance.digitalWrite(Settings.arduinoDamageLightsPinTwo, State.HIGH);
                }
                if (i == 2)
                {
                    UduinoManager.Instance.digitalWrite(Settings.arduinoDamageLightsPinThree, State.HIGH);
                }
            }

        }

        if (blackHoleWarningActive)
        {
            blackHoleWarningActive = false;

            turnOffDamageLightFeeds();

            Debug.Log("******LIGHT: Black Hole Warning Lights Triggered");
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleWarningLightsPin, State.HIGH);
        }

        if (blackHoleInstanceActive)
        {
            blackHoleInstanceActive = false;

            turnOffDamageLightFeeds();
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleWarningLightsPin, State.LOW);

            Debug.Log("******LIGHT: Black Hole Instance Lights Triggered");
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleInstanceLightsPin, State.HIGH);
        }

        if (initializeLightsActive)
        {
            initializeLightsActive = false;

            turnOffDamageLightFeeds();
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleWarningLightsPin, State.LOW);
            UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleInstanceLightsPin, State.LOW);
            UduinoManager.Instance.digitalWrite(Settings.arduinoPinWin, State.LOW);
            UduinoManager.Instance.digitalWrite(Settings.arduinoPinWin, State.LOW);

            //UduinoManager.Instance.digitalWrite(gameWinLightsPin, State.LOW);
            //UduinoManager.Instance.digitalWrite(gameLoseLightsPin, State.LOW);

            Debug.Log("******LIGHT: Initialize Lights Triggered");
            UduinoManager.Instance.digitalWrite(Settings.arduinoInitializeLightsPin, State.HIGH);
        }

        /*if (gameWinLightsActive)
        {
            turnOffDamageLightFeeds();
            UduinoManager.Instance.digitalWrite(blackHoleWarningLightsPin, State.LOW);
            UduinoManager.Instance.digitalWrite(blackHoleInstanceLightsPin, State.LOW);
            UduinoManager.Instance.digitalWrite(gameWinLightsPin, State.HIGH);
        }*/

        /*if (gameLoseLightsActive)
        {
            //turnOffDamageLightFeeds();
            //UduinoManager.Instance.digitalWrite(blackHoleWarningLightsPin, State.LOW);
            //UduinoManager.Instance.digitalWrite(blackHoleInstanceLightsPin, State.LOW);
            UduinoManager.Instance.digitalWrite(gameLoseLightsPin, State.HIGH);
        }*/
    }

    public void turnOffDamageLightFeeds()
    {
        Debug.Log("******LIGHT: Turn off Damage Lights Triggered");

        UduinoManager.Instance.digitalWrite(Settings.arduinoDamageLightsPinOne, State.LOW);
        UduinoManager.Instance.digitalWrite(Settings.arduinoDamageLightsPinTwo, State.LOW);
        UduinoManager.Instance.digitalWrite(Settings.arduinoDamageLightsPinThree, State.LOW);
    }

    public void turnOffBothBlackHoleLights()
    {
        UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleWarningLightsPin, State.LOW);
        UduinoManager.Instance.digitalWrite(Settings.arduinoBlackHoleInstanceLightsPin, State.LOW);
    }
}
