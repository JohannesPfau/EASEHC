using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Logic : LevelLogic {

    public GameObject lamps;

	// Use this for initialization
	void Start ()
    {
        base.Start();

        DialogSys.addDialog("AI", "Willkommen in Trainingseinheit Nummer 2!", Mood.STARS);
        DialogSys.addDialog("AI", "Mithilfe der von Ihnen übermittelten Daten in Vereinigung mit der Optocloud haben wir eine Post-Abendbrötliche Athmosphäre computieren können.", Mood.SMIRKING, "");
        DialogSys.addDialog("AI", "Wie Sie sehen wird diese Welt von Schritt zu Schritt detailreicher und individueller.", Mood.WINKING, "");
        DialogSys.addDialog("AI", "Das von Ihnen ausgewählte Opto-Programm beinhaltet außerdem den anschließenden manuellen Geschirrsäuberungsvorgang, umgangssprachlich auch den \"Abwasch\".", Mood.HAPPY, "cmd_choice1");
        DialogSys.display();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Keypad0))
            cmd_2();
    }

    public void cmd_choice1()
    {
        DialogSys.showMultipleChoice("Ich soll jetzt abwaschen? Dafür habe ich Dich doch gekauft!", "cmd_1a", "Also so richtig detailreich sieht das hier ja noch nicht aus.", "cmd_1b", "Wann sind wir denn endlich fertig?", "cmd_1c");
    }

    public void cmd_1a()
    {
        DialogSys.addDialog("Player", "Ich soll jetzt abwaschen? Dafür habe ich Dich doch gekauft!", "");
        DialogSys.addDialog("AI", "Ich wiederhole. Nach einer einmaligen Nutzermodellkonfiguration wird der Optomat 9000 all Ihre Alltagstaetigkeiten uebernehmen koennen.", Mood.NEUTRAL, "cmd_1_followup");
    }
    public void cmd_1b()
    {
        DialogSys.addDialog("Player", "Also so richtig detailreich sieht das hier ja noch nicht aus.", "");
        DialogSys.addDialog("AI", "Entsprechend Ihrem Fortschritt in den einzelnen Trainingseinheiten wird sich das Umgebungsmodell mehr und mehr auf Ihr eigenes Zuhause einstellen.", Mood.THINKING);
        DialogSys.addDialog("Player", "Ich meine ja nur. Ich wohne ja nicht in einem sterilen Raum. Ein bisschen Deko hier und da wäre vielleicht nicht verkehrt.", "");
        DialogSys.addDialog("AI", "In Ordnung. Als Vorgeschmack auf die Detailtiefe präsentiere ich Ihnen zwei formschöne Lampen außer der Reihe.", Mood.STARS, "cmd_1bb");
        DialogSys.display();
    }
    public void cmd_1bb()
    {
        lamps.SetActive(true);
        DialogSys.addDialog("AI", "Ich möchte Sie darauf hinweisen, trotz der gestiegenen Umgebungskomplexität nicht von der Trainingseinheit abzuweichen.", Mood.ASHAMED);
        DialogSys.addDialog("AI", "Verlieren Sie sich bitte nicht im Detail.", Mood.NEUTRAL, "cmd_1_followup");
    }
    public void cmd_1c()
    {
        DialogSys.addDialog("Player", "Wann sind wir denn endlich fertig?", "");
        DialogSys.addDialog("AI", "Nach Aufzeichnung und Inkorporation aller von Ihnen ausgewählten Opto-Programme wird das Trainingsprogramm abgeschlossen sein.", Mood.THINKING);
        DialogSys.addDialog("Player", "Ich habe aber ziemlich viele...");
        DialogSys.addDialog("Player", "*seufz*", "cmd_1_followup");
        DialogSys.display();
    }

    public void cmd_1_followup()
    {
        DialogSys.addDialog("Player", "Das heißt, ich muss mich jetzt einmal um den kompletten Haushalt kümmern und danach nie wieder?");
        DialogSys.addDialog("AI", "Korrekt. Danach nie wieder.", Mood.WINKING);
        DialogSys.addDialog("AI", "Nie. Wieder.", Mood.OOPS);
        DialogSys.addDialog("Player", "Ich mache ja schon den Abwasch.");
        DialogSys.addDialog("AI", "Treten sie wie gehabt nach Vollendung der Aufgaben zum Opto-Interface vor.", Mood.NEUTRAL);
        DialogSys.display();
    }

    public void cmd_2()
    {
        DialogSys.addDialog("AI", "Hallo!", Mood.HAPPY);
        DialogSys.addDialog("AI", "Konnten sie sämtliche Geschirrutensilien zu Ihrer persönlichen Zufriedenheit reinigen?", Mood.HAPPY, "cmd_choice2");
        DialogSys.display();
    }

    public void cmd_choice2()
    {
        DialogSys.showMultipleChoice("Ja, es ist alles sauber.", "cmd_2a", "Ich bin mir nicht sicher.", "cmd_2b");
    }

    public void cmd_2a()
    {
        DialogSys.addDialog("Player", "Ja, es ist alles sauber.");
        DialogSys.addDialog("AI", "Besteck sowie Schüsselware?", Mood.PROUD, "cmd_choice3");
        DialogSys.display();
    }
    public void cmd_2b()
    {
        DialogSys.addDialog("Player", "Ich bin mir nicht sicher.");
        DialogSys.addDialog("AI", "Dann gehen Sie bitte noch einmal auf Nummer sicher. Wir haben alle Zeit der Welt.", Mood.SLEEPY);
        DialogSys.display();
    }

    public void cmd_choice3()
    {
        DialogSys.showMultipleChoice("Ja, wirklich alles.", "cmd_3a", "Ich bin mir nicht sicher.", "cmd_2b");
    }

    public void cmd_3a()
    {
        DialogSys.addDialog("Player", "Ja, wirklich alles.");
        DialogSys.addDialog("AI", "Die reibungslose Funktionsweise in allen Alltagssituationen kann nur gewährleistet werden, wenn die initialen Trainingseinheiten realitätsnah abgebildet werden.", Mood.COOL);
        DialogSys.addDialog("AI", "Unser höchstpriorisiertes Anliegen ist also die konsistente Langzeit-Zufriedenheit unserer Kunden. Wir bitten um Verständnis bei Wiederholungen und eingänglichen Eingewöhnungskomplikationen.", Mood.COOL);
        DialogSys.addDialog("Player", "Lad einfach das nächste Level. Ich hab doch verstanden, worum es geht.", "cmd_dark");
        DialogSys.addDialog("AI", "Einen Moment bitte.", Mood.ANGRY, "cmd_toLevel3");
        DialogSys.display();
    }

    public void onOptoInterface()
    {
        cmd_2();
    }

    public void cmd_toLevel3()
    {
        Invoke("cmd_PortToLevel3", 7);
    }
    public void cmd_PortToLevel3()
    {
        Physics.gravity = new Vector3(0, -9.8f, 0);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level3_VR");
    }
}
