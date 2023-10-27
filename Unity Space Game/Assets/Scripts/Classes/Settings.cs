using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public static string version;
    public static string ip;
    public static string[] deviceIP;
    public static int port;
    public static int portDevice;
    public static string APIaddress;
    public static int APIAppId;
    public static int APIExperienceId;

    public static int APICheckCountdown;
    public static float musicVolume;
    public static float soundVolume;

    public static float heartBeatTimer = 60;

    public static int roundToWin;
    public static float timePerInstruction;
    public static float timePerInstructionRoundDecrease;

    public static int shipHealthMax;

    public static int shipHealthDamageTrigger;
    public static int instructionsToWinRound;
    public static int amountOfPauseBetweenBlackhole;
    public static int chanceForBlackholePercent;

    public static int chanceForBlackholeIncreaseByRound;
    public static int blackholeInputNeeded;
    public static float timeToAvoidBlackHole;
    public static float timeBlackHoleEffect;
    public static int launchSequenceCountdown;

    public static bool LCDOnlyNames;
    public static bool showDebug;
    public static Color LedActiveColor;

    public static int[] digitalControlAmount;
    public static int arduinoPinWin;
    public static int arduinoPinLoss;
    public static int arduinoTimerEnd;

    public static int timeOutSeconds;
    public string[] actionNames = { "Shake", "Activate", "Refrigerate", "Pull", "Imagine", "Jiggle", "Twist", "Reattach", "Extinguish", "Take" };

    public static int arduinoDamageLightsPinOne;
    public static int arduinoDamageLightsPinTwo;
    public static int arduinoDamageLightsPinThree;
    public static int arduinoBlackHoleWarningLightsPin;
    public static int arduinoBlackHoleInstanceLightsPin;
    public static int arduinoInitializeLightsPin;
    public static int arduinoFlexMaxPinTime;
}
