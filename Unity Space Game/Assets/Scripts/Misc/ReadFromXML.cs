using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine;

public class ReadFromXML
{
    public static int LoadDeviceID()

    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Device_ID.xml");
        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(filePath));
            Settings.showDebug = bool.Parse(xmlDoc.SelectNodes("deviceid/showdebug").Item(0).InnerText);
            return int.Parse(xmlDoc.SelectNodes("deviceid/id").Item(0).InnerText);
        }
        else
        {
            return -1;
        }
    }

    public static void LoadDeviceIP()

    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Device_ID.xml");
        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(filePath));

            Settings.ip = xmlDoc.SelectNodes("deviceid/ip").Item(0).InnerText;
            Settings.deviceIP = new string[6];

            Settings.deviceIP[0] = xmlDoc.SelectNodes("deviceid/serverip").Item(0).InnerText;
            Settings.port = int.Parse(xmlDoc.SelectNodes("deviceid/port").Item(0).InnerText);
            Settings.LCDOnlyNames = bool.Parse(xmlDoc.SelectNodes("deviceid/lcdonlynames").Item(0).InnerText);
            string ledColorText = xmlDoc.SelectNodes("deviceid/ledcoloractive").Item(0).InnerText;
            string[] ledColorSplit = ledColorText.Split(',');
            Settings.LedActiveColor = new Color(float.Parse(ledColorSplit[0]) / 255, float.Parse(ledColorSplit[1]) / 255, float.Parse(ledColorSplit[2]) / 255, 1);
            Debug.Log("Led Active Color is " + Settings.LedActiveColor.ToString());
        }
    }
    public static void LoadSettings()

    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Settings.xml");
        if (File.Exists(filePath))
        {


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(filePath));

            Settings.version = xmlDoc.SelectNodes("settings/version").Item(0).InnerText;
            Settings.ip = xmlDoc.SelectNodes("settings/ip").Item(0).InnerText;
            Settings.deviceIP = new string[6];
            Settings.deviceIP[0] = xmlDoc.SelectNodes("settings/device0ip").Item(0).InnerText;
            Settings.deviceIP[1] = xmlDoc.SelectNodes("settings/device1ip").Item(0).InnerText;
            Settings.deviceIP[2] = xmlDoc.SelectNodes("settings/device2ip").Item(0).InnerText;
            Settings.deviceIP[3] = xmlDoc.SelectNodes("settings/device3ip").Item(0).InnerText;
            Settings.deviceIP[4] = xmlDoc.SelectNodes("settings/device4ip").Item(0).InnerText;
            Settings.deviceIP[5] = xmlDoc.SelectNodes("settings/device5ip").Item(0).InnerText;
            Settings.port = int.Parse(xmlDoc.SelectNodes("settings/port").Item(0).InnerText);
            Settings.portDevice = Settings.port;
            Settings.APIaddress = xmlDoc.SelectNodes("settings/apiaddress").Item(0).InnerText;
            Settings.APIAppId = int.Parse(xmlDoc.SelectNodes("settings/apiappid").Item(0).InnerText);
            Settings.APIExperienceId = int.Parse(xmlDoc.SelectNodes("settings/apiexperienceid").Item(0).InnerText);
            Settings.APICheckCountdown = int.Parse(xmlDoc.SelectNodes("settings/apicheckcountdown").Item(0).InnerText);
            Settings.musicVolume = float.Parse(xmlDoc.SelectNodes("settings/musicVolume").Item(0).InnerText);
            Settings.soundVolume = float.Parse(xmlDoc.SelectNodes("settings/soundVolume").Item(0).InnerText);
            Settings.heartBeatTimer = float.Parse(xmlDoc.SelectNodes("settings/heartBeatTimer").Item(0).InnerText);
            //Settings.roundToWin = int.Parse(xmlDoc.SelectNodes("settings/roundToWin").Item(0).InnerText);
            //Settings.timePerInstruction = float.Parse(xmlDoc.SelectNodes("settings/timePerInstruction").Item(0).InnerText);
            //Settings.timePerInstructionRoundDecrease = float.Parse(xmlDoc.SelectNodes("settings/timePerInstructionRoundDecrease").Item(0).InnerText);
            //Settings.shipHealthMax = int.Parse(xmlDoc.SelectNodes("settings/shipHealthMax").Item(0).InnerText);
            //Settings.shipHealthDamageTrigger = int.Parse(xmlDoc.SelectNodes("settings/shipHealthDamageTrigger").Item(0).InnerText);
            //Settings.instructionsToWinRound = int.Parse(xmlDoc.SelectNodes("settings/instructionsToWinRound").Item(0).InnerText);
            //Settings.amountOfPauseBetweenBlackhole = int.Parse(xmlDoc.SelectNodes("settings/amountOfPauseBetweenBlackhole").Item(0).InnerText);
            //Settings.blackholeInputNeeded = int.Parse(xmlDoc.SelectNodes("settings/blackholeInputNeeded").Item(0).InnerText);
            //Settings.chanceForBlackholePercent = int.Parse(xmlDoc.SelectNodes("settings/chanceForBlackholePercent").Item(0).InnerText);
            //Settings.chanceForBlackholeIncreaseByRound = int.Parse(xmlDoc.SelectNodes("settings/chanceForBlackholeIncreaseByRound").Item(0).InnerText);
            //Settings.timeToAvoidBlackHole = float.Parse(xmlDoc.SelectNodes("settings/timeToAvoidBlackHole").Item(0).InnerText);
            //Settings.timeBlackHoleEffect = float.Parse(xmlDoc.SelectNodes("settings/timeBlackHoleEffect").Item(0).InnerText);
            Settings.launchSequenceCountdown = int.Parse(xmlDoc.SelectNodes("settings/launchSequenceCountdown").Item(0).InnerText);
            //Settings.digitalControlStart = int.Parse(xmlDoc.SelectNodes("settings/digitalControlStart").Item(0).InnerText);
            //Settings.digitalControlEndMinPlayers = int.Parse(xmlDoc.SelectNodes("settings/digitalControlEndMinPlayers").Item(0).InnerText);
            //Settings.digitalControlEndMaxPlayers = int.Parse(xmlDoc.SelectNodes("settings/digitalControlEndMaxPlayers").Item(0).InnerText);
            Settings.arduinoPinWin = int.Parse(xmlDoc.SelectNodes("settings/arduinoPinWin").Item(0).InnerText);
            Settings.arduinoPinLoss = int.Parse(xmlDoc.SelectNodes("settings/arduinoPinLoss").Item(0).InnerText);
            Settings.arduinoTimerEnd = int.Parse(xmlDoc.SelectNodes("settings/arduinoTimerEnd").Item(0).InnerText);
            Settings.timeOutSeconds = int.Parse(xmlDoc.SelectNodes("settings/timeOutSeconds").Item(0).InnerText);
            Settings.showDebug = bool.Parse(xmlDoc.SelectNodes("settings/showdebug").Item(0).InnerText);

            Debug.Log("New XML ReadFromXML Start");
            Settings.arduinoDamageLightsPinOne = int.Parse(xmlDoc.SelectNodes("settings/arduinoDamageLightsPinOne").Item(0).InnerText);
            Settings.arduinoDamageLightsPinTwo = int.Parse(xmlDoc.SelectNodes("settings/arduinoDamageLightsPinTwo").Item(0).InnerText);
            Settings.arduinoDamageLightsPinThree = int.Parse(xmlDoc.SelectNodes("settings/arduinoDamageLightsPinThree").Item(0).InnerText);
            Settings.arduinoBlackHoleWarningLightsPin = int.Parse(xmlDoc.SelectNodes("settings/arduinoBlackHoleWarningLightsPin").Item(0).InnerText);
            Settings.arduinoBlackHoleInstanceLightsPin = int.Parse(xmlDoc.SelectNodes("settings/arduinoBlackHoleInstanceLightsPin").Item(0).InnerText);
            Settings.arduinoInitializeLightsPin = int.Parse(xmlDoc.SelectNodes("settings/arduinoInitializeLightsPin").Item(0).InnerText);
            Settings.arduinoFlexMaxPinTime = int.Parse(xmlDoc.SelectNodes("settings/arduinoFlexMaxPinTime").Item(0).InnerText);
            Debug.Log("New XML ReadFromXML End");
            Debug.Log(Settings.arduinoFlexMaxPinTime);
        }
        else
        {
            Debug.Log("Error: 'StreamingAssets/Settings.xml' not found");
        }
    }

    public static SettingsPlayerCount LoadSettingsForPlayerCount(int index)
    {
        SettingsPlayerCount settingsPlayer = new SettingsPlayerCount();
        int playerCount = index + 1;
        playerCount = Mathf.Clamp(playerCount, 3, 6);

        string filePath = Path.Combine(Application.streamingAssetsPath, "Settings" + playerCount + "Players.xml");
        if (File.Exists(filePath))
        {


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(filePath));

            settingsPlayer.timePerInstruction = float.Parse(xmlDoc.SelectNodes("settings/timePerInstruction").Item(0).InnerText);
            settingsPlayer.timePerInstructionRoundDecrease = float.Parse(xmlDoc.SelectNodes("settings/timePerInstructionRoundDecrease").Item(0).InnerText);
            settingsPlayer.shipHealthMax = int.Parse(xmlDoc.SelectNodes("settings/shipHealthMax").Item(0).InnerText);
            settingsPlayer.shipHealthDamageTrigger = int.Parse(xmlDoc.SelectNodes("settings/shipHealthDamageTrigger").Item(0).InnerText);
            settingsPlayer.instructionsToWinRound = int.Parse(xmlDoc.SelectNodes("settings/instructionsToWinRound").Item(0).InnerText);
            settingsPlayer.amountOfPauseBetweenBlackhole = int.Parse(xmlDoc.SelectNodes("settings/amountOfPauseBetweenBlackhole").Item(0).InnerText);
            settingsPlayer.blackholeInputNeeded = int.Parse(xmlDoc.SelectNodes("settings/blackholeInputNeeded").Item(0).InnerText);
            settingsPlayer.chanceForBlackholePercent = int.Parse(xmlDoc.SelectNodes("settings/chanceForBlackholePercent").Item(0).InnerText);
            settingsPlayer.chanceForBlackholeIncreaseByRound = int.Parse(xmlDoc.SelectNodes("settings/chanceForBlackholeIncreaseByRound").Item(0).InnerText);
            settingsPlayer.timeToAvoidBlackHole = float.Parse(xmlDoc.SelectNodes("settings/timeToAvoidBlackHole").Item(0).InnerText);
            settingsPlayer.timeBlackHoleEffect = float.Parse(xmlDoc.SelectNodes("settings/timeBlackHoleEffect").Item(0).InnerText);

            string digitalCText = xmlDoc.SelectNodes("settings/digitalControlAmount").Item(0).InnerText;
            string[] digitalCTextSplit = digitalCText.Split(',');
            settingsPlayer.digitalControlAmount = new int[digitalCTextSplit.Length];
            for (int k = 0; k < digitalCTextSplit.Length; k++)
            {
                settingsPlayer.digitalControlAmount[k] = int.Parse(digitalCTextSplit[k]);
            }


            return settingsPlayer;

        }

        else
        {
            Debug.Log("Error: 'StreamingAssets/Settings.xml' not found");
            return null;
        }
    }
    public static void LoadDigitalControls()

    {
        Database.digitalControls = new List<DigitalControl>();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Digital_Controls.xml");
        if (File.Exists(filePath))
        {


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(filePath));

            XmlNodeList controlModules = xmlDoc.SelectNodes("control_modules");
            for (int i = 0; i < controlModules.Count; i++)
            {

                XmlNodeList content = controlModules[i].ChildNodes;
                for (int j = 0; j < content.Count; j++)
                {

                    string typeText = content[j].SelectNodes("type").Item(0).InnerText;
                    DigitalControl.ControlType type;
                    if (typeText == "knob")
                        type = DigitalControl.ControlType.knob;
                    else if (typeText == "fader")
                        type = DigitalControl.ControlType.fader;
                    else if (typeText == "switch")
                        type = DigitalControl.ControlType.switchtype;
                    else
                        type = DigitalControl.ControlType.button;

                    string configurationText = content[j].SelectNodes("configuration").Item(0).InnerText;
                    string[] configurationSplit = configurationText.Split(',');
                    float[] configurationInt = new float[configurationSplit.Length];
                    for (int k = 0; k < configurationSplit.Length; k++)
                    {
                        configurationInt[k] = float.Parse(configurationSplit[k]);
                    }


                    for (int k = 0; k < content.Count; k++)
                    {
                        Database.digitalControls.Add(new DigitalControl(type, configurationInt));
                        //Debug.Log(Database.digitalControls[k].type);
                    }





                }

            }
        }
        else
        {
            Debug.Log("Error: 'StreamingAssets/Digital_Controls.xml' not found");
        }
    }

    public static List<PhysicalControl> LoadPhysicalControls(int id)
    {
        List<PhysicalControl> physicalControls = new List<PhysicalControl>();

        string filePath = Path.Combine(Application.streamingAssetsPath, "Physical_Controls.xml");
        if (File.Exists(filePath))
        {


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(filePath));

            XmlNodeList controlModules = xmlDoc.SelectNodes("control_modules/device");

            for (int i = 0; i < controlModules.Count; i++)
            {
                if (int.Parse(controlModules[i].SelectNodes("id").Item(0).InnerText) == id)
                {
                    //Debug.Log("Right ID");
                    XmlNodeList controls = controlModules[i].SelectNodes("control");
                    for (int j = 0; j < controls.Count; j++)
                    {
                        int controlCount = int.Parse(controls[j].SelectNodes("count").Item(0).InnerText);

                        string lcdPin;
                        if (controls[j].SelectNodes("lcd")?.Item(0)?.InnerText == null || controls[j].SelectNodes("lcd")?.Item(0)?.InnerText == "")
                        {
                            lcdPin = "";

                        }
                        else
                        {
                            lcdPin = controls[j].SelectNodes("lcd").Item(0).InnerText;

                        }

                        string typeText = controls[j].SelectNodes("type").Item(0).InnerText;
                        PhysicalControl.PhysicalControlType type;
                        if (typeText == "rocker_switch")
                            type = PhysicalControl.PhysicalControlType.rockerSwitch;
                        else if (typeText == "latching_button")
                            type = PhysicalControl.PhysicalControlType.latchingButton;
                        else if (typeText == "joystick")
                            type = PhysicalControl.PhysicalControlType.joystick;
                        else if (typeText == "lcd")
                            type = PhysicalControl.PhysicalControlType.lcd;
                        else
                            type = PhysicalControl.PhysicalControlType.knob;

                        string configurationText = controls[j].SelectNodes("configuration").Item(0).InnerText;
                        string[] configurationSplit = configurationText.Split(',');
                        float[] configurationInt = new float[configurationSplit.Length];
                        for (int k = 0; k < configurationSplit.Length; k++)
                        {
                            configurationInt[k] = float.Parse(configurationSplit[k]);
                        }

                        string pinText = controls[j].SelectNodes("pin").Item(0).InnerText;
                        string[] pinSplit = pinText.Split(',');
                        int[] pinInt = new int[pinSplit.Length];
                        for (int k = 0; k < pinSplit.Length; k++)
                        {
                            pinInt[k] = int.Parse(pinSplit[k]);
                        }

                        int[] ledInt;

                        if (controls[j].SelectNodes("led")?.Item(0)?.InnerText == null || controls[j].SelectNodes("led")?.Item(0)?.InnerText == "")
                        {
                            ledInt = new int[0];

                        }
                        else
                        {
                            string ledText = controls[j].SelectNodes("led").Item(0).InnerText;
                            string[] ledSplit = ledText.Split(',');
                            ledInt = new int[ledSplit.Length];
                            for (int k = 0; k < ledSplit.Length; k++)
                            {
                                ledInt[k] = int.Parse(ledSplit[k]);
                            }
                        }

                        /*
                                                for (int k = 0; k < controlCount; k++)
                                                {
                                                    physicalControls.Add(new PhysicalControl(j * k, type, pinInt[k], ledInt[k], lcdPin, configurationInt));
                                                }*/
                        PhysicalControl controlToAdd = new PhysicalControl(j, type, pinInt, ledInt, lcdPin, configurationInt);
                        if (controlToAdd.type == PhysicalControl.PhysicalControlType.knob)
                        {
                            controlToAdd.knobMax = float.Parse(controls[j].SelectNodes("knobmax").Item(0).InnerText);
                            controlToAdd.knobMin = float.Parse(controls[j].SelectNodes("knobmin").Item(0).InnerText);
                        }
                        physicalControls.Add(controlToAdd);


                    }


                }
                else
                {
                    // Debug.Log("Wrong ID");

                }



            }
        }
        else
        {
            Debug.Log("Error: 'StreamingAssets/Physical_Controls.xml' not found");
        }
        return physicalControls;
    }
    public static void LoadControlNames()

    {
        Database.ControlNames = new string[0];
        string filePath = Path.Combine(Application.streamingAssetsPath, "Control_Names.xml");
        if (File.Exists(filePath))
        {


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(System.IO.File.ReadAllText(filePath));

            foreach (XmlNode node in xmlDoc.SelectNodes("control_names"))
            {
                XmlNodeList content = node.ChildNodes;
                string[] controlNames = new string[content.Count];
                for (int i = 0; i < content.Count; i++)
                {
                    //System.Array.Resize<string>(ref Database.ControlNames, Database.ControlNames.Length + 1);
                    controlNames[i] = content[i].InnerText;

                }
                Database.SetControlName(controlNames);

            }
            Debug.Log(Database.ControlNames.Length);

        }
        else
        {
            Debug.Log("Error: 'StreamingAssets/Control_Names.xml' not found");
        }
    }

}