using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Logic : LevelLogic {
    public GameObject broom;
    public GameObject plunger;
    public GameObject waterGround;
    int plungeProgress;

    // Use this for initialization
    void Start ()
    {
        base.Start();

        plungeProgress = 0;
        DialogSys.addDialog("AI", "Ach Du meine Güte!", Mood.SPOOKED);
        DialogSys.addDialog("AI", "Was ist denn hier passiert?", Mood.SHOCKED);
        DialogSys.addDialog("AI", "Sieht so aus, als hätte da jemand vergessen, nach dem Abwasch den Wasserhahn zuzudrehen, nicht wahr?", Mood.DISAPPOINTED, "cmd_choice1");
        DialogSys.display();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();

        if(progress == 0 && GameObject.Find("Player").transform.localPosition.x > 4)
        {
            cmd_addProgress();
            cmd_2();
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
            cmd_2();
        if (Input.GetKeyDown(KeyCode.Keypad1))
            cmd_3();
        if (Input.GetKeyDown(KeyCode.Keypad2))
            cmd_4();
        if(Input.GetKey(KeyCode.Space))
        {
            onBroomed();
        }
    }

    public void cmd_choice1()
    {
        DialogSys.showMultipleChoice("Was? Ich habe den Wasserhahn nichtmal aufgedreht!", "cmd_1a", "Igitt! In was stehe ich denn hier?", "cmd_1b", "Sag mal, willst Du mich eigentlich quälen!?", "cmd_1c");
    }

    public void cmd_1a()
    {
        DialogSys.addDialog("Player", "Was? Ich habe den Wasserhahn nichtmal aufgedreht!");
        DialogSys.addDialog("AI", "Ups! Dann muss es wohl etwas anderes gewesen sein.", Mood.OOPS, "cmd_1_followup");
        DialogSys.display();
    }
    public void cmd_1b()
    {
        DialogSys.addDialog("Player", "Igitt! In was stehe ich denn hier?");
        DialogSys.addDialog("AI", "Dihydrogenmonoxid, aller Wahrscheinlichkeit nach!", Mood.OOPS);
        DialogSys.addDialog("AI", "Aber keine Sorge. Es mag sich real anfühlen, aber ich kann Ihnen versichern, dass sich das alles nur in Ihrem Kopf abspielt.", Mood.OOPS);
        DialogSys.addDialog("Player", "Ich will aber auch in meinem Kopf nicht in einem gefluteten Haus stehen. Bitte... was soll ich denn dagegen machen?");
        DialogSys.addDialog("AI", "Die buchstäbliche \"Quelle\" dieses Malheurs müsste in diesem begrenzten Szenario relativ leicht zu lokalisieren sein.", Mood.THINKING, "cmd_1_followup");
        DialogSys.display();
    }
    public void cmd_1c()
    {
        DialogSys.addDialog("Player", "Sag mal, willst Du mich eigentlich quälen!?");
        DialogSys.addDialog("AI", "Nichts läge mir ferner.", Mood.OOPS);
        DialogSys.addDialog("AI", "Allerdings wimmelt es im Alltag von ungeplanten Situationen, die der Optomat 9000 für Sie und Ihre Familie bewältigen möchte!", Mood.STARS);
        DialogSys.addDialog("Player", "Ich hoffe, ich muss jetzt nicht jede Eventualität simulieren...");
        DialogSys.addDialog("AI", "Keine Sorge! Ihre Performance wird mit den Daten von tausenden von anderen Kunden in der Optocloud vereinigt und individualisiert.", Mood.COOL);
        DialogSys.addDialog("AI", "Kümmern wir uns also erst einmal um dieses Szenario.", Mood.COOL, "cmd_1_followup");
        DialogSys.display();
    }

    public void cmd_1_followup()
    {
        DialogSys.addDialog("AI", "Sieh Dich doch mal um, ob Du den Fehler identifizieren kannst.", Mood.OOPS);
        DialogSys.addDialog("Player", "Ich mach ja schon...");
        DialogSys.display();
    }

    public void cmd_2()
    {
        DialogSys.addDialog("AI", "Ach! Der Klassiker.", Mood.OOPS);
        DialogSys.addDialog("AI", "Eine übergelaufene Toilette.", Mood.OOPS);
        DialogSys.addDialog("AI", "Na dann lass mal sehen, ob Du dagegen etwas unternehmen kannst.", Mood.OOPS, "cmd_spawn_plunger");
        DialogSys.display();
    }
    public void cmd_spawn_plunger()
    {
        cmd_addProgress();
        plunger.SetActive(true);
    }

    public void cmd_3()
    {
        DialogSys.addDialog("AI", "Na bitte!", Mood.OOPS);
        DialogSys.addDialog("AI", "An Dir ist wohl oder übel ein Klempner verloren gegangen!", Mood.OOPS);
        DialogSys.addDialog("Player", "Können wir dann ins nächste Level? Ich will zurück zu meiner Familie.");
        DialogSys.addDialog("AI", "Na, na, na. Nicht so hastig.", Mood.OOPS);
        DialogSys.addDialog("AI", "Schließlich steht das komplette Haus noch unter Wasser.", Mood.OOPS);
        DialogSys.addDialog("AI", "Würdest Du vom Optomaten 9000 nicht erwarten, dass er diese Unstimmigkeit auch bereinigt?", Mood.OOPS);
        DialogSys.addDialog("Player", ". . . *seufz*", "cmd_spawn_broom");
        DialogSys.display();
    }
    public void cmd_spawn_broom()
    {
        cmd_addProgress();
        broom.SetActive(true);
        pm = new ProgressMinigame(749, progressGO);
        runProgressMinigame();
    }

    public void cmd_4()
    {
        DialogSys.addDialog("AI", "Und wieder eine erfolgreiche Lehrstunde für den Optomat 9000!", Mood.STARS);
        DialogSys.addDialog("AI", "Die Optocorp möchte sich an dieser Stelle bei jedem einzelnen Kunden bedanken.", Mood.PROUD);
        DialogSys.addDialog("AI", "Nur durch Ihre Zusammenarbeit können wir ein reibungsfreies Haushaltsunterstützungsprogramm rund um den Globus anbieten.", Mood.PROUD);
        DialogSys.addDialog("AI", "Wenn Sie interessiert an Zusatz- und weiteren Produkten von Octocorp sind, sagen sie jetzt bitte \"Ja.\"", Mood.PROUD);
        DialogSys.addDialog("Player", ". . . *schnauf*", "cmd_choice5");
        DialogSys.display();
    }

    public void cmd_choice5()
    {
        DialogSys.showMultipleChoice("Ehrlich ... ich will nur hier raus.", "cmd_5a", "Keine Werbung bitte. Schick mich ins nächste Level.", "cmd_5b", "Ja.", "cmd_5c");
    }
    public void cmd_5a()
    {
        DialogSys.addDialog("Player", "Ehrlich ... ich will nur hier raus.");
        DialogSys.addDialog("AI", "Ein frühzeitiger Abbruch der Trainingseinheiten kann zu unerwünschtem Verhalten und unvorhersehbaren Klassifikationen ihres Optomats führen.", Mood.DISAPPOINTED);
        DialogSys.addDialog("AI", "Um dieses Sicherheitsrisiko auszuschließen, komplettieren Sie bitte die restlichen Szenarien.", Mood.DISAPPOINTED, "cmd_5_followup");
        DialogSys.display();
    }
    public void cmd_5b()
    {
        DialogSys.addDialog("Player", "Keine Werbung bitte. Schick mich ins nächste Level.");
        DialogSys.addDialog("AI", "Alles klar! Initiiere Übergang in anschließende Trainingseinheit.", Mood.HAPPY, "cmd_5_followup");
        DialogSys.display();
    }
    public void cmd_5c()
    {

        DialogSys.addDialog("AI", "Erweitern Sie jetzt Ihr übliches Haushaltspaket mit \"HomeSmartHome\". Mit zusätzlichen Überwachungs-, Steuer-, Regel- und Optimierungseinrichtungen kontrolliert Ihr Optomat in Zukunft Heizungen, Schlösser, Lichter, Klimaanlagen, Jalousien, Alarmanlagen und vieles mehr!", Mood.STARS);
        DialogSys.addDialog("AI", "Oder wie wäre es mit dem OutOfTheDoor-Programm? Heckenschneiden, Rasenmähen, Blumengießen, Unkraut jäten und all die anderen lästigen Tätigkeiten - Überlassen Sie sie dem freundlichen Robo-Gärtner von Optocorp!", Mood.ROBOT);
        DialogSys.addDialog("AI", "Wenn Sie innerhalb der nächsten 4 Trainingseinheiten bestellen, erhalten sie einen OptoDog gratis dazu.", Mood.LOVING);
        DialogSys.addDialog("AI", "OptoDog - Kinderfreundlich, stubenrein, allergikergeeignet.", Mood.PROUD, "cmd_5_followup");
        DialogSys.display();
    }

    public void cmd_5_followup()
    {
        DialogSys.addDialog("AI", "Informationen über Zusatz- und weitere Produkte von Octocorp wurden Ihnen an die verknüpfte E-Mail-Adresse gesandt.", Mood.PROUD);
        DialogSys.addDialog("AI", "Sie können den nun automatisch abonnierten Newsletter natürlich jederzeit abbestellen.", Mood.WINKING);
        DialogSys.addDialog("Player", ". . . Danke.");
        DialogSys.addDialog("AI", "Aber nun auf ins nächste Level!", Mood.HAPPY);
        DialogSys.addDialog("AI", "Leere Trainingseinheit-Buffer. Reinitialisiere Szenario auf Cloudtemplate.", Mood.NEUTRAL, "cmd_toLevel4");
        DialogSys.display();
    }

    public void cmd_toLevel4()
    {
        cmd_dark();
        waterGround.GetComponent<Animator>().SetTrigger("Start");
        Invoke("cmd_PortToLevel4", 7);
    }
    public void cmd_PortToLevel4()
    {
        Physics.gravity = new Vector3(0, -9.8f, 0);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level4");
    }

    public void onPlunge()
    {
        if(progress == 2)
        {
            GameObject.Find("Plunge" + plungeProgress).SetActive(false);
            plungeProgress++;
            if(plungeProgress > 8)
            {
                cmd_addProgress();
                cmd_3();
            }
        }
    }

    int broomed = 0;
    public void onBroomed()
    {
        pm.addProgress();
        //broomed++;
        Debug.Log(pm.progressCount);
        if(pm.isFinished())
        {
            cmd_4();
        }
    }
}
