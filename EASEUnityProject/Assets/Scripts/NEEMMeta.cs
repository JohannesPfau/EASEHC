using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NEEMMeta
{
    public string id;
    public string created_by;
    public string created_at;
    public string model_version;
    public string description;
    public string[] keywords;
    public string name;
    public NEEMActivity activity;
    public string environment = "Kitchen";
    public string image = "https://i.imgur.com/hSnydEb.png";
    public string agent = "Human";

    [Serializable]
    public class NEEMActivity
    {
        public string name;
        public string url;

        public NEEMActivity(string name)
        {
            this.name = name;
            url = "https://neemgit.informatik.uni-bremen.de/neems/kitchenclash";
        }
    }
}
