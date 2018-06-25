using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogEntry {

    public string character;
    public string text;
    public Mood mood;
    public string finishedCmd;


    public DialogEntry(string character, string text, Mood mood, string finishedCmd)
    {
        this.character = character;
        this.text = text;
        this.mood = mood;
        this.finishedCmd = finishedCmd;
    }  
    
}
