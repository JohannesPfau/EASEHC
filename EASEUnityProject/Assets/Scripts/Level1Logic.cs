using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Logic : LevelLogic {

    public GameObject[] tableware;
    public GameObject[] silverware;
    public GameObject tablegroup;

    public DialogEntry currentDialogEntry;

    // Use this for initialization
    void Start ()
    {
        base.Start();

        DialogSys.addDialog("AI", "Willkommen im Konfigurationsmenue ihres neuen und technisch ueberragenden Optomat 9000!", Mood.STARS);
        DialogSys.addDialog("AI", "Mithilfe unseres BCI-Assistenten richten wir auch ihren Optomaten passformgerecht auf Ihre Vorstellungen ein.", Mood.HAPPY, "cmd_choice1");
        DialogSys.display();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();

        if (progress == 1)
        {
            float camY = GameObject.Find("VRCamera").transform.localRotation.eulerAngles.y;
            if((camY > 120 && camY < 180) || (camY < 240 && camY > 180))
                cmd_4();
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
            // simulate: opto-interface action after furniture has arranged
            cmd_3();
        if (Input.GetKeyDown(KeyCode.Keypad1))
            cmd_5();
    }

    public void cmd_choice1()
    {
        DialogSys.showMultipleChoice("Alles klar! Programm starten!", "cmd_1a", "Und dafuer muesst ihr in meinen Kopf?", "cmd_1b", "Und in der Werbung sah es noch nach Plug'n'Play aus...", "cmd_1c");
    }

    public void cmd_1a()
    {
        DialogSys.addDialog("Player", "Alles klar! Programm starten!", "cmd_1_followup");
        DialogSys.display();
    }
    public void cmd_1b()
    {
        DialogSys.addDialog("Player", "Und dafuer muesst ihr in meinen Kopf?");
        DialogSys.addDialog("AI", "Nur durch individualisierte Kundenmodelle können wir die nahtlose Integration in jede Alltagssituation gewährleisten.", Mood.OOPS);
        DialogSys.addDialog("Player", "Na wenn es nicht zu lange dauert...", "cmd_1_followup");
        DialogSys.display();

    }
    public void cmd_1c()
    {
        DialogSys.addDialog("Player", "Und in der Werbung sah es noch nach Plug'n'Play aus...");
        DialogSys.addDialog("AI", "Nach einer einmaligen Nutzermodellkonfiguration wird der Optomat 9000 all Ihre Alltagstaetigkeiten uebernehmen koennen.", Mood.PROUD);
        DialogSys.addDialog("Player", "Na wenn es nicht zu lange dauert...", "cmd_1_followup");
        DialogSys.display();
    }

    public void cmd_1_followup()
    {
        DialogSys.addDialog("AI", "In der ersten Trainingseinheit wird ein Grundlagenszenario repliziert, das sie aus ihrem täglichen Ablauf kennen sollten.", Mood.PROUD);
        DialogSys.addDialog("AI", "Das Abendessen ist fertig zubereitet. Ihre vierkoepfige Familie wartet darauf, dass der Tisch gedeckt ist. Es wird eine schmackhafte Suppe serviert werden.", Mood.SMIRKING, "cmd_choice2");
        DialogSys.display();
    }

    public void cmd_choice2()
    {
        DialogSys.showMultipleChoice("Eigentlich deckt meine Frau den Tisch...", "cmd_2a", "Ich sehe noch nicht mal einen Tisch...", "cmd_2b");
    }

    public void cmd_2a()
    {
        DialogSys.addDialog("Player", "Eigentlich deckt meine Frau den Tisch...");
        DialogSys.addDialog("AI", "Der Optomat 9000 verfuegt über ein uneingeschraenkt emanzipiertes Weltbild. Bitte konfigurieren sie das Tischdecken für 4 Personen, die sich auf ihre schmackhafte Suppe freuen.", Mood.NEUTRAL, "cmd_2_followup");
        DialogSys.display();
    }

    public void cmd_2b()
    {
        DialogSys.addDialog("Player", "Ich sehe noch nicht mal einen Tisch...", "cmd_2_followup");
        DialogSys.display();
    }

    public void cmd_2_followup()
    {
        tablegroup.SetActive(true);
        DialogSys.addDialog("AI", "Achja. Der Tisch.", Mood.OOPS);
        DialogSys.addDialog("Player", "Ist das Dein Ernst? Daran kann doch niemand sitzen!");
        DialogSys.addDialog("AI", "Bitte rekonfigurieren Sie die Moebelkonstellation nach ihrer individuellen Vorliebe.", Mood.PROUD);
        DialogSys.addDialog("AI", "Nach Vollendung dieses Arbeitsschrittes treten Sie bitte zum Opto-Interface vor.", Mood.NEUTRAL);
        DialogSys.display();
    }

    public void cmd_3()
    {
        DialogSys.addDialog("AI", "Hallo!", Mood.HAPPY);
        DialogSys.addDialog("AI", "Ist die personalisierte Rekonfiguration der Möbelsituation abgeschlossen?", Mood.HAPPY, "cmd_choice3");
        DialogSys.display();
    }

    public void cmd_choice3()
    {
        DialogSys.showMultipleChoice("Ja, ich bin fertig.", "cmd_3a", "Ich brauche noch einen Moment.", "cmd_3b");
    }

    public void cmd_3a()
    {
        DialogSys.addDialog("Player", "Ja, ich bin fertig.");
        DialogSys.addDialog("AI", "Prima!", Mood.STARS);
        DialogSys.addDialog("AI", "Arrangieren sie jetzt wie besprochen die Abendbrotsituation für ihre vierköpfige Familie, die sich auf ihre schmackhafte Suppe freut.", Mood.PROUD);
        DialogSys.addDialog("Player", "Hier ist doch nicht einmal Besteck!");
        DialogSys.addDialog("AI", "Diese Welt ist einzig und allein beschränkt auf ihren Vorstellungshorizont.", Mood.MINDBLOWN);
        DialogSys.addDialog("AI", "Sehen Sie sich doch einmal um. Das sollte an \"Besteck\" genügen, nicht wahr?", Mood.SMIRKING, "cmd_addProgress");
        DialogSys.display();
    }

    public void cmd_spawnTableware()
    {
        foreach (GameObject go in tableware)
            go.SetActive(true);
        //    for (int i = 0; i < 8; i++)
        //        Instantiate(go, new Vector3(UnityEngine.Random.Range(-2f, -14f), 6f, -4f), Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));

        foreach (GameObject go in silverware)
            for (int i = 0; i < 8; i++)
                Instantiate(go, new Vector3(UnityEngine.Random.Range(-1f, -14f), 10f, UnityEngine.Random.Range(0f, 1f)), Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f)));
    }

    public void cmd_3b()
    {
        DialogSys.addDialog("Player", "Ich brauche noch einen Moment.");
        DialogSys.addDialog("AI", "Das individuelle Konfigurationsmenü des Optomat 9000 findet direkt in Ihrem Bewusstsein statt. Nehmen Sie sich alle Zeit der Welt.", Mood.SLEEPY);
        DialogSys.display();
    }

    public void cmd_4()
    {
        cmd_spawnTableware();
        cmd_addProgress();
        DialogSys.addDialog("Player", ". . .");
        DialogSys.addDialog("Player", "Wirklich ... Wirklich jetzt?");
        DialogSys.addDialog("AI", "Gibt es ein Problem im Rearrangement des Essbestecks?", Mood.THINKING);
        DialogSys.addDialog("Player", "Ich verstehe nicht, warum das hier alles sein muss. Ich wollte einfach nur einen Roboter, der mir im Haushalt hilft.");
        DialogSys.addDialog("AI", "Der vollständige Einsatz des Optomat 9000 in allen Lebenslagen kann gewährleistet werden, wenn die Konfiguration auf den jeweiligen individuellen Haushalt angepasst werden konnte.", Mood.COOL);
        DialogSys.addDialog("Player", "Ja, ja .. Ich habe schon verstanden.", "cmd_makeBreakable");
        DialogSys.display();
    }

    public void cmd_makeBreakable()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Physical"))
            if (go.GetComponentInChildren<Meshinator>() != null)
                go.GetComponentInChildren<Meshinator>().enabled = true;
    }

    public void cmd_5()
    {
        DialogSys.addDialog("AI", "Hallo!", Mood.HAPPY);
        DialogSys.addDialog("AI", "Ist die personalisierte Rekonfiguration der Bestecksituation abgeschlossen?", Mood.HAPPY, "cmd_choice5");
        DialogSys.display();
    }
    public void cmd_choice5()
    {
        DialogSys.showMultipleChoice("Ja... sind wir jetzt endlich fertig?", "cmd_5a", "Noch nicht ganz.", "cmd_5b");
    }

    public void cmd_5a()
    {
        cmd_addProgress();
        DialogSys.addDialog("Player", "Ja... sind wir jetzt endlich fertig?");
        DialogSys.addDialog("AI", "Prima!", Mood.STARS);
        DialogSys.addDialog("AI", "Die Berechnungen für das Suppenszenario werden soeben in der Optocloud berechnet.", Mood.PROUD);
        DialogSys.addDialog("Player", "Optocloud? Okay. Dann kann ich hier auch erstmal wieder raus, ja?");
        DialogSys.addDialog("AI", "Nach Abschluss der Berechnungen ist die Konfiguration zu 2.3% abgeschlossen.", Mood.OOPS);
        DialogSys.addDialog("AI", "Initiiere parallel verlaufende Trainingseinheit 2.", Mood.WINKING);
        DialogSys.addDialog("Player", "Trainingseinheit 2? Nein. Abbruch.");
        DialogSys.addDialog("AI", "Kompiliere Szenario. Lade Shader Cache. Rendere Lichtinformationen.", Mood.NEUTRAL, "cmd_dark");
        DialogSys.addDialog("Player", "Hallo? Hört mich jemand? Beenden! BEENDEN!","cmd_toLevel2");
        DialogSys.display();
    }

    public void cmd_toLevel2()
    {
        Invoke("cmd_PortToLevel2", 7);
    }
    public void cmd_PortToLevel2()
    {
        Physics.gravity = new Vector3(0, -9.8f, 0);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level2_VR");
    }

    public void onOptoInterface()
    {
        switch(progress)
        {
            case 0:
                cmd_3();
                break;
            case 2:
                cmd_5();
                break;
        }
    }
}
