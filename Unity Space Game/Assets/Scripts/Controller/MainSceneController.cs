
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    public List<SettingsPlayerCount> settingsPlayerCount;
    public FMNetworkManager fmNetworkManager;
    //public CommunicationManager communicationManager;
    public APICommunication apiCommunication;
    public ArduinoServerController arduinoServerController;
    public SFXManager SFX, Music;

    [SerializeField]
    private GameObject tunnelEnd;
    public TunnelController tunnel;
    //public RiptideDemos.RudpTransport.Unity.PlayerHosted.NetworkManager networkManager;
    public UDPReceive udpReceive;
    public UDPSend udpSend;
    //public Socket socket;
    public int phase;
    public GameObject UIDebugText;
    public GameObject UIStartInstructions, UISpaceshipPlayers, UILaunchWhenReady, UILaunchSequence, UISequenceTitle, UISequenceInGame, UISequenceCompleted, UIGameoverText, UIGameover, UIGameoverScore, UIWinText, UIWin, UIWinScore, UIBlackhole;
    [SerializeField]
    private Animator UIPrepareForInstructions, UIFlash;
    PlayerDevice[] player;
    public int requiredPlayers = 2;
    Sequence[] sequences;
    int currentSequence;

    GameStatus gameStatus;

    //bool isBlackhole;
    int blackholeCounterCurrent;
    int blackholeInputCurrent;
    float blackholeInstructionsTimerCurrent;
    float blackholeDuringTimerCurrent;
    bool blackholeDuringInstructions, blackholeDuringEffect, blackHoleInThisSequence;

    float timeOutSecondsCur;
    public RectTransform blackholeInputFillBar;
    float heartBeatTimerCurrent;
    enum GameStatus
    {
        waiting, in_session, error
    }

    int globalShipHealth;
    int launchSequenceCountCurrent;
    float launchSequenceCountTimerCurrent;
    public Text launchSequenceCountText;
    public Image launchSequenceFill;

    public UISmoothTransition progressBar;
    public UISmoothTransition progressShipIcon;
    public UISmoothTransition[] UISpaceshipPlayersParts;
    public Animator UISpaceshipPlayersFull;
    public Text currentSequenceText;
    public Text UINewSequenceNumber, UISequenceCompletedNumber, UIGameOverSequenceNumber, UIWinSequenceNumber;

    public Text[] UIWinScoreText;
    public Text[] UIWinPlaceText;
    public Text[] UIGameOverScoreText;
    public Text[] UIGameOverPlaceText;
    public Text waitingPlayerCountText;

    float apiCheckCurrent;

    //Light Additions
    int damageAudioBedCount = 0;               //tracks the damage phase by holding the damage-audio-bed counts
    bool[] damageFlags = {false,false,false};  //utilized to allow/prevent re-triggering of each 'damage-light' phase
    bool blackholeActive = false;              //utilized to allow/prevent 'damage-lights' from triggering during BlackHole events
    bool gameResetActive = false;              //utilized to allow/prevent 'damage-lights' from triggering during non-gameplay activities (resetting)

    void Start()
    {

        ReadFromDatabase();
        if (false)
        {
            UIDebugText.SetActive(true);
        }
        else
        {
            UIDebugText.SetActive(false);

        }
        //communicationManager.Setup();
        // networkManager.Setup();
        // networkManager.StartHost();

        udpSend.init();

        udpReceive.init();
        //socket.Setup();
        //SendDgram("JSON",JsonUtility.ToJson(d).ToString());

        //socket.SendDgram("JSON","{'sender':'server', 'id':-1, 'msg':'Connected', 'val':'0'}");
        //fmNetworkManager.ServerSettings.ServerListenPort=Settings.port;
        //fmNetworkManager.ClientSettings.ClientListenPort=Settings.port+1;
        //fmNetworkManager.Action_InitAsServer();
        SendUDPMessage("{\"sender\":\"server\", \"id\":-1, \"msg\":\"Connected\", \"val\":\"0\"}");
        heartBeatTimerCurrent = Settings.heartBeatTimer;
        apiCheckCurrent = Settings.APICheckCountdown;
        //apiCommunication.GetRequest(Settings.APIaddress);~
        if (Settings.APIaddress != "")
        {
            apiCommunication.SetupSocket();
        }

        Music.LoadMusicDatabase();
        SFX.LoadServerSFXDatabase();

        SetupPlayers();
        hideAllUI();
        WaitingForPlayers();

    }

    void SetupSequences()
    {
        sequences = new Sequence[Settings.roundToWin];
        for (int i = 0; i < sequences.Length; i++)
        {
            sequences[i] = new Sequence();
            sequences[i].id = i;
            sequences[i].sequenceTime = Settings.timePerInstruction - (i * Settings.timePerInstructionRoundDecrease);
        }
    }

    void SetupPlayers()
    {
        player = new PlayerDevice[6];
        for (int i = 0; i < player.Length; i++)
        {
            player[i] = new PlayerDevice();
            player[i].physicalControls = ReadFromXML.LoadPhysicalControls(i);

            Debug.Log("player physical count" + player[i].physicalControls.Count);
        }
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Reset();
        }
        UDPCheckForMessages();

        APICheck();
        HeartBeat();
        if (Settings.showDebug)
        {
            DebugControls();
        }
        if (phase == 0)
        {
            if (timeOutSecondsCur > 0)
            {
                timeOutSecondsCur -= Time.deltaTime;
            }
            else
            {
                EndGameTimeOut();
            }
        }
        if (phase == 1)
            LaunchSequence();
        if (phase == 2)
            SequenceDuring();
        if (phase == 3)
        {
            EndDisplayingScores();
        }

    }

    void DebugControls()
    {
        if (Input.GetKey(KeyCode.F5))
        {
            SendAPIMessage("testMessage", "{\"status\":\"ok\"}");
        }
        if (Input.GetKey(KeyCode.F12))
        {

            //WaitingForPlayers();
        }
        if (phase == 0) //waiting for players
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    // SendUDPMessage("{'sender':'server', 'id':-1, 'msg':'spacepressed', 'val':'0'}");

                }
                //StartLaunchSequence();
                //phase = 1;

            }
        }
        if (phase == 1) //waiting for players
        {

        }

        if (phase == 2)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    if (blackholeDuringInstructions && blackholeInstructionsTimerCurrent > 0)
                    {
                        AddBlackholeInstruction();
                    }
                }

                if (Input.GetKey(KeyCode.F1))
                {
                    StartBlackholeInstructions();
                }
                if (Input.GetKey(KeyCode.F2))
                {
                    for (int i = 0; i < player.Length; i++)
                    {
                        if (player[i].ready)
                            SendNewInstruction(i);





                    }
                }
                //StartLaunchSequence();
                //phase = 1;

            }

        }
        if (phase == 3) //waiting for players
        {
            if (Input.anyKeyDown)
            {
                Reset();
            }
        }
    }

    void ReadFromDatabase()
    {
        ReadFromXML.LoadSettings();
        settingsPlayerCount = new List<SettingsPlayerCount>();
        for (int i = 0; i < 6; i++)
        {
            settingsPlayerCount.Add(ReadFromXML.LoadSettingsForPlayerCount(i));
        }
        ReadFromXML.LoadControlNames();
        ReadFromXML.LoadDigitalControls();
    }

    public void WaitingForPlayers()
    {
        ReadFromDatabase();
        SendPhysicalControls(-1);
        gameStatus = GameStatus.waiting;
        SendGameStatusToAPI();
        UIStartInstructions.SetActive(true);
        UISpaceshipPlayers.SetActive(true);
        UILaunchWhenReady.SetActive(true);
        UISpaceshipPlayersFull.SetTrigger("reset");

        SendUDPMessage("{\"sender\":\"server\", \"id\":-1, \"msg\":\"WaitingForPlayers\", \"val\":\"0\"}");
        UIChangeReadyPlayers();
        tunnel.gameObject.SetActive(true);
        tunnelEnd.SetActive(false);
        tunnel.SetSpeed(0.01f);
        tunnel.StartSequence();


        timeOutSecondsCur = Settings.timeOutSeconds;

        Music.StopAllMusic();

        gameResetActive = false;                    //allow damage-light triggering during gameplay
        EndBlackhole();
        arduinoServerController.InitializeLights();     //set lights to their initialized state (game start)
        damageAudioBedCount = 0;                        //reset damage count - redundancy
        
        //reset damage flags for damage-lights triggering
        for (int i = 0; i < damageFlags.Length; i++)
        {
            damageFlags[i] = false;
        }
    }

    void ResetPlayers()
    {
        for (int i = 0; i < player.Length; i++)
        {
            player[i].ready = false;
            player[i].score = 0;
            if (player[i].digitalControls != null)
                player[i].digitalControls.Clear();

        }

    }

    void StartLaunchSequence()
    {
        UIStartInstructions.SetActive(false);
        UILaunchWhenReady.SetActive(false);
        UILaunchSequence.SetActive(true);
        UISpaceshipPlayersFull.SetTrigger("smallsize");
        launchSequenceCountCurrent = Settings.launchSequenceCountdown;
        launchSequenceCountText.text = launchSequenceCountCurrent.ToString();
        launchSequenceCountTimerCurrent = 1;
        SFX.PlaySFX("sfx/server/countdown.wav");

        phase = 1;
        SendUDPMessage("{\"sender\":\"server\", \"id\":-1, \"msg\":\"StartLaunchSequence\", \"val\":\"" + Settings.launchSequenceCountdown + "\"}");
        tunnel.SetSpeed(0);


    }

    void StopLaunchSequence()
    {
        UIStartInstructions.SetActive(true);
        UILaunchWhenReady.SetActive(true);
        UILaunchSequence.SetActive(false);

        SendUDPMessage("{\"sender\":\"server\", \"id\":-1, \"msg\":\"StopLaunchSequence\", \"val\":\"0\"}");

        phase = 0;
        //SendUDPMessage("{\"sender\":\"server\", \"id\":-1, \"msg\":\"StartLaunchSequence\", \"val\":\"" + Settings.launchSequenceCountdown + "\"}");
        tunnel.SetSpeed(0.1f);


    }

    void LaunchSequence()
    {
        if (launchSequenceCountTimerCurrent > 0)
        {
            launchSequenceCountTimerCurrent -= Time.deltaTime;
            launchSequenceFill.fillAmount = 1 - launchSequenceCountTimerCurrent;
        }
        else
        {
            if (launchSequenceCountCurrent > 0)
            {
                launchSequenceCountCurrent -= 1;
                launchSequenceCountText.text = launchSequenceCountCurrent.ToString();
                SFX.PlaySFX("sfx/server/countdown.wav");
                launchSequenceCountTimerCurrent = 1;

            }
            else
            {
                GameStart();
            }


        }
    }

    void GameStart()
    {
        Debug.Log("Game Start");
        gameStatus = GameStatus.in_session;
        SendGameStatusToAPI();

        //SendAPIMessage("{\"msg\":\"gameStarted\",\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + "}");
        //SendAPIMessage("From Client: gameStarted - Response {\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + "}");
        SendAPIMessage("gameStarted", "{\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + "}");

        SendUDPMessage("{\"sender\":\"server\", \"id\":-1, \"msg\":\"GameStart\"}");

        phase = 2;
        UISpaceshipPlayersFull.SetTrigger("out");

        //UISpaceshipPlayers.SetActive(false);
        HideAfterTime(UISpaceshipPlayers, 0.5f);

        UILaunchSequence.SetActive(false);
        GetSettingsFromPlayerCount();
        SetupSequences();
        globalShipHealth = GetShipMaxHealth();
        currentSequence = 0;
        blackholeCounterCurrent = 0;
        Music.PlayMusic("sfx/server/ship_soundbed.wav");

        SFX.PlaySFX("sfx/server/game_start.wav");

        SequenceStart();

    }

    void GetSettingsFromPlayerCount()
    {
        int p = GetPlayerInGameCount() - 1;
        Settings.roundToWin = settingsPlayerCount[p].digitalControlAmount.Length;
        Settings.digitalControlAmount = settingsPlayerCount[p].digitalControlAmount;
        Settings.timePerInstruction = settingsPlayerCount[p].timePerInstruction;
        Settings.timePerInstructionRoundDecrease = settingsPlayerCount[p].timePerInstructionRoundDecrease;
        Settings.shipHealthMax = settingsPlayerCount[p].shipHealthMax;
        Settings.shipHealthDamageTrigger = settingsPlayerCount[p].shipHealthDamageTrigger;
        Settings.instructionsToWinRound = settingsPlayerCount[p].instructionsToWinRound;
        Settings.amountOfPauseBetweenBlackhole = settingsPlayerCount[p].amountOfPauseBetweenBlackhole;
        Settings.blackholeInputNeeded = settingsPlayerCount[p].blackholeInputNeeded;
        Settings.chanceForBlackholePercent = settingsPlayerCount[p].chanceForBlackholePercent;
        Settings.chanceForBlackholeIncreaseByRound = settingsPlayerCount[p].chanceForBlackholeIncreaseByRound;
        Settings.timeToAvoidBlackHole = settingsPlayerCount[p].timeToAvoidBlackHole;
        Settings.timeBlackHoleEffect = settingsPlayerCount[p].timeBlackHoleEffect;

    }

    IEnumerator HideAfterTime(GameObject _which, float time)
    {
        yield return new WaitForSeconds(time);
        _which.SetActive(false);
    }


    void SequenceStart()
    {

        sequenceCompletePause = false;

        Debug.Log("sequence start");
        sequences[currentSequence].progress = 0;
        UISequenceTitle.SetActive(true);
        UISequenceInGame.SetActive(true);
        UIPrepareForInstructions.SetTrigger("in");

        progressBar.targetScale = new Vector3(1, Tools.map(sequences[currentSequence].progress, 0, Settings.instructionsToWinRound, 0, 1), 1);
        progressShipIcon.targetLocalPosition = new Vector3(0, Tools.map(sequences[currentSequence].progress, 0, Settings.instructionsToWinRound, 0, 1000), 0);
        currentSequenceText.text = "Sequence " + (currentSequence + 1);
        UINewSequenceNumber.text = (currentSequence + 1).ToString();
        ResetDamage();
        CreatePlayerControls();
        tunnel.SetSpeed(0.01f);
        tunnel.StartSequence();
        SFX.PlaySFX("sfx/server/round_begins.wav");

        if (blackHoleInThisSequence)
        {
            blackHoleInThisSequence = false;

            gameResetActive = false;    //allow damage lighting to be active during gameplay
            EndBlackhole();
        }
        else
        {
            blackholeCounterCurrent += 1;
        }

    }

    void ResetDamage()
    {
        globalShipHealth = GetShipMaxHealth();
        Music.StopAllMusicExcept("sfx/server/ship_soundbed.wav");

        arduinoServerController.InitializeLights(); //Re-initialize lights for beginning of round (or game)
        damageAudioBedCount = 0;                    //reset damage-count for proper damage-light triggering for the next round (or game)

        //Reset damage-light flags for the next round (or game)
        for (int i = 0; i < damageFlags.Length; i++)
        {
            damageFlags[i] = false;
        }
    }


    void CheckIfBlackhole()
    {
        Debug.Log("Check if Blackhole. Sequences since last blackhole:" + blackholeCounterCurrent + "/" + Settings.amountOfPauseBetweenBlackhole);
        if (!blackHoleInThisSequence && blackholeCounterCurrent >= Settings.amountOfPauseBetweenBlackhole)
        {
            float chanceToBlackhole = Settings.chanceForBlackholePercent + (Settings.chanceForBlackholeIncreaseByRound * currentSequence);
            float randomChance = Random.Range(0, 100);
            Debug.Log("chanceToBlackhole:" + chanceToBlackhole + "/" + randomChance);
            if (chanceToBlackhole > randomChance)
            {
                StartBlackholeInstructions();

            }
        }
        else
        {
        }
    }
    void StartBlackholeInstructions()
    {
        blackHoleInThisSequence = true;
        blackholeCounterCurrent = 0;
        Debug.Log("Start Blackhole Instructions");

        UIBlackhole.SetActive(true);
        blackholeInstructionsTimerCurrent = Settings.timeToAvoidBlackHole;
        blackholeDuringInstructions = true;
        blackholeInputCurrent = 0;
        blackholeInputFillBar.localScale = new Vector3(1, Tools.map(blackholeInputCurrent, 0, Settings.blackholeInputNeeded, 0, 1), 1);

        SFX.PlaySFX("sfx/server/blackhole_warning.wav");
        SendUDPMessage("{\"sender\":\"server\", \"id\":" + -1 + ", \"msg\":\"StartBlackholeInput\"}");

        blackholeActive = true;                             //prevent damage-light triggering
        arduinoServerController.turnOffDamageLightFeeds();
        arduinoServerController.BlackHoleWarningLights();   //Trigger black hole warning lights
    }

    void DuringBlackholeInstructions()
    {

        if (blackholeInstructionsTimerCurrent > 0)
        {
            blackholeInstructionsTimerCurrent -= Time.deltaTime;
        }
        else
        {
            StartBlackholeEffect();

        }
    }

    void AddBlackholeInstruction()
    {
        Debug.Log("Add Blackhole Instruction Input: " + blackholeInputCurrent + "/" + Settings.blackholeInputNeeded);

        if (blackholeInputCurrent >= Settings.blackholeInputNeeded)
        {
            AvoidBlackholeEffect();

        }
        else
        {
            blackholeInputCurrent += 1;
            blackholeInputFillBar.localScale = new Vector3(1, Tools.map(blackholeInputCurrent, 0, Settings.blackholeInputNeeded, 0, 1), 1);
        }
    }

    void AvoidBlackholeEffect()
    {

        Debug.Log("Avoid Blackhole Effect");

        UIBlackhole.GetComponent<Animator>().SetTrigger("avoided");
        blackholeDuringInstructions = false;

        gameResetActive = false;    //allow damage-lighting to be active during gameplay - this is a redundancy
        EndBlackhole();
    }

    void StartBlackholeEffect()
    {
        Debug.Log("Start Blackhole Effect");
        blackholeDuringInstructions = false;
        blackholeDuringEffect = true;

        blackholeDuringTimerCurrent = Settings.timeBlackHoleEffect;
        UIBlackhole.GetComponent<Animator>().SetTrigger("notavoided");
        SendUDPMessage("{\"sender\":\"server\", \"id\":" + -1 + ", \"msg\":\"StartBlackholeEffect\"}");
        tunnel.targetAmplitude = 0.55f;

        //blackholeActive = true;
        arduinoServerController.turnOffDamageLightFeeds();
        arduinoServerController.BlackHoleInstanceLights();  //trigger black hole instance lighting

        Music.PlayMusic("sfx/server/blackhole_soundbed.wav");
    }

    void DuringBlackholeEffect()
    {

        if (blackholeDuringTimerCurrent > 0)
        {
            blackholeDuringTimerCurrent -= Time.deltaTime;
        }
        else
        {
            UIBlackhole.GetComponent<Animator>().SetTrigger("end");

            gameResetActive = false;    //allow damage lighting to be active during game play - this is a redundancy
            EndBlackhole();
        }
    }


    void EndBlackhole()
    {
        Debug.Log("End Blackhole ");

        blackholeInstructionsTimerCurrent = 0;
        blackholeDuringTimerCurrent = 0;
        blackholeInputCurrent = 0;
        blackholeDuringInstructions = false;
        blackholeDuringEffect = false;

        SendUDPMessage("{\"sender\":\"server\", \"id\":" + -1 + ", \"msg\":\"EndBlackhole\"}");
        tunnel.targetAmplitude = 0f;

        StartCoroutine(HideAfterTime(UIBlackhole, 1f));
        //UIBlackhole.SetActive(false);
        Music.StopMusic("sfx/server/blackhole_soundbed.wav");

        blackholeActive = false;    //Allow damage lights to become active after black hole instance

        //Check if any damage phases (pertaining to damage audio beds) are active
        if (damageAudioBedCount > 0)
        {
            if (!gameResetActive)   //Check if this EndBlackhol() instance is related to a game reset.  If it isnt...
            {
                arduinoServerController.turnOffBothBlackHoleLights();       //turn off warning and instance lights.  Not necessary, but for reassurance.
                arduinoServerController.DamageLights(damageAudioBedCount);  //trigger appropriate damage phase lighting
            }
        }

    }

    void CreatePlayerControls()
    {
        int amountToCreate;
        /* 
         if (currentSequence > Settings.roundToWin / 2)
         {
             amountToCreate = 6;
         }
         else if (currentSequence > Settings.roundToWin / 4)
         {
             amountToCreate = 5;

         }
         else if (currentSequence > Settings.roundToWin / 6)
         {
             amountToCreate = 4;

         }
         else
         {
             amountToCreate = 3;

         }*/
        //int instructionAmountMax = Mathf.RoundToInt(Tools.map(GetPlayerInGameCount(), requiredPlayers, 6, Settings.digitalControlEndMinPlayers, Settings.digitalControlEndMaxPlayers));
        //amountToCreate = Mathf.RoundToInt(Tools.map(currentSequence, 0, Settings.roundToWin - 1, Settings.digitalControlStart, Settings.digitalControlEnd));
        amountToCreate = Settings.digitalControlAmount[currentSequence];

        Debug.Log("Amount to create. " + amountToCreate);
        Debug.Log("Starting current sequence:" + currentSequence);

        List<string> usedNames = new List<string>();
        usedNames.Clear();
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].ready)
            { //is in Game
                Debug.Log("Creating " + amountToCreate + " controls for player " + i);
                player[i].digitalControls = new List<DigitalControl>();
                //player[i].physicalControls = new List<PhysicalControl>();
                for (int j = 0; j < amountToCreate; j++)
                {
                    int newNameIndex = Random.Range(0, Database.ControlNames.Length);
                    Debug.Log(Database.ControlNames[newNameIndex]);


                    while (usedNames.Contains(Database.ControlNames[newNameIndex]))
                    {
                        Debug.Log("Name is repeating. Finding new");
                        newNameIndex = Random.Range(0, Database.ControlNames.Length);
                        Debug.Log(Database.ControlNames[newNameIndex]);


                    }
                    usedNames.Add(Database.ControlNames[newNameIndex]);

                    //DigitalControl.ControlType[] types = { DigitalControl.ControlType.button, DigitalControl.ControlType.fader, DigitalControl.ControlType.knob, DigitalControl.ControlType.switchtype };

                    //Knob is removed from types, preventing any knob instances
                    DigitalControl.ControlType[] types = { DigitalControl.ControlType.button, DigitalControl.ControlType.fader, DigitalControl.ControlType.switchtype };
                    int randomType = Random.Range(0, types.Length);
                    DigitalControl newControl = new DigitalControl(j, Database.ControlNames[newNameIndex], types[randomType]);
                    player[i].digitalControls.Add(newControl);

                }

                //Create unique names for physical controls
                for (int j = 0; j < player[i].physicalControls.Count; j++)
                {
                    if (player[i].physicalControls[j].lcdPin != "")
                    {
                        int newNameIndex = Random.Range(0, Database.ControlNames.Length);
                        Debug.Log(Database.ControlNames[newNameIndex]);

                        while (usedNames.Contains(Database.ControlNames[newNameIndex]))
                        {
                            Debug.Log("Name is repeating. Finding new");
                            newNameIndex = Random.Range(0, Database.ControlNames.Length);
                            Debug.Log(Database.ControlNames[newNameIndex]);


                        }
                        usedNames.Add(Database.ControlNames[newNameIndex]);

                        player[i].physicalControls[j].name = Database.ControlNames[newNameIndex];
                    }
                }

                DigitalControl[] digitalControlArray = new DigitalControl[player[i].digitalControls.Count];
                string digitalControlString = "";
                for (int j = 0; j < player[i].digitalControls.Count; j++)
                {
                    //digitalControlArray[j] = player[i].digitalControls[j];
                    string newInput = player[i].digitalControls[j].GetJSON();
                    //newInput = newInput.Substring(1, newInput.Length - 2);
                    if (j > 0)
                        digitalControlString += ", " + newInput;
                    else
                        digitalControlString += newInput;

                    //string newControlsJSON = JsonUtility.ToJson(digitalControlArray[0]);
                    //string newControlsJSON = ;
                    Debug.Log(digitalControlString);


                }
                PhysicalControl[] physicalControlArray = new PhysicalControl[player[i].physicalControls.Count];
                string physicalControlString = "";
                for (int j = 0; j < player[i].physicalControls.Count; j++)
                {

                    //digitalControlArray[j] = player[i].digitalControls[j];
                    string newInput = player[i].physicalControls[j].GetJSON();
                    //newInput = newInput.Substring(1, newInput.Length - 2);
                    if (j > 0)
                        physicalControlString += ", " + newInput;
                    else
                        physicalControlString += newInput;

                    //string newControlsJSON = JsonUtility.ToJson(digitalControlArray[0]);
                    //string newControlsJSON = ;
                    Debug.Log(physicalControlString);


                }
                SendUDPMessage("{\"sender\":\"server\", \"id\":" + i + ", \"msg\":\"UpdateControlDevices\", \"physicalControl\":[" + physicalControlString + "], \"digitalControl\":[" + digitalControlString + "]}");

            }




        }
    }

    void SendPhysicalControls(int which)
    {


        for (int i = 0; i < player.Length; i++)
        {
            if (which == -1 || which == i)
            {

                string digitalControlString = "";

                PhysicalControl[] physicalControlArray = new PhysicalControl[player[i].physicalControls.Count];
                string physicalControlString = "";
                for (int j = 0; j < player[i].physicalControls.Count; j++)
                {

                    //digitalControlArray[j] = player[i].digitalControls[j];
                    string newInput = player[i].physicalControls[j].GetJSON();
                    //newInput = newInput.Substring(1, newInput.Length - 2);
                    if (j > 0)
                        physicalControlString += ", " + newInput;
                    else
                        physicalControlString += newInput;

                    //string newControlsJSON = JsonUtility.ToJson(digitalControlArray[0]);
                    //string newControlsJSON = ;
                    Debug.Log(physicalControlString);


                }
                SendUDPMessage("{\"sender\":\"server\", \"id\":" + i + ", \"msg\":\"SetupPhysicalControls\", \"physicalControl\":[" + physicalControlString + "]}");




            }

        }
    }

    void SequenceDuring()
    {
        if (Settings.showDebug && Input.GetKeyDown(KeyCode.F6))
        {
            CorrectInstruction();
        }
        if (blackholeDuringInstructions)
            DuringBlackholeInstructions();
        if (blackholeDuringEffect)
            DuringBlackholeEffect();
    }

    void SendNewInstruction(int whichDevice)
    {
        //Declare variables
        bool isNotDuplicated = true;
        string newInstructionJSON = "";
        int randomPhysicalOrDigital = 0;
        int randomP = 0;
        string instructionName = "";
        float targetValue = 0;
        float instructionTime = 0;
        int whichConfig = 0;
        int whichPin = 0;
        int whichPinOrder = 0;
        DigitalControl digitalControl = new DigitalControl();
        PhysicalControl physicalControl = new PhysicalControl();
        //Create instruction without allowing duplicates;
        while (isNotDuplicated == true)
        {
            randomP = Random.Range(0, player.Length);
            while (!player[randomP].ready || randomP == whichDevice)
            {
                randomP = Random.Range(0, player.Length);
                Debug.Log("Random Player" + randomP);
            }

            randomPhysicalOrDigital = Random.Range(0, 2);
            //randomPhysicalOrDigital = 0;
            Debug.Log("physicalordigital" + randomPhysicalOrDigital);
            Debug.Log(player[randomP].physicalControls.Count);
            if (randomPhysicalOrDigital == 1 && player[randomP].physicalControls.Count > 0)
            {
                Debug.Log("Physical Command");
                //Random Physical Control
                int RandomControl = player[randomP].GetRandomPhysicalControl();
                physicalControl = player[randomP].physicalControls[RandomControl];
                instructionName = physicalControl.name;

                //float[] config = Database.GetConfiguration(control.type);
                float[] config = physicalControl.configuration;
                whichConfig = Random.Range(0, config.Length);
                whichPinOrder = Random.Range(0, physicalControl.pin.Length);
                whichPin = physicalControl.pin[whichPinOrder];
                Debug.Log("whichName" + instructionName);
                Debug.Log("which config" + whichConfig);
                Debug.Log("which pin order" + whichPinOrder);
                Debug.Log("whichPin" + whichPin);

                targetValue = config[whichConfig];


                while (targetValue == player[randomP].physicalControls[RandomControl].value[whichPinOrder])
                {
                    Debug.Log("finding new physical control:" + config.Length);

                    whichConfig = Random.Range(0, config.Length);
                    targetValue = config[whichConfig];
                    if (config.Length <= 1)
                    {
                        break;
                    }
                    //Debug.Log("Rnndom Target " + whichConfig);

                }

                instructionTime = Settings.timePerInstruction - (Settings.timePerInstructionRoundDecrease * currentSequence);


            }
            else
            {
                Debug.Log("Digital Command");

                //Random Digital Control
                int RandomControl = player[randomP].GetRandomDigitalControl();
                Debug.Log("randomControl" + RandomControl);

                digitalControl = player[randomP].digitalControls[RandomControl];
                instructionName = digitalControl.name;
                //float[] config = Database.GetConfiguration(control.type);
                float[] config = digitalControl.configuration;
                whichConfig = Random.Range(0, config.Length);
                targetValue = config[whichConfig];


                while (targetValue == player[randomP].digitalControls[RandomControl].value)
                {
                    whichConfig = Random.Range(0, config.Length);
                    targetValue = config[whichConfig];
                    //Debug.Log("Random Target " + whichConfig);
                    Debug.Log("Available Configuration Lengths" + config.Length);
                    if (config.Length <= 1)
                    {
                        break;
                    }
                }
                instructionTime = Settings.timePerInstruction - (Settings.timePerInstructionRoundDecrease * currentSequence);
                Debug.Log("new instructions created");



            }
            bool isDuplicated = false;
            //Check if there are duplicates
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].ready) //player must be in game
                {
                    if (player[i].currentInstructions != null)
                    { // if player has instruction
                        if (player[i].currentInstructions.active) //must be comparing with an active instruction
                        {
                            if (player[i].currentInstructions.instructionName == instructionName && player[i].currentInstructions.value == targetValue && player[i].currentInstructions.pin == whichPin && player[i].currentInstructions.pinOrder == whichPinOrder)
                            {
                                Debug.Log("New instruction already exists. Creating new");
                                isDuplicated = true;

                            }
                        }
                    }
                }

            }
            if (isDuplicated == false)
            {
                isNotDuplicated = false;

            }
        }

        //Send instruction
        if (randomPhysicalOrDigital == 1 && player[randomP].physicalControls.Count > 0)
        {
            //Physical

            Debug.Log(newInstructionJSON);
            Debug.Log("Set " + instructionName + " to " + targetValue.ToString() + " in " + instructionTime.ToString() + " seconds");

            player[whichDevice].currentInstructions = new Instruction(instructionName, whichPin, whichPinOrder, targetValue, instructionTime, physicalControl.type);
            newInstructionJSON = player[whichDevice].currentInstructions.GetJSON();
            SendUDPMessage("{\"sender\":\"server\", \"id\":" + whichDevice + ", \"msg\":\"NewInstruction\", \"instruction\":" + newInstructionJSON + "}");
            Debug.Log("physical instructions sent");

        }
        else
        {
            //Digital
            Debug.Log("Set " + instructionName + " to " + targetValue.ToString() + " in " + instructionTime.ToString() + " seconds");

            player[whichDevice].currentInstructions = new Instruction(instructionName, targetValue, instructionTime, digitalControl.type);
            newInstructionJSON = player[whichDevice].currentInstructions.GetJSON();

            SendUDPMessage("{\"sender\":\"server\", \"id\":" + whichDevice + ", \"msg\":\"NewInstruction\", \"instruction\":" + newInstructionJSON + "}");

            Debug.Log("digital instructions sent");
        }


    }

    void ChangeControlValue(int _device, int _which, float _val)
    {
        player[_device].digitalControls[_which].value = _val;
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].currentInstructions != null)
            {
                if (player[i].currentInstructions.instructionName == player[_device].digitalControls[_which].name && player[i].currentInstructions.active)
                {
                    if (player[i].currentInstructions.value == player[_device].digitalControls[_which].value)
                    {
                        Debug.Log("CORRECT DIGITAL INSTRUCTION. Device who completed:" + _device + ". Device who instructed:" + i + ". Instruction:" + player[i].currentInstructions.GetJSON());

                        player[i].score += 10;
                        player[i].currentInstructions.active = false;
                        CorrectInstruction();
                        if (!sequenceCompletePause)
                        {
                            SendUDPMessage("{\"sender\":\"server\", \"id\":" + i + ", \"msg\":\"CorrectInstruction\"}");
                            SendNewInstruction(i);
                        }

                    }
                }
            }
        }

    }

    void ChangePhysicalControlValue(int _device, int _pin, int _which, float _val)
    {
        Debug.Log("Changing Physical. device " + _device + " pin " + _pin + " which " + _which + " val " + _val);
        player[_device].physicalControls[_which].value[_pin] = _val;
        for (int i = 0; i < player.Length; i++)
        {
            if (blackholeDuringInstructions)
            {
                if (player[i].ready && blackholeInstructionsTimerCurrent > 0)
                {
                    if (player[_device].physicalControls[_which].type == PhysicalControl.PhysicalControlType.joystick && _val == 0)
                    {
                        AddBlackholeInstruction();
                    }
                }
            }
            if (player[i].currentInstructions != null)
            {
                if (player[i].currentInstructions.instructionName == player[_device].physicalControls[_which].name && player[i].currentInstructions.active)
                {
                    if (player[i].currentInstructions.value == player[_device].physicalControls[_which].value[_pin])
                    {
                        Debug.Log("PIN CHANGED:" + _pin);
                        Debug.Log("INSTRUCTIONS PIN:" + player[i].currentInstructions.pin);
                        if (player[i].currentInstructions.pinOrder == _pin)
                        {
                            Debug.Log("CORRECT PHYSICAL INSTRUCTION. Device who completed:" + _device + ". Device who instructed:" + i + ". Instruction:" + player[i].currentInstructions.GetJSON());

                            player[i].score += 10;
                            player[i].currentInstructions.active = false;

                            CorrectInstruction();
                            if (!sequenceCompletePause)
                            {

                                SendUDPMessage("{\"sender\":\"server\", \"id\":" + i + ", \"msg\":\"CorrectInstruction\"}");
                                SendNewInstruction(i);
                            }
                        }
                    }
                }
            }
        }

    }

    void FailInstruction(int _device)
    {
        Debug.Log("Handling Failed Player's health");
        if (globalShipHealth >= 0)
        {
            globalShipHealth -= 1;
            SFX.PlaySFX("sfx/server/ship_damaged.wav");
            if (globalShipHealth < (GetShipMaxHealth() / 5) * 4)
            {
                Music.PlayMusic("sfx/server/ship_damage_1_soundbed.wav");

                damageAudioBedCount = 1;   //set variable to store damage phase - utilized for end-of-black-hole event

                //conditional statement to allow a non-repeated light triggering for this damage phase
                if (!damageFlags[0])
                {
                    if (!blackholeActive)   //pass through if black hole is not active
                    {
                        arduinoServerController.DamageLights(damageAudioBedCount);  //trigger appropriate damage phase lighting
                    }
                    damageFlags[0] = true;  //set for non-repeat triggering
                }
            }

            if (globalShipHealth < (GetShipMaxHealth() / 5) * 3)
            {
                Music.PlayMusic("sfx/server/ship_damage_2_soundbed.wav");

                //Same as above 
                damageAudioBedCount = 2;
                if (!damageFlags[1])
                {
                    if (!blackholeActive)
                    {
                        arduinoServerController.DamageLights(damageAudioBedCount);
                    }
                    damageFlags[1] = true;
                }
            }

            if (globalShipHealth < (GetShipMaxHealth() / 5) * 2)
            {
                Music.PlayMusic("sfx/server/ship_damage_3_soundbed.wav");
                
                //Same as above
                damageAudioBedCount = 3;
                if (!damageFlags[2])
                {
                    if (!blackholeActive)
                    {
                        arduinoServerController.DamageLights(damageAudioBedCount);
                    }         
                    damageFlags[2] = true;
                }
            }

            if (globalShipHealth < (GetShipMaxHealth() / 5) * 1)
            {
                Music.PlayMusic("sfx/server/ship_damage_4_soundbed.wav");
            }
            if (player[_device].shipHealth >= Settings.shipHealthDamageTrigger)
            {
                SendUDPMessage("{\"sender\":\"server\", \"id\":" + _device + ", \"msg\":\"DamageTrigger\"}");

                player[_device].shipHealth = 0;
            }
            else
            {
                player[_device].shipHealth += 1;



            }
            Debug.Log("Creating new Instruction");
            SendNewInstruction(_device);
        }
        else
        {
            EndGame();

        }

    }

    void CorrectInstruction()
    {
        if (sequences[currentSequence].progress < Settings.instructionsToWinRound)
        {
            sequences[currentSequence].progress += 1;
            progressBar.targetScale = new Vector3(1, Tools.map(sequences[currentSequence].progress, 0, Settings.instructionsToWinRound, 0, 1), 1);
            progressShipIcon.targetLocalPosition = new Vector3(0, Tools.map(sequences[currentSequence].progress, 0, Settings.instructionsToWinRound, 0, 1000), 0);
            tunnel.SetSpeed(Tools.map(sequences[currentSequence].progress, 0, Settings.instructionsToWinRound, 0.01f, 0.7f));
            CheckIfBlackhole();

        }
        else
        {
            SequenceComplete();
        }

    }

    bool sequenceCompletePause;
    void SequenceComplete()
    {
        if (currentSequence < sequences.Length - 1)
        {
            arduinoServerController.turnOffBothBlackHoleLights();   //turn off feeds for blackhole warning and instance
            arduinoServerController.turnOffDamageLightFeeds();      //turn off feeds for damage lights

            SFX.PlaySFX("sfx/server/round_end.wav");

            currentSequence += 1;
            UISequenceCompleted.SetActive(false);
        UISequenceTitle.SetActive(false);
        tunnel.EndSequence();
            UISequenceCompleted.SetActive(true);
            UISequenceCompletedNumber.text = (currentSequence).ToString();
            SendUDPMessage("{\"sender\":\"server\", \"id\":" + -1 + ", \"msg\":\"SequenceComplete\"}");
            sequenceCompletePause = true;
            StartCoroutine(SequenceStartAfterTime(6));
        }
        else
        {

            WinGame();
        }

    }

    IEnumerator SequenceStartAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        SequenceStart();
    }



    void WinGame()
    {
        gameResetActive = true;         //disable damage-lighting for EndBlackHole, pertains to resetting the game
        EndBlackhole();
        gameResetActive = false;        //re-enable damage-lighting for EndBlackhole, pertains to resetting the game

        UISequenceTitle.SetActive(false);
        UISequenceCompleted.SetActive(false);
        UISequenceInGame.SetActive(false);
        UIWinText.SetActive(true);
        UIWin.SetActive(true);
        UIWinScore.SetActive(true);
        UIWinSequenceNumber.text = (currentSequence + 1).ToString();

        /*
                int[] orderedScore = new int[player.Length];
                string[] place={"1st","2nd","3rd", "4th","5th","6th"};

                for (int i = 0; i < player.Length; i++)
                {
                    orderedScore[i] = player[i].score;
                }
                System.Array.Sort(orderedScore);
              */

        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].ready)
            {
                UIWinScoreText[i].gameObject.SetActive(true);
                UIWinPlaceText[i].gameObject.SetActive(true);
                UIWinScoreText[i].text = player[i].score.ToString();
                UIWinPlaceText[i].text = (i + 1).ToString();
            }
            else
            {
                UIWinScoreText[i].gameObject.SetActive(false);
                UIWinPlaceText[i].gameObject.SetActive(false);
            }
        }

        arduinoServerController.Win();
        //arduinoServerController.GameWinLights();

        damageAudioBedCount = 0;    //reset damage count for damage phase lighting - might be redundant

        //reset flags for damage phase single triggering - might be redundant
        for (int i = 0; i < damageFlags.Length; i++)
        {
            damageFlags[i] = false;
        }

        //SendAPIMessage("endGame", "{\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"win\"}");
        //SendAPIMessage("From Client: endGame - Response {\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"win\"}");
        //SendAPIMessage("{\"msg\":\"endGame\",\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"win\"}");
        SendUDPMessage("{\"sender\":\"server\", \"id\":" + -1 + ", \"msg\":\"WinGame\"}");
        SFX.PlaySFX("sfx/server/ship_win.wav");
        Music.StopAllMusic();
        //tunnel.gameObject.SetActive(false);
        //tunnelEnd.SetActive(true);
        //tunnel.SetSpeed(0f);
        tunnel.SetSpeed(1);
        tunnel.EndSequence();
        StartCoroutine(TransitionToWinScene());
        phase = 3;
    }

    IEnumerator TransitionToWinScene(){
        yield return new WaitForSeconds(6f);
        UIFlash.SetTrigger("in");
        yield return new WaitForSeconds(0.1f);

        tunnel.gameObject.SetActive(false);
        tunnelEnd.SetActive(true);
    }
    void EndGame()
    {
        gameResetActive = true;         //disable damage-lighting for EndBlackHole, pertains to resetting the game
        EndBlackhole();
        gameResetActive = false;        //re-enable damage-lighting for EndBlackHole, pertains to resetting the game

        UISequenceTitle.SetActive(false);
        UISequenceCompleted.SetActive(false);
        UISequenceInGame.SetActive(false);
        UIGameoverText.SetActive(true);
        UIGameover.SetActive(true);
        UIGameoverScore.SetActive(true);
        UIGameOverSequenceNumber.text = (currentSequence + 1).ToString();


        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].ready)
            {
                UIGameOverScoreText[i].gameObject.SetActive(true);
                UIGameOverPlaceText[i].gameObject.SetActive(true);
                UIGameOverScoreText[i].text = player[i].score.ToString();
                UIGameOverPlaceText[i].text = (i + 1).ToString();
            }
            else
            {
                UIGameOverScoreText[i].gameObject.SetActive(false);
                UIGameOverPlaceText[i].gameObject.SetActive(false);
            }
        }

        arduinoServerController.Lose();
        //arduinoServerController.GameLoseLights();

        damageAudioBedCount = 0;        //reset damage count for damage phase lighting - might be redundant

        //reset flags for damage phase single triggering - might be redundant
        for (int i = 0; i < damageFlags.Length; i++)
        {
            damageFlags[i] = false;
        }

        //SendAPIMessage("endGame", "{\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"lose\"}");
        // SendAPIMessage("From Client: endGame - Response {\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"lose\"}");
        //SendAPIMessage("{\"msg\":\"endGame\",\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"lose\"}");
        SendUDPMessage("{\"sender\":\"server\", \"id\":" + -1 + ", \"msg\":\"EndGame\"}");
        SFX.PlaySFX("sfx/server/ship_destroyed.wav");
        Music.StopAllMusic();

        tunnel.SetSpeed(0f);

        phase = 3;
    }

    void EndGameTimeOut()
    {
        //Lights Addition
        gameResetActive = true;
        EndBlackhole();
        gameResetActive = false;

        UIStartInstructions.SetActive(false);
        UILaunchWhenReady.SetActive(false);
        UILaunchSequence.SetActive(false);
        UISpaceshipPlayers.SetActive(false);
        UISequenceTitle.SetActive(false);
        UISequenceCompleted.SetActive(false);
        UISequenceInGame.SetActive(false);
        UIGameoverText.SetActive(true);
        UIGameover.SetActive(true);
        //UIGameoverScore.SetActive(true);
        UIGameOverSequenceNumber.text = (currentSequence + 1).ToString();

        arduinoServerController.Lose();
        //arduinoServerController.GameLoseLights();

        damageAudioBedCount = 0;        //reset damage count for damage phase lighting - might be redundant

        //reset flags for damage phase single triggering - might be redundant
        for (int i = 0; i < damageFlags.Length; i++)
        {
            damageFlags[i] = false;
        }

        //SendAPIMessage("endGame", "{\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"lose\"}");
        // SendAPIMessage("From Client: endGame - Response {\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"lose\"}");
        //SendAPIMessage("{\"msg\":\"endGame\",\"gameAPid\":" + Settings.APIAppId + ", \"groupID\":" + groupID + ", \"result\":\"lose\"}");
        SendUDPMessage("{\"sender\":\"server\", \"id\":" + -1 + ", \"msg\":\"EndGameTimeOut\"}");
        Music.StopAllMusic();

        tunnel.SetSpeed(0f);

        phase = 3;
    }

    void EndDisplayingScores()
    {

    }
    public void Reset()
    {
        currentSequence = 0;
        phase = 0;
        timeOutSecondsCur = Settings.timeOutSeconds;
        hideAllUI();
        ResetPlayers();
        /*
                for (int i = 0; i < sequences.Length; i++)
                {
                    sequences[i].progress = 0;

                }
                */
        WaitingForPlayers();


    }

    void APICheck()
    {
        /*
        if (Settings.APIaddress != "")
        {
            apiCheckCurrent -= Time.deltaTime;
            if (apiCheckCurrent <= 0)
            {
                StartCoroutine(apiCommunication.GetRequestFromAPI());
                apiCheckCurrent = Settings.APICheckCountdown;
            }
        }*/
    }

    void HeartBeat()
    {
        if (heartBeatTimerCurrent > 0)
        {
            heartBeatTimerCurrent -= Time.deltaTime;
        }
        else
        {
            SendAPIMessage("gameHeartbeat", "{\"gameAPid\":" + Settings.APIAppId + "}");
            //SendAPIMessage("From Client: gameHeartbeat - Response {\"gameAPid\":" + Settings.APIAppId + "}");
            //SendAPIMessage("{\"msg\":\"gameHeartbeat\",\"gameAPid\":" + Settings.APIAppId + "}");
            heartBeatTimerCurrent = Settings.heartBeatTimer;

        }

    }

    void hideAllUI()
    {
        UIStartInstructions.SetActive(false);
        UISpaceshipPlayers.SetActive(false);
        UILaunchWhenReady.SetActive(false);
        UILaunchSequence.SetActive(false);
        UISequenceTitle.SetActive(false);
        UISequenceInGame.SetActive(false);
        UISequenceCompleted.SetActive(false);
        UIGameoverText.SetActive(false);
        UIGameover.SetActive(false);
        UIGameoverScore.SetActive(false);
        UIWinText.SetActive(false);
        UIWin.SetActive(false);
        UIWinScore.SetActive(false);
        UIBlackhole.SetActive(false);


    }

    void UIChangeReadyPlayers()
    {
        if (phase < 2)
        {
            int amountReady = 0;
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].ready == true)
                {
                    amountReady += 1;
                    UISpaceshipPlayersParts[i].targetAlpha = 1;
                }
                else
                {
                    UISpaceshipPlayersParts[i].targetAlpha = 0;

                }
            }
            waitingPlayerCountText.text = amountReady.ToString() + "/" + player.Length.ToString();
            if (amountReady >= requiredPlayers)
            {
                StartLaunchSequence();
            }
            else
            {
                StopLaunchSequence();
            }
        }
    }


    void SendAPIMessage(string _msg)
    {
        /*
        From Server: endGame - Response {"status":"ok"}
From Server: currentGameStatus - Response {"status":"ok"}
From Server: endGame - Response {"status":"ok"}
From Server: gameStarted - Response {"status":"ok"}*/
        if (Settings.APIaddress != "")
        {
            //StartCoroutine(apiCommunication.Post(_msg));
        }

    }


    void SendAPIMessage(string _function, string _msg)
    {
        /*
        From Server: endGame - Response {"status":"ok"}
From Server: currentGameStatus - Response {"status":"ok"}
From Server: endGame - Response {"status":"ok"}
From Server: gameStarted - Response {"status":"ok"}*/
        if (Settings.APIaddress != "")
        {
            apiCommunication.SocketEmit(_function, _msg);
        }

    }

    void ReceiveAPIMessage(string _msg)
    {
        Debug.Log(_msg);
        APIMessage msg = JsonUtility.FromJson<APIMessage>(_msg);
    }
    void SendUDPMessage(string _msg)
    {
        //fmNetworkManager.SendToAll(_msg);
        udpSend.sendString(_msg);
        //communicationManager.SendString(_msg);

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
        if (FMNetworkManager.instance.NetworkType == FMNetworkType.Server)
        {
            ReceiveMessage(_string);
        }
        else
        {
            ReceiveMessage(_string);
        }
    }
    public void ReceiveMessage(string _string)
    {
        //Debug.Log("Message Received");

        if (_string != "")
        {
            UDPMessage msg = JsonUtility.FromJson<UDPMessage>(_string);
            if (msg == null)
            {
                Debug.Log("Received message not in the correct format");
                return;

            }
            if (msg.sender == "client")
            {
                if (phase < 2)
                {
                    if (msg.msg == "setready")
                    {
                        player[msg.id].ready = bool.Parse(msg.val);
                        Debug.Log("Player " + msg.id + " ready:" + player[msg.id].ready.ToString());
                        UIChangeReadyPlayers();
                    }
                }
                if (phase == 2)
                {

                    if (msg.msg == "requestinstruction")
                    {
                        SendNewInstruction(msg.id);
                    }

                    if (msg.msg == "ChangeControlValue")
                    {
                        ChangeControlValue(msg.id, msg.controlID, float.Parse(msg.val));
                    }
                    if (msg.msg == "ChangePhysicalControlValue")
                    {
                        ChangePhysicalControlValue(msg.id, msg.pin, msg.controlID, float.Parse(msg.val));
                    }
                    if (msg.msg == "FailInstruction")
                    {
                        FailInstruction(msg.id);
                    }
                }
                if (msg.msg == "RequestPhysicalControls")
                {
                    SendPhysicalControls(msg.id);
                }
            }
        }
    }

    public string GetGameStatus()
    {
        if (gameStatus == GameStatus.waiting)
        {
            return "waiting";
        }
        else if (gameStatus == GameStatus.in_session)
        {
            return "in_session";
        }
        else
        {
            return "error";
        }

    }

    public int groupID;
    public void SendGameStatusToAPI()
    {
        SendAPIMessage("currentGameStatus", "{\"gameAPid\":" + Settings.APIAppId + ", \"status\":\"" + GetGameStatus() + "\", \"groupID\":" + groupID + "}");
        //SendAPIMessage("From Client: currentGameStatus - Response {\"gameAPid\":" + Settings.APIAppId + ", \"status\":\"" + GetGameStatus() + "\", \"groupID\":" + groupID + "}");
        //SendAPIMessage("{\"msg\":\"currentGameStatus\",\"gameAPid\":" + Settings.APIAppId + ", \"status\":\"" + GetGameStatus() + "\", \"groupID\":" + groupID + "}");

    }

    int GetPlayerInGameCount()
    {
        int count = 0;
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].ready)
            {
                count += 1;
            }
        }
        return count;
    }

    int GetShipMaxHealth()
    {
        //return Settings.shipHealthMax * GetPlayerInGameCount();
        return Settings.shipHealthMax;
    }


}
