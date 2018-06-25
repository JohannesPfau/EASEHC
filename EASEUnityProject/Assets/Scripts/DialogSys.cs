using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSys : MonoBehaviour {
    
    public static List<DialogEntry> dialogList;
    public static DialogEntry currentDialog;

    public static void addDialog(DialogEntry de)
    {
        if (dialogList == null)
            dialogList = new List<DialogEntry>();
        dialogList.Add(de);
    }
    public static void addDialog(string character, string text, Mood mood, string finishedCmd)
    {
        addDialog(new DialogEntry(character, text, mood, finishedCmd));
    }
    public static void addDialog(string character, string text, Mood mood)
    {
        addDialog(new DialogEntry(character, text, mood, ""));
    }
    public static void addDialog(string character, string text)
    {
        addDialog(new DialogEntry(character, text, Mood.NULL, ""));
    }
    public static void addDialog(string character, string text, string finishedCmd)
    {
        addDialog(new DialogEntry(character, text, Mood.NULL, finishedCmd));
    }
    public static void display()
    {
        if (dialogList == null || dialogList.Count == 0)
            return;

        //Debug.Log("list: ");
        //foreach (DialogEntry de in dialogList)
        //    Debug.Log(de.text);

        currentDialog = dialogList[0];
        dialogList.RemoveAt(0);

        GameObject.Find(currentDialog.character + "_Text").GetComponent<Text>().text = currentDialog.text;
        if (currentDialog.character.Equals("AI"))
        {
            GameObject.Find("AI_Emoji_VR").GetComponentInChildren<Image>().enabled = true;
            if(currentDialog.mood != Mood.NULL)
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("AI_MOOD"))
                    go.GetComponent<Animator>().SetTrigger(currentDialog.mood.ToString());
        }
    }

    public static void proceed()
    {
        clearText();
        if (currentDialog.finishedCmd.Length > 0)
        {
            clearText();
            bool dialogRemaining = false;
            if (dialogList.Count > 0)
                dialogRemaining = true;

            GameObject.Find("LevelLogic").SendMessage(currentDialog.finishedCmd);
            currentDialog.finishedCmd = "";
            if (dialogRemaining)
                display();
        }
        else
            display();
    }

    public static void clearText()
    {
        GameObject.Find("AI_Text").GetComponent<Text>().text = "";
        GameObject.Find("Player_Text").GetComponent<Text>().text = "";
        GameObject.Find("Player_Text0").GetComponent<Text>().text = "";
        GameObject.Find("Player_Text1").GetComponent<Text>().text = "";
        GameObject.Find("Player_Text2").GetComponent<Text>().text = "";
        GameObject.Find("Daughter_Text").GetComponent<Text>().text = "";
        GameObject.Find("AI_Emoji_VR").GetComponentInChildren<Image>().enabled = false;
    }

    static int selected = -1;
    static int selectableOptions = 3;
    static bool _isSelecting;
    static string cmd0;
    static string cmd1;
    static string cmd2;

    public static void showMultipleChoice(string option0, string cmd0, string option1, string cmd1, string option2, string cmd2)
    {
        clearText();
        selectMultipleChoice(0); 
        _isSelecting = true;
        selectableOptions = 3;
        GameObject.Find("Player_Text0").GetComponent<Text>().text = option0;
        GameObject.Find("Player_Text1").GetComponent<Text>().text = option1;
        GameObject.Find("Player_Text2").GetComponent<Text>().text = option2;
        DialogSys.cmd0 = cmd0;
        DialogSys.cmd1 = cmd1;
        DialogSys.cmd2 = cmd2;
    }

    public static void showMultipleChoice(string option0, string cmd0, string option1, string cmd1)
    {
        clearText();
        selectMultipleChoice(0);
        _isSelecting = true;
        selectableOptions = 2;
        GameObject.Find("Player_Text0").GetComponent<Text>().text = option0;
        GameObject.Find("Player_Text1").GetComponent<Text>().text = option1;
        DialogSys.cmd0 = cmd0;
        DialogSys.cmd1 = cmd1;
    }

    public static void selectMultipleChoice(int optionId)
    {
        selected = optionId;
        GameObject.Find("Player_Text0").GetComponent<Text>().color = new Color(150f / 255f, 150f / 255f, 150f / 255f);
        GameObject.Find("Player_Text1").GetComponent<Text>().color = new Color(150f / 255f, 150f / 255f, 150f / 255f);
        GameObject.Find("Player_Text2").GetComponent<Text>().color = new Color(150f / 255f, 150f / 255f, 150f / 255f);
        if (selected >= 0)
            GameObject.Find("Player_Text" + optionId).GetComponent<Text>().color = Color.white;
    }

    public static void moveSelectedDown()
    {
        selectMultipleChoice((selected + 1) % selectableOptions);
    }
    public static void moveSelectedUp()
    {
        selectMultipleChoice((selected + selectableOptions - 1) % selectableOptions);
    }

    public static bool isSelecting()
    {
        return _isSelecting;
    }

    public static void chooseMultipleChoice()
    {
        clearText();
        switch (selected)
        {
            case 0:
                if (cmd0.Length > 0)
                    GameObject.Find("LevelLogic").SendMessage(cmd0);
                break;
            case 1:
                if (cmd1.Length > 0)
                    GameObject.Find("LevelLogic").SendMessage(cmd1);
                break;
            case 2:
                if (cmd2.Length > 0)
                    GameObject.Find("LevelLogic").SendMessage(cmd2);
                break;
        }

        selectMultipleChoice(-1);
        _isSelecting = false;
        cmd0 = "";
        cmd1 = "";
        cmd2 = "";
    }

    public static void setMood(Mood mood)
    {

    }

}
