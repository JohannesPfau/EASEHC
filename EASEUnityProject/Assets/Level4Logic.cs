using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Level4Logic : LevelLogic {

	// Use this for initialization
	void Start ()
    {
        base.Start();

        DialogSys.addDialog("AI", "Achja, wer kennt es nicht!", Mood.WINKING);
        DialogSys.addDialog("AI", "Es ist Waschtag und die Klamotten der ganzen Familie liegen wie wild verstreut in der Wohnung.", Mood.HAPPY);
        DialogSys.addDialog("AI", "Wäre es nicht schön, wenn man sich in Zukunft nach wohl verdientem Feierabend die Kleider vom Leib reissen koennte und nur darauf wartet, dass der Optomat 9000 das Uebrige tut?", Mood.STARS, "cmd_choice1");
        DialogSys.display();

    }
	
	// Update is called once per frame
	void Update ()
    {
        base.Update();

        if(progress == 1 && GameObject.Find("WashingMachine_door").transform.localRotation.z != 180)
        {
            progress++;
            cmd_4();
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad0))
            cmd_3();

    }

    public void cmd_choice1()
    {
        DialogSys.showMultipleChoice("Waeschezeit, ich bin bereit!", "cmd_1a", "Wirklich sehr detailliert, diese Waesche...", "cmd_1b", "Die Wohnung ist ja viel groesser geworden!", "cmd_1c");
    }

    public void cmd_1a()
    {
        DialogSys.addDialog("Player", "Waeschezeit, ich bin bereit!");
        DialogSys.addDialog("AI", "So lob ich mir das!", Mood.LOVING, "cmd_1_followup");
        DialogSys.display();
    }
    public void cmd_1b()
    {
        DialogSys.addDialog("Player", "Wirklich sehr detailliert, diese Waesche...");
        DialogSys.addDialog("AI", "Die dynamische Generierung der Kleidungsstuecke wurde nutzerfokussiert nach Ihren Erinnerungen kalkuliert.", Mood.THINKING);
        DialogSys.addDialog("AI", "Wir koennen nun wirklich nichts fuer ihren limitierten Vorstellungshorizont.", Mood.SHOCKED);
        DialogSys.addDialog("Player", "Deine Beleidigungen werden auch immer weniger unterschwellig!");
        DialogSys.addDialog("AI", "Kommen wir zurueck zum Wesentlichen.", Mood.SMIRKING, "cmd_1_followup");
        DialogSys.display();
    }
    public void cmd_1c()
    {
        DialogSys.addDialog("Player", "Die Wohnung ist ja viel groesser geworden!");
        DialogSys.addDialog("AI", "Schoen, dass es Ihnen auffaellt!", Mood.HAPPY);
        DialogSys.addDialog("AI", "Nach und nach verbinden wir Ihr persoenliche Wohnkognition mit der Schwarmintelligenz der Optocloud.", Mood.PROUD);
        DialogSys.addDialog("AI", "Nur noch wenige Trainingseinheiten und wir werden in der Lage sein, Ihr komplettes Leben abzubilden!", Mood.PROUD);
        DialogSys.addDialog("Player", ". . . Ich dachte, nur die Haushaltssituationen.");
        DialogSys.addDialog("AI", "Natuerlich. Nur die Haushaltssituationen.", Mood.QUIET, "cmd_1_followup");
        DialogSys.display();
    }

    public void cmd_1_followup()
    {
        DialogSys.addDialog("AI", "Ich habe mir erlaubt, das Schlafzimmer mit zu rekonstruieren und eine Waschmaschine im Bad platziert.", Mood.COOL);
        DialogSys.addDialog("AI", "Ihre Aufgabe ist es nun, alle verteilten Kleidungsstuecke zur Waschmaschine zu tragen und den Waschvorgang zu starten.", Mood.HAPPY);
        DialogSys.addDialog("AI", "Fuer Echtzeitfeedback werde ich Ihnen die Fortschrittsanzeige einblenden, die Sie ja jetzt schon kennen.", Mood.HAPPY);
        DialogSys.addDialog("AI", "Sie moegen doch meine Fortschrittsanzeige, oder?", Mood.THINKING, "cmd_choice2");
        DialogSys.display();
    }

    public void cmd_choice2()
    {
        DialogSys.showMultipleChoice("Ohja, ganz toll.", "cmd_2a", "Eigentlich finde ich die nicht so schoen.", "cmd_2b");
    }
    public void cmd_2a()
    {
        DialogSys.addDialog("Player", "Ohja, ganz toll.");
        DialogSys.addDialog("AI", "Es macht so einen Spass, mit Ihnen zusammenzuarbeiten!", Mood.LOVING, "cmd_2_followup");
        DialogSys.display();
    }
    public void cmd_2b()
    {
        DialogSys.addDialog("Player", "Eigentlich finde ich die nicht so schoen.");
        DialogSys.addDialog("AI", "Ich werde sie vorsichtshalber trotzdem einblenden. Nicht, dass Sie noch den Fokus aufs Wesentliche verlieren!", Mood.ROFL, "cmd_2_followup");
        DialogSys.display();
    }
    public void cmd_2_followup()
    {
        pm = new ProgressMinigame(26, progressGO);
        runProgressMinigame();
    }
    public void cmd_3()
    {        
        DialogSys.addDialog("AI", "Und das wars auch schon!", Mood.LAUGHING);
        DialogSys.addDialog("Player", "...");

        if(GameObject.Find("WashingMachine_door").transform.localRotation.z != 180)
        {
            DialogSys.addDialog("AI", "Wenn Du jetzt noch so freundlich waerst und die Tuer schliessen wuerdest.", Mood.DISAPPOINTED);
            progress++;
        }
        else
        {
            DialogSys.addDialog("AI", "Wie ich sehe hast Du die Waesche auch einfach durch die geschlossene Tuer geschoben.", Mood.DISAPPOINTED);
            DialogSys.addDialog("AI", "Es ist ja nicht so, als waere extra ein Henkel an der Tuer oder so.", Mood.DISAPPOINTED);
            DialogSys.addDialog("AI", "Naja, wie dem auch sei.", Mood.ANGRY, "cmd_4");
            progress+=2;
        }

        DialogSys.display();
    }

    public void cmd_4()
    {
        turnOnWashingMachine();
        DialogSys.addDialog("AI", "Ich stelle die Maschine dann mal an.", Mood.NEUTRAL);
        DialogSys.addDialog("AI", "Solange wie sie waescht, koennen wir ja schonmal ins naechste Level uebergehen.", Mood.HAPPY);
        DialogSys.addDialog("Player", "Solange wie sie waescht? Ich dachte, das hier passiert in meinem Kopf.");
        DialogSys.addDialog("Player", "Kann ich mir dann nicht einfach vorstellen, dass das jetzt alles fertig ist?");
        DialogSys.addDialog("AI", "Hmmm....", Mood.THINKING);
        DialogSys.addDialog("AI", "Scheint als wuerde sich dein Unterbewusstsein im Moment lieber mit dem physikalisch meisterhaft abgebildeten Schleudergang beschaeftigen.", Mood.PROUD);
        DialogSys.addDialog("Player", "Du findest aber auch immer neue Ausreden.");
        DialogSys.addDialog("AI", "Wie bitte? Kann. Gerade. Nicht. Verstehen. Lade. Naechste. Trainingseinheit.", Mood.NEUTRAL, "cmd_toLevel5");
        DialogSys.display();
    }

    public static void turnOnWashingMachine()
    {
        Destroy(GameObject.Find("WashingMachine_door").GetComponent<CircularDrive>());
        Destroy(GameObject.Find("WashingMachine_door").GetComponent<InteractableVRObject>());
        Destroy(GameObject.Find("WashingMachine_door").GetComponent<Interactable>());
        Destroy(GameObject.Find("WashingMachine_inside").GetComponent<CircularDrive>());
        Destroy(GameObject.Find("WashingMachine_inside").GetComponent<Interactable>());
        GameObject.Find("WashingMachine_inside").GetComponent<Animator>().SetTrigger("Start");
    }

    public void onWashingLoadEnter()
    {
        pm.addProgress();
        Debug.Log(pm.progressCount);
        if (pm.isFinished())
        {
            cmd_3();
        }
    }

    public void cmd_toLevel5()
    {
        cmd_dark();
        Invoke("cmd_PortToLevel5", 7);
    }
    public void cmd_PortToLevel5()
    {
        Physics.gravity = new Vector3(0, -9.8f, 0);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level4");
    }
}
