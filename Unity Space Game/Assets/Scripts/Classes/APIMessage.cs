using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

    public class APIGroupData
    {
        public int rfid_id { get; set; }
        public string rfid { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public int accesslevel { get; set; }
        public int eventid { get; set; }
        public string lastupdate { get; set; }
        public string additionaldata { get; set; }
        public object prize { get; set; }
        public int prizevended { get; set; }
        public object preregid { get; set; }
        public string email { get; set; }
        public int registered { get; set; }
        public string phone { get; set; }
        public string ride { get; set; }
        public int rfidGroupId { get; set; }
    }

    public class APIMessage
    {
        public int apid { get; set; }
        public List<APIGroupData> groupData { get; set; }
        public int players { get; set; }
        public string groupID { get; set; }
        public int experienceid { get; set; }
        public int gameApid { get; set; }
        public string room { get; set; }
    }
