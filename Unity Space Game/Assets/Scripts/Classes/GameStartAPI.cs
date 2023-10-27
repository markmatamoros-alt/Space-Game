using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStartAPI
{
    public int apid;
    public int players;
    public int groupID;
    public int experienceid;
    public GroupData groupData;
    public GameStartAPI()
    {

    }
}

[System.Serializable]

public class GroupData
{
    public int rfid_id;
    public string rfid;
    public string fname;
    public string lname;
    public int accesslevel;
    public int eventid;
    public string lastupdate;
    public string additionaldata;
    public string prize;
    public int prizevended;
    public string preregid;
    public string email;
    public int registered;
    public string phone;
    public string ride;
    public int rfidGroupId;

    public GroupData()
    {

    }
}
