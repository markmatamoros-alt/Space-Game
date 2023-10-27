using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsPlayerCount
{

    public float timePerInstruction;
    public float timePerInstructionRoundDecrease;

    public int shipHealthMax;

    public int shipHealthDamageTrigger;
    public int instructionsToWinRound;
    public int amountOfPauseBetweenBlackhole;
    public int chanceForBlackholePercent;

    public int chanceForBlackholeIncreaseByRound;
    public int blackholeInputNeeded;
    public float timeToAvoidBlackHole;
    public float timeBlackHoleEffect;


    public int[] digitalControlAmount;

}
