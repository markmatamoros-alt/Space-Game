using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSceneController : MonoBehaviour
{
    public FMNetworkManager fmNetworkManager;
    public PlayerDevice thisDevice;
    public SFXManager SFX;
    public ArduinoController arduinoController;
    public UDPReceive udpReceive;
    public UDPSend udpSend;

    public int currentSequence;
    public GameObject UIDebugText;

    public GameObject UIWaiting, UIReadyButton, UIStartCountdown, UIGame, UIEnd, UIWin;

    public UIPlayerStart uIPlayerStart;
    public UIPlayerGameControls uIPlayerGameControls;
    public Text UILaunchSequenceText, UIModulePlayerName, UIModulePlayerNameDark, UISpaceCrewPlayerName, UISpaceCrewPlayerNameDark, UIEndScore, UIEndSequence, UIEndScoreWin, UIEndSequenceWin;
    public RectTransform UILaunchSequenceFill;
    public Text UICurrentInstruction;
    public Image UICUrrentInstructionBar;
    public Animator UIGameScreenAnim, UIHyperdrive;
    public Text UISequenceCompletedNumber, UISequenceCompletedSuccessText;
    float launchSequenceCountTimerCurrent, launchSequenceCountCurrent;
    float instructionTimerCurrent;
    bool hasInstruction;
    int phase;

    float noInstructionTimer;
    float noInstructionTimerLimit = 1;
    Color cGreen, cYellow, cRed;

    void Start()
    {
        ReadFromXML.LoadDeviceIP();

        //Debug
        ReadFromXML.LoadControlNames();
        //ReadFromXML.LoadSettings();
        thisDevice = new PlayerDevice();
        thisDevice.id = ReadFromXML.LoadDeviceID();
        Settings.portDevice = Settings.port + (thisDevice.id + 1);
        thisDevice.physicalControls = new List<PhysicalControl>();
        thisDevice.digitalControls = new List<DigitalControl>();
        udpSend.init();
        udpReceive.init();
        if (false)
        {
            UIDebugText.SetActive(true);
        }
        else
        {
            UIDebugText.SetActive(false);

        }
        //fmNetworkManager.ClientSettings.ServerIP=Settings.ip;
        //fmNetworkManager.ClientSettings.ClientListenPort=Settings.portDevice;
        //fmNetworkManager.Action_InitAsClient();

        //SendUDPMessage("{UDPMessage:[{'sender':'client', 'id':" + thisDevice.id + ", 'msg':'Connected', 'val':'0'}]}");
        SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"Connected\", \"val\":\"0\"}");
        //Debug.Log("Device id:" + thisDevice.id);

        SFX.LoadPlayerSFXDatabase();
        SetupColors();
        UIHideAll();
        GoToWaiting();
        SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"RequestPhysicalControls\"}");

    }
    string[] randomStringText = { "Test test testingly testing", "Amazon is a company also", "Google is a company", "Space Pants that are really nice", "Super Lord Darth Vader", "Simple" };

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GoToWaiting();
        }

        if (Input.GetKeyUp(KeyCode.F1))
        {
            arduinoController.SetLCDName("pL_LCD", randomStringText[Random.Range(0, randomStringText.Length)]);
            arduinoController.SetLCDName("pR_LCD", randomStringText[Random.Range(0, randomStringText.Length)]);
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            arduinoController.ClearLCDs();
        }
        if (Input.GetKeyUp(KeyCode.F5))
        {
            SetReady(false);
        }
        UDPCheckForMessages();
        if (phase == 1)
        {
            LaunchSequence();
        }
        if (phase == 2)
        {
            if (thisDevice.ready)
                DuringGame();
        }

    }
    void GoToWaiting()
    {
        UIHideAll();
        ResetPlayer();
        UIWaiting.SetActive(true);
        UIReadyButton.SetActive(true);

        currentSequence = 0;

        phase = 0;
        if (!Settings.LCDOnlyNames)
            SetLCDTextOnArduino("Unready");
    }
    void ResetPlayer()
    {
        EndBlackhole();
        thisDevice.ready = false;
        thisDevice.score = 0;
        //uIPlayerStart.ResetReady();
        SetReady(false);
        thisDevice.digitalControls.Clear();
    }
    float launchSequenceMaxTimer;
    void StartLaunchSequence(float _time)
    {
        launchSequenceMaxTimer = _time;
        phase = 1;
        launchSequenceCountCurrent = launchSequenceMaxTimer;
        launchSequenceCountTimerCurrent = 1;
        if (thisDevice.ready == true)
        {
            StartLaunchSequenceUI();
        }
    }

    void StartLaunchSequenceUI()
    {
        UIReadyButton.SetActive(false);
        UIStartCountdown.SetActive(true);

        UILaunchSequenceText.text = "Launch in " + launchSequenceCountCurrent.ToString();

        if (!Settings.LCDOnlyNames)
            SetLCDTextOnArduino(launchSequenceCountCurrent.ToString());
    }

    void HideLaunchSequenceUI()
    {
        UIReadyButton.SetActive(true);
        UIStartCountdown.SetActive(false);
    }

    void StopLaunchSequence()
    {
        UIReadyButton.SetActive(true);

        UIStartCountdown.SetActive(false);
        phase = 0;

    }

    void LaunchSequence()
    {
        if (Input.anyKeyDown)
        {
            //HideLaunchSequenceUI();
            //SetReady(false);

        }

        if (launchSequenceCountTimerCurrent > 0)
        {
            launchSequenceCountTimerCurrent -= Time.deltaTime;
            UILaunchSequenceFill.localScale = new Vector3(Tools.map(launchSequenceCountTimerCurrent + launchSequenceCountCurrent, 0, launchSequenceMaxTimer + 1, 0, 1), 1, 1);
        }
        else
        {
            if (launchSequenceCountCurrent > 0)
            {
                launchSequenceCountCurrent -= 1;
                UILaunchSequenceText.text = "Launch in " + launchSequenceCountCurrent.ToString();
                if (!Settings.LCDOnlyNames)
                    SetLCDTextOnArduino(launchSequenceCountCurrent.ToString());

                launchSequenceCountTimerCurrent = 1;

            }
            else
            {

            }


        }
    }
    void GameStart()
    {
        phase = 2;
        //UIWaiting.SetActive(false);
        if (thisDevice.ready)
        {
            EndBlackhole();
            HideAfterTime(UIWaiting, 1);
            currentSequence = 0;
        }
        else
        {
            UIReadyButton.SetActive(false);

        }
    }
    void SetupColors()
    {
        cGreen = Tools.HexColor("#68FFB4");
        cYellow = Tools.HexColor("#FAFF68");
        cRed = Tools.HexColor("#FF7070");

        string newID = (thisDevice.id + 1).ToString();
        UIModulePlayerName.text = UIModulePlayerName.text + newID;
        UIModulePlayerNameDark.text = UIModulePlayerNameDark.text + newID;
        UISpaceCrewPlayerName.text = UISpaceCrewPlayerName.text + newID;
        UISpaceCrewPlayerNameDark.text = UISpaceCrewPlayerNameDark.text + newID;
    }
    void DuringGame()
    {

        if (hasInstruction)
        {
            if (instructionTimerCurrent > 0)
            {
                float currentTimer = Tools.map(instructionTimerCurrent, 0, thisDevice.currentInstructions.currentTime, 0, 1);
                UICUrrentInstructionBar.GetComponent<RectTransform>().localScale = new Vector3(currentTimer, 1, 1);
                if (currentTimer > 0.5f)
                {
                    UICUrrentInstructionBar.color = Color.Lerp(cYellow, cGreen, (currentTimer * 2) - 1f);

                }
                else
                {
                    //                    Debug.Log("bar" + currentTimer * 2);
                    UICUrrentInstructionBar.color = Color.Lerp(cRed, cYellow, (currentTimer * 2));

                }
                instructionTimerCurrent -= Time.deltaTime;
            }
            else
            {//fail
                FailInstruction();
            }
        }
        else
        {
            if (noInstructionTimer > 0)
            {
                noInstructionTimer -= Time.deltaTime;
            }
            else
            {
                RequestNewInstruction();
            }
        }
    }

    void RequestNewInstruction()
    {
        SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"requestinstruction\", \"val\":\"0\"}");


    }

    void ReceiveNewInstruction()
    {
        string instructionText = "";
        if (thisDevice.currentInstructions.isPhysical)
        {
            //Physical Controls
            if (thisDevice.currentInstructions.physicalType == PhysicalControl.PhysicalControlType.knob)
            {
                instructionText = "Set <b>" + thisDevice.currentInstructions.instructionName + "</b> to <b>" + thisDevice.currentInstructions.value.ToString() + "</b>";
            }
            else
            {
                string stateToChangeTo;
                if (thisDevice.currentInstructions.value == 0)
                    stateToChangeTo = "on";
                else
                    stateToChangeTo = "off";

                instructionText = "Set <b>" + thisDevice.currentInstructions.instructionName + " #" + (thisDevice.currentInstructions.pinOrder + 1).ToString() + "</b>  to <b>" + stateToChangeTo + "</b>";

            }
        }
        else
        {
            //Digital Controls
            if (thisDevice.currentInstructions.type == DigitalControl.ControlType.button)
            {
                instructionText = "Press <b>" + thisDevice.currentInstructions.instructionName + "</b>";
            }
            if (thisDevice.currentInstructions.type == DigitalControl.ControlType.fader)
            {
                instructionText = "Set <b>" + thisDevice.currentInstructions.instructionName + "</b> to <b>" + thisDevice.currentInstructions.value.ToString() + "</b>";
            }
            if (thisDevice.currentInstructions.type == DigitalControl.ControlType.knob)
            {
                instructionText = "Set <b>" + thisDevice.currentInstructions.instructionName + "</b> to <b>" + thisDevice.currentInstructions.value.ToString() + "</b>";
            }
            if (thisDevice.currentInstructions.type == DigitalControl.ControlType.switchtype)
            {
                if (thisDevice.currentInstructions.value == 0)
                    instructionText = "Turn off <b>" + thisDevice.currentInstructions.instructionName + "</b>";
                else
                    instructionText = "Turn on <b>" + thisDevice.currentInstructions.instructionName + "</b>";

            }
        }
        UICurrentInstruction.text = instructionText;
        instructionTimerCurrent = thisDevice.currentInstructions.currentTime;
        hasInstruction = true;

    }

    void CompleteInstruction()
    {
        UICurrentInstruction.text = "";
        hasInstruction = false;
        noInstructionTimer = noInstructionTimerLimit;
        SFX.PlaySFX("sfx/player/success.wav");
        thisDevice.score += 10;
        UIGameScreenAnim.SetTrigger("success");

    }

    void FailInstruction()
    {
        SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"FailInstruction\"}");
        UICurrentInstruction.text = "";
        hasInstruction = false;
        noInstructionTimer = noInstructionTimerLimit;
        SFX.PlaySFX("sfx/player/fail.wav");
        UIGameScreenAnim.SetTrigger("fail");

    }


    public void ChangeControlValue(int _which, float _val)
    {
        Debug.Log("changing " + _which + " to " + _val);
        Debug.Log(thisDevice.digitalControls[_which].value);
        thisDevice.digitalControls[_which].value = _val;

        if (thisDevice.digitalControls[_which].value != thisDevice.digitalControls[_which].lastValue)
        {

            SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"ChangeControlValue\", \"controlID\":\"" + _which.ToString() + "\", \"val\":\"" + _val.ToString() + "\"}");

        }
        thisDevice.digitalControls[_which].lastValue = thisDevice.digitalControls[_which].value;

    }
    float valueThreshold = 0.1f;
    public void ChangePhysicalControlValue(int _which, int _pin, float _val)
    {

        thisDevice.physicalControls[_which].value[_pin] = _val;
        /*
        bool isInsideThreshold;
        if (thisDevice.physicalControls[_which].value[_pin] > thisDevice.physicalControls[_which].lastValue[_pin] - valueThreshold && thisDevice.physicalControls[_which].value[_pin] < thisDevice.physicalControls[_which].lastValue[_pin] + valueThreshold)
        {
            isInsideThreshold = true;
        }
        else
        {
            isInsideThreshold = false;
        }*/

        if (thisDevice.physicalControls[_which].value[_pin] != thisDevice.physicalControls[_which].lastValue[_pin])
        {
            Debug.Log("changing " + _which + "on pin " + _pin + " to " + _val);
            Debug.Log(thisDevice.physicalControls[_which].value[_pin]);
            SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"ChangePhysicalControlValue\", \"controlID\":\"" + _which.ToString() + "\", \"pin\":\"" + _pin.ToString() + "\", \"val\":\"" + _val.ToString() + "\"}");

        }
        thisDevice.physicalControls[_which].lastValue[_pin] = thisDevice.physicalControls[_which].value[_pin];

    }

    void SequenceComplete()
    {
        currentSequence += 1;
        UICurrentInstruction.text = "";
        instructionTimerCurrent = 0;
        UIGameScreenAnim.SetTrigger("endsequence");
        UISequenceCompletedNumber.text = currentSequence.ToString();
        string[] _welldonearray={"Well done!", "Good job!", "Fantastic!", "Perfect!", "Awesome!", "Impressive!", "Outstanding!", "That was good!", "Great!"};
        string _welldonetext=_welldonearray[Random.Range(0,_welldonearray.Length)];
        string[] _morearray={"Get ready for more!", "More incoming!", "Get ready!", "Prepare for more!", "Buckle up!", "Brace yourselves!", "Be alert!", "Watch out for more!"};
        string _moretext=_morearray[Random.Range(0,_morearray.Length)];        
        UISequenceCompletedSuccessText.text=_welldonetext+" "+_moretext;
        UIHyperdrive.gameObject.SetActive(true);
        UIHyperdrive.SetTrigger("in");

        hasInstruction = false;
        arduinoController.stopReading = true;
        if (!Settings.LCDOnlyNames)
            SetLCDTextOnArduino("Sequence " + currentSequence.ToString() + " completed!");
        else
            arduinoController.ClearLCDs();


    }
    void DamageTrigger()
    {

    }

    void StartBlackholeInput()
    {
        if (thisDevice.ready)
        {
            arduinoController.readBlackHoleJoysticks = true;

        }
    }
    void StartBlackholeEffect()
    {
        if (thisDevice.ready)
        {
            UIGame.GetComponent<Animator>().SetBool("blackhole", true);
            arduinoController.readBlackHoleJoysticks = false;
            //if (!Settings.LCDOnlyNames)
            //SetLCDTextOnArduino("010101010101100");
        }
    }

    void EndBlackhole()
    {
        if (thisDevice.ready)
        {
            arduinoController.readBlackHoleJoysticks = false;
            UIGame.GetComponent<Animator>().SetBool("blackhole", false);
            //SetLCDNamesOnArduino();
        }
    }
    void EndGame()
    {
        //UIHideAll();
        UIGameScreenAnim.SetTrigger("end");
        UIEnd.SetActive(true);
        phase = 3;
        SFX.PlaySFX("sfx/player/gameover.wav");
        UIEndScore.text = thisDevice.score.ToString();
        UIEndSequence.text = currentSequence.ToString();
        arduinoController.stopReading = true;

        if (!Settings.LCDOnlyNames)
            SetLCDTextOnArduino("FATAL ERROR!");
        else
            arduinoController.ClearLCDs();



    }
    void WinGame()
    {
        //UIHideAll();
        UIGameScreenAnim.SetTrigger("end");
        UIWin.SetActive(true);
        phase = 3;
        UIEndScoreWin.text = thisDevice.score.ToString();
        UIEndSequenceWin.text = currentSequence.ToString();
        arduinoController.stopReading = true;

        if (!Settings.LCDOnlyNames)
            SetLCDTextOnArduino("SUCCESS!");
        else
            arduinoController.ClearLCDs();


    }
    void UIHideAll()
    {
        UIWaiting.SetActive(false);
        UIReadyButton.SetActive(false);
        UIStartCountdown.SetActive(false);
        UIGame.SetActive(false);
        UIEnd.SetActive(false);
        UIWin.SetActive(false);

    }


    void SendUDPMessage(string _msg)
    {
        //fmNetworkManager.SendToServer(_msg);
        udpSend.sendString(_msg);


    }
    string lastReceivedUDPPacket = "";
    void UDPCheckForMessages()
    {

        //if (udpReceive.lastReceivedUDPPacket != lastReceivedUDPPacket)
        if (udpReceive.allReceivedUDPPackets.Count > 0)
        {
            lastReceivedUDPPacket = udpReceive.lastReceivedUDPPacket;
            for (int i = 0; i < udpReceive.allReceivedUDPPackets.Count; i++)
            {
                ReceiveMessage(udpReceive.allReceivedUDPPackets[i]);
            }
            udpReceive.allReceivedUDPPackets.Clear();


        }
    }

    public void Action_ProcessStringData(string _string)
    {
        ReceiveMessage(_string);
    }
    public void ReceiveMessage(string _string)
    {
        if (_string != "")
        {
            //Debug.Log("Message Received");
            UDPMessage msg = JsonUtility.FromJson<UDPMessage>(_string);

            if (msg.sender == "server" && (msg.id == -1 || msg.id == thisDevice.id))
            {
                if (msg.msg == "WaitingForPlayers")
                {
                    Debug.Log("player Waiting");
                    GoToWaiting();
                }

                if (msg.msg == "StartLaunchSequence")
                {
                    StartLaunchSequence(int.Parse(msg.val));
                }

                if (msg.msg == "StopLaunchSequence")
                {
                    StopLaunchSequence();
                }
                if (msg.msg == "GameStart")
                {
                    GameStart();
                }
                if (msg.msg == "UpdateControlDevices")
                {
                    if (thisDevice.ready)
                    {
                        thisDevice.digitalControls.Clear();
                        for (int i = 0; i < msg.digitalControl.Length; i++)
                        {
                            thisDevice.digitalControls.Add(msg.digitalControl[i]);
                        }

                        thisDevice.physicalControls.Clear();
                        for (int i = 0; i < msg.physicalControl.Length; i++)
                        {
                            thisDevice.physicalControls.Add(msg.physicalControl[i]);
                        }
                        arduinoController.Setup();

                        SetLCDNamesOnArduino();
                        arduinoController.stopReading = false;

                        uIPlayerGameControls.SetupContainer();
                        UIGameScreenAnim.SetTrigger("newsequence");

                        UIGame.SetActive(true);

                        RequestNewInstruction();
                    }

                }
                if (msg.msg == "SetupPhysicalControls")
                {
                    thisDevice.physicalControls.Clear();
                    for (int i = 0; i < msg.physicalControl.Length; i++)
                    {
                        thisDevice.physicalControls.Add(msg.physicalControl[i]);
                    }
                }
                if (msg.msg == "NewInstruction")
                {
                    if (thisDevice.ready)
                    {
                        thisDevice.currentInstructions = msg.instruction;
                        ReceiveNewInstruction();
                    }
                }
                if (msg.msg == "CorrectInstruction")
                {
                    if (thisDevice.ready)

                        CompleteInstruction();

                }
                if (msg.msg == "DamageTrigger")
                {
                    DamageTrigger();
                }

                if (msg.msg == "SequenceComplete")
                {
                    if (thisDevice.ready)
                        SequenceComplete();
                }

                if (msg.msg == "EndGame")
                {
                    if (thisDevice.ready)
                        EndGame();
                }
                if (msg.msg == "EndGameTimeOut")
                {
                    EndGame();
                }
                if (msg.msg == "WinGame")
                {
                    if (thisDevice.ready)
                        WinGame();
                }
                if (msg.msg == "EndBlackhole")
                {
                    if (thisDevice.ready)
                        EndBlackhole();
                }

                if (msg.msg == "StartBlackholeEffect")
                {
                    if (thisDevice.ready)
                        StartBlackholeEffect();
                }
                if (msg.msg == "StartBlackholeInput")
                {
                    if (thisDevice.ready)
                        StartBlackholeInput();
                }
            }
        }
    }

    void SetLCDNamesOnArduino()
    {
        for (int j = 0; j < thisDevice.physicalControls.Count; j++)
        {
            if (thisDevice.physicalControls[j].lcdPin != "" && thisDevice.physicalControls[j].name != "")
            {
                Debug.Log(j + ":" + "Set arduino LCD " + thisDevice.physicalControls[j].lcdPin + " to " + thisDevice.physicalControls[j].name);

                arduinoController.SetLCDName(thisDevice.physicalControls[j].lcdPin, thisDevice.physicalControls[j].name);

            }

        }

    }

    void SetLCDTextOnArduino(string _text)
    {
        int amountNotFound = 0;

        for (int j = 0; j < thisDevice.physicalControls.Count; j++)
        {
            Debug.Log(j + ":" + thisDevice.physicalControls[j].lcdPin);
            if (thisDevice.physicalControls[j].lcdPin != "")
            {
                Debug.Log(j + ":" + "Set arduino LCD " + thisDevice.physicalControls[j].lcdPin + " to " + _text);

                arduinoController.SetLCDName(thisDevice.physicalControls[j].lcdPin, _text);

            }
            else
            {
                amountNotFound += 1;
            }
        }
        if (amountNotFound == thisDevice.physicalControls.Count)
        {
            SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"RequestPhysicalControls\"}");

        }
    }
    public void SetReady(bool state)
    {
        Debug.Log("Set Ready:" + state);
        if (state == true)
        {
            if (thisDevice.ready == false)
            {
                SFX.PlaySFX("sfx/player/player_ready.wav");
                thisDevice.ready = true;
                if (phase == 1)
                {
                    StartLaunchSequenceUI();

                }
                else
                {

                    if (!Settings.LCDOnlyNames)
                        SetLCDTextOnArduino("Ready");
                }
            }
        }
        else
        {
            if (thisDevice.ready == true)
            {
                thisDevice.ready = false;

                if (!Settings.LCDOnlyNames)
                    SetLCDTextOnArduino("Unready");

                uIPlayerStart.ResetReady();
            }
        }
        Debug.Log("Ready:" + thisDevice.ready);

        SendUDPMessage("{\"sender\":\"client\", \"id\":" + thisDevice.id + ", \"msg\":\"setready\", \"val\":\"" + thisDevice.ready.ToString() + "\"}");

    }

    IEnumerator HideAfterTime(GameObject _which, float time)
    {
        yield return new WaitForSeconds(time);
        _which.SetActive(false);
    }

}
